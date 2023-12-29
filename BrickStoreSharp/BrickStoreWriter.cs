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
            writer.WriteDocType("BrickStoreXML", null, null, null);
            writer.WriteStartElement("BrickStoreXML");
            writer.WriteStartElement("Inventory");            
            #endregion // !XML Header


            foreach(BrickStoreInventoryItem item in inventory.Items)
            {
                writer.WriteStartElement("Item");
                writer.WriteElementString("ItemID", item.Id);
                writer.WriteElementString("ItemTypeID", item.ItemTypeId);

                if (item.ColorId.HasValue)
                {
                    writer.WriteElementString("ColorID", $"{item.ColorId}");
                }

                writer.WriteElementString("ItemName", item.Name);
                writer.WriteElementString("ItemTypeName", item.ItemType);

                writer.WriteElementString("ColorName", item.ColorName);
                writer.WriteElementString("Status", item.Status.EnumToString());

                if (item.Quantity.HasValue)
                {
                    writer.WriteElementString("Qty", $"{item.Quantity}");
                }

                if (item.Price.HasValue)
                {
                    writer.WriteElementString("Price", item.Price.Value.ToString("N4"));
                }

                writer.WriteElementString("Condition", item.Condition.EnumToString());
                writer.WriteElementString("Remarks", item.Remarks);
                writer.WriteElementString("LotID", $"{item.LotId}");
                writer.WriteElementString("OwlID", $"{item.OwlId}");
                writer.WriteElementString("OwlLotID", $"{item.OwlLotId}");

                writer.WriteEndElement(); // !Item
            }

            writer.WriteEndElement(); // !Inventory
            writer.WriteEndElement(); // !BrickStoreXML
            writer.WriteEndDocument();
            writer.Flush();

            stream.Seek(streamPosition, SeekOrigin.Begin);

            await Task.CompletedTask;
        } // !SaveAsync()


        public async Task SaveAsync(BrickStoreInventory inventory, string filename)
        {
            FileStream fs = new FileStream(filename, FileMode.Create, FileAccess.Write);
            await SaveAsync(inventory, fs);
            fs.Flush();
            fs.Close();
        } // !SaveAsync()
    }
}

