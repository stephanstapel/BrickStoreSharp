/*
 * Licensed to the Apache Software Foundation (ASF) under one
 * or more contributor license agreements.  See the NOTICE file
 * distributed with this work for additional information
 * regarding copyright ownership.  The ASF licenses this file
 * to you under the Apache License, Version 2.0 (the
 * "License"); you may not use this file except in compliance
 * with the License.  You may obtain a copy of the License at
 * 
 *   http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing,
 * software distributed under the License is distributed on an
 * "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
 * KIND, either express or implied.  See the License for the
 * specific language governing permissions and limitations
 * under the License.
 */
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace BrickStoreSharp
{
    internal class BrickStoreWriter
    {
        internal async Task SaveAsync(BrickStoreInventory inventory, Stream stream)
        {
            if (!stream.CanWrite || !stream.CanSeek)
            {
                throw new IllegalStreamException("Cannot write to stream");
            }

            long streamPosition = stream.Position;

            
            XmlTextWriter writer = new XmlTextWriter(stream, Encoding.UTF8);
            writer.Formatting = Formatting.Indented;
            writer.WriteStartDocument();

            #region XML-Kopfbereich
            if (!String.IsNullOrWhiteSpace(inventory.Comment))
            {
                writer.WriteComment(inventory.Comment);
            }
            writer.WriteDocType("BrickStoreXML", null, null, null);
            writer.WriteStartElement("BrickStoreXML");
            writer.WriteStartElement("Inventory");
            #endregion // !XML Header                       

            foreach (BrickStoreInventoryItem item in inventory.Items)
            {
                writer.WriteStartElement("Item");
                writer.WriteElementString("ItemID", item.Id);
                writer.WriteElementString("ItemTypeID", _mapItemType(item.ItemType));

                if (item.ColorId.HasValue)
                {
                    writer.WriteElementString("ColorID", $"{item.ColorId}");
                }

                writer.WriteElementString("ItemName", item.Name);
                writer.WriteElementString("ItemTypeName", _mapItemTypeName(item.ItemType));
                writer.WriteElementString("ColorName", item.ColorName);
                writer.WriteElementString("CategoryID", $"{item.CategoryId}");
                writer.WriteElementString("CategoryName", item.CategoryName);

                if (item.Status != StatusCodes.Unknown)
                {
                    writer.WriteElementString("Status", item.Status.EnumToString());
                }

                if (item.Quantity.HasValue)
                {
                    writer.WriteElementString("Qty", $"{item.Quantity}");
                }

                if (item.Price.HasValue)
                {
                    writer.WriteElementString("Price", item.Price.Value.ToString("N4").Replace(",", "."));
                }

                writer.WriteElementString("Condition", item.Condition.EnumToString());
                writer.WriteElementString("Bulk", $"{item.Bulk}");
                writer.WriteElementString("Sale", $"{item.Sale}");

                if (!String.IsNullOrWhiteSpace(item.Comments))
                {
                    writer.WriteElementString("Comments", item.Comments);
                }

                if (!String.IsNullOrWhiteSpace(item.Remarks))
                {
                    writer.WriteElementString("Remarks", item.Remarks);
                }

                if (item.Retain)
                {
                    writer.WriteStartElement("Retain");
                    writer.WriteEndElement();
                }

                writer.WriteElementString("LotID", $"{item.LotId}");
                writer.WriteElementString("OwlID", $"{item.OwlId}");
                writer.WriteElementString("OwlLotID", $"{item.OwlLotId}");

                if (item.TieredPrice != null)
                {
                    if (item.TieredPrice.TieredQuantity1 != 0)
                    {
                        writer.WriteElementString("TQ1", $"{item.TieredPrice.TieredQuantity1}");
                        writer.WriteElementString("TP1", item.TieredPrice.TieredPrice1.ToString("N4").Replace(",", "."));
                    }

                    if (item.TieredPrice.TieredQuantity2 != 0)
                    {
                        writer.WriteElementString("TQ2", $"{item.TieredPrice.TieredQuantity2}");
                        writer.WriteElementString("TP2", item.TieredPrice.TieredPrice2.ToString("N4").Replace(",", "."));
                    }

                    if (item.TieredPrice.TieredQuantity3 != 0)
                    {
                        writer.WriteElementString("TQ3", $"{item.TieredPrice.TieredQuantity3}");
                        writer.WriteElementString("TP3", item.TieredPrice.TieredPrice3.ToString("N4").Replace(",", "."));
                    }
                }

                if (!String.IsNullOrWhiteSpace(_mapSubCondition(item.SubCondition)))
                {
                    writer.WriteElementString("SubCondition", _mapSubCondition(item.SubCondition));
                }

                if (!String.IsNullOrWhiteSpace(item.Stockroom))
                {
                    writer.WriteElementString("Stockroom", item.Stockroom);
                }

                writer.WriteEndElement(); // !Item
            }

            writer.WriteEndElement(); // !Inventory
            writer.WriteEndElement(); // !BrickStoreXML
            writer.WriteEndDocument();
            writer.Flush();

            stream.Seek(streamPosition, SeekOrigin.Begin);

            await Task.CompletedTask;
        } // !SaveAsync()


        private string _mapSubCondition(SubConditions subcondition)
        {
            switch (subcondition)
            {        
                case SubConditions.Complete: return "C";
                case SubConditions.Incomplete: return "I";
                case SubConditions.Sealed: return "M";
            }
            return String.Empty;
        } // !_mapSubCondition()


        private string _mapItemType(ItemTypes itemType)
        {
            switch (itemType)
            {
                case ItemTypes.Book: return "B";
                case ItemTypes.Catalog: return "C";
                case ItemTypes.Gear: return "G";
                case ItemTypes.Instruction: return "I";
                case ItemTypes.Minifigure: return "M";
                case ItemTypes.OriginalBox: return "O";
                case ItemTypes.Part: return "P";
                case ItemTypes.Set: return "S";
                default: return ""; // oder eine andere Standardbehandlung
            }
        } // !_mapItemType()


        private string _mapItemTypeName(ItemTypes itemType)
        {
            switch (itemType)
            {
                case ItemTypes.Book: return "Book";
                case ItemTypes.Catalog: return "Catalog";
                case ItemTypes.Gear: return "Gear";
                case ItemTypes.Instruction: return "Instruction";
                case ItemTypes.Minifigure: return "Minifig";
                case ItemTypes.OriginalBox: return "OriginalBox";
                case ItemTypes.Part: return "Part";
                case ItemTypes.Set: return "Set";
                default: return ""; // oder eine andere Standardbehandlung
            }
        } // !_mapItemTypeName()


        public async Task SaveAsync(BrickStoreInventory inventory, string filename)
        {
            FileStream fs = new FileStream(filename, FileMode.Create, FileAccess.Write);
            await SaveAsync(inventory, fs);
            fs.Flush();
            fs.Close();
        } // !SaveAsync()
    }
}

