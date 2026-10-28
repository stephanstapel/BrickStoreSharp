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
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace BrickStoreSharp
{
    internal class BrickStoreWriter
    {
        /// <summary>
        /// At least four decimals like BrickStore, but never rounds away more precise values
        /// </summary>
        private const string _PriceFormat = "0.0000########################";

        internal async Task SaveAsync(BrickStoreInventory inventory, Stream stream)
        {
            if (!stream.CanWrite || !stream.CanSeek)
            {
                throw new IllegalStreamException("Cannot write to stream");
            }

            long streamPosition = stream.Position;

            
            // XmlWriter.Create checks characters and comments, so invalid input throws instead of producing unreadable files
            XmlWriterSettings settings = new XmlWriterSettings()
            {
                Encoding = Encoding.UTF8,
                Indent = true,
                CloseOutput = false
            };

            XmlWriter writer = XmlWriter.Create(stream, settings);
            writer.WriteStartDocument();

            #region XML-Kopfbereich
            if (!String.IsNullOrWhiteSpace(inventory.Comment))
            {
                writer.WriteComment(inventory.Comment);
            }
            writer.WriteDocType("BrickStoreXML", null, null, null);
            writer.WriteStartElement("BrickStoreXML");
            writer.WriteStartElement("Inventory");

            // Currency and BrickLinkChangelogId are attributes of <Inventory>, see BrickStoreXML.rnc
            if (!String.IsNullOrWhiteSpace(inventory.Currency))
            {
                writer.WriteAttributeString("Currency", inventory.Currency);
            }

            if (inventory.BrickLinkChangelogId.HasValue)
            {
                writer.WriteAttributeString("BrickLinkChangelogId", inventory.BrickLinkChangelogId.Value.ToString(CultureInfo.InvariantCulture));
            }
            #endregion // !XML Header                       

            foreach (BrickStoreInventoryItem item in inventory.Items)
            {
                writer.WriteStartElement("Item");
                writer.WriteElementString("ItemID", item.Id);
                // an item type unknown to this library is kept in AdditionalElements by the reader
                if ((item.ItemType != ItemTypes.Unknown) || !_containsElement(item.AdditionalElements, "ItemTypeID"))
                {
                    writer.WriteElementString("ItemTypeID", item.ItemType.ToCode());
                }

                // ColorID is required by BrickStoreXML.rnc, 0 is BrickLink's '(Not Applicable)' color
                writer.WriteElementString("ColorID", (item.ColorId ?? 0).ToString(CultureInfo.InvariantCulture));

                // optional elements: BrickStoreXML.rnc asks not to write them if they equal the default value
                _writeOptionalElement(writer, "ItemName", item.Name);
                _writeOptionalElement(writer, "ItemTypeName", item.ItemType.ToName());
                _writeOptionalElement(writer, "ColorName", item.ColorName);

                if (item.CategoryId != 0)
                {
                    writer.WriteElementString("CategoryID", item.CategoryId.ToString(CultureInfo.InvariantCulture));
                }

                _writeOptionalElement(writer, "CategoryName", item.CategoryName);

                if (item.Status != StatusCodes.Unknown)
                {
                    writer.WriteElementString("Status", item.Status.ToCode());
                }

                if (item.Quantity.HasValue)
                {
                    writer.WriteElementString("Qty", $"{item.Quantity}");
                }

                if (item.Price.HasValue)
                {
                    writer.WriteElementString("Price", item.Price.Value.ToString(_PriceFormat, CultureInfo.InvariantCulture));
                }

                if (item.Condition != ConditionCodes.Unknown)
                {
                    writer.WriteElementString("Condition", item.Condition.ToCode());
                }

                if (item.Bulk != 1)
                {
                    writer.WriteElementString("Bulk", item.Bulk.ToString(CultureInfo.InvariantCulture));
                }

                if (item.Sale != 0)
                {
                    writer.WriteElementString("Sale", item.Sale.ToString(CultureInfo.InvariantCulture));
                }

                _writeOptionalElement(writer, "Comments", item.Comments);
                _writeOptionalElement(writer, "Remarks", item.Remarks);

                if (item.Retain)
                {
                    writer.WriteStartElement("Retain");
                    writer.WriteEndElement();
                }

                if (item.LotId.HasValue && item.LotId.Value != 0)
                {
                    writer.WriteElementString("LotID", item.LotId.Value.ToString(CultureInfo.InvariantCulture));
                }

                _writeOptionalElement(writer, "OwlID", item.OwlId);

                if (item.OwlLotId.HasValue)
                {
                    writer.WriteElementString("OwlLotID", item.OwlLotId.Value.ToString(CultureInfo.InvariantCulture));
                }

                if (item.TieredPrice != null)
                {
                    if (item.TieredPrice.TieredQuantity1 != 0)
                    {
                        writer.WriteElementString("TQ1", $"{item.TieredPrice.TieredQuantity1}");
                        writer.WriteElementString("TP1", item.TieredPrice.TieredPrice1.ToString(_PriceFormat, CultureInfo.InvariantCulture));
                    }

                    if (item.TieredPrice.TieredQuantity2 != 0)
                    {
                        writer.WriteElementString("TQ2", $"{item.TieredPrice.TieredQuantity2}");
                        writer.WriteElementString("TP2", item.TieredPrice.TieredPrice2.ToString(_PriceFormat, CultureInfo.InvariantCulture));
                    }

                    if (item.TieredPrice.TieredQuantity3 != 0)
                    {
                        writer.WriteElementString("TQ3", $"{item.TieredPrice.TieredQuantity3}");
                        writer.WriteElementString("TP3", item.TieredPrice.TieredPrice3.ToString(_PriceFormat, CultureInfo.InvariantCulture));
                    }
                }

                if (item.SubCondition != SubConditions.Unknown)
                {
                    writer.WriteElementString("SubCondition", item.SubCondition.ToCode());
                }

                _writeOptionalElement(writer, "Stockroom", item.Stockroom);
                _writeOptionalElement(writer, "Reserved", item.Reserved);

                if (item.Cost.HasValue)
                {
                    writer.WriteElementString("Cost", item.Cost.Value.ToString(_PriceFormat, CultureInfo.InvariantCulture));
                }

                if (item.TotalWeight.HasValue)
                {
                    writer.WriteElementString("TotalWeight", item.TotalWeight.Value.ToString(CultureInfo.InvariantCulture));
                }

                _writeOptionalElement(writer, "MarkerText", item.MarkerText);
                _writeOptionalElement(writer, "MarkerColor", item.MarkerColor);
                _writeAdditionalElements(writer, item.AdditionalElements);

                writer.WriteEndElement(); // !Item
            }

            writer.WriteEndElement(); // !Inventory
            _writeAdditionalElements(writer, inventory.AdditionalElements);
            writer.WriteEndElement(); // !BrickStoreXML
            writer.WriteEndDocument();
            writer.Flush();

            stream.Seek(streamPosition, SeekOrigin.Begin);

            await Task.CompletedTask;
        } // !SaveAsync()


        private static bool _containsElement(List<XmlElement> elements, string name)
        {
            return (elements != null) && elements.Any(e => (e != null) && (e.Name == name));
        } // !_containsElement()


        private static void _writeOptionalElement(XmlWriter writer, string name, string value)
        {
            if (!String.IsNullOrWhiteSpace(value))
            {
                writer.WriteElementString(name, value);
            }
        } // !_writeOptionalElement()


        private static void _writeAdditionalElements(XmlWriter writer, List<XmlElement> elements)
        {
            if (elements == null)
            {
                return;
            }

            foreach (XmlElement element in elements.Where(e => e != null))
            {
                element.WriteTo(writer);
            }
        } // !_writeAdditionalElements()


        public async Task SaveAsync(BrickStoreInventory inventory, string filename)
        {
            // write to a temporary file first, so an existing file is not destroyed if writing fails
            string fullPath = Path.GetFullPath(filename);
            string tempPath = Path.Combine(Path.GetDirectoryName(fullPath), $"{Path.GetFileName(fullPath)}.{Guid.NewGuid():N}.tmp");
            try
            {
                using (FileStream fs = new FileStream(tempPath, FileMode.CreateNew, FileAccess.Write))
                {
                    await SaveAsync(inventory, fs);
                    fs.Flush();
                }

                if (File.Exists(fullPath))
                {
                    File.Replace(tempPath, fullPath, null);
                }
                else
                {
                    File.Move(tempPath, fullPath);
                }
            }
            finally
            {
                if (File.Exists(tempPath))
                {
                    File.Delete(tempPath);
                }
            }
        } // !SaveAsync()
    }
}

