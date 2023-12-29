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
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;

namespace BrickStoreSharp
{
    internal class BrickStoreReader
    {
        internal static BrickStoreInventory Load(string filename)
        {
            Task<BrickStoreInventory> t = LoadAsync(filename);
            t.Wait();
            return t.Result;
        } // !Load()


        internal static BrickStoreInventory Load(Stream stream)
        {
            Task<BrickStoreInventory> t = LoadAsync(stream);
            t.Wait();
            return t.Result;
        } // !Load()


        public static async Task<BrickStoreInventory> LoadAsync(Stream stream)
        {
            if (!stream.CanRead)
            {
                throw new IllegalStreamException("Cannot read from stream");
            }

            // read the file            
            XmlDocument doc = new XmlDocument();
            doc.Load(stream);

            BrickStoreInventory retval = new BrickStoreInventory()
            {
                Currency = XmlUtils.NodeAsString(doc.DocumentElement, "./Currency"),
                BrickLinkChangelogId = XmlUtils.NodeAsInt(doc.DocumentElement, "./BrickLinkChangelogId")
            };

            Parallel.ForEach(doc.DocumentElement.SelectNodes("./Inventory/Item").Cast<XmlNode>(), (XmlNode itemNode) =>
            {
                BrickStoreInventoryItem item = new BrickStoreInventoryItem()
                {
                    Id = XmlUtils.NodeAsString(itemNode, "./ItemID"),
                    ItemTypeId = XmlUtils.NodeAsString(itemNode, "./ItemTypeID"),
                    ColorId = XmlUtils.NodeAsInt(itemNode, "./ColorID"),
                    Name = XmlUtils.NodeAsString(itemNode, "./ItemName"),
                    ItemType = XmlUtils.NodeAsString(itemNode, "./ItemTypeName"),
                    ColorName = XmlUtils.NodeAsString(itemNode, "./ColorName"),
                    Status = default(StatusCodes).FromString(XmlUtils.NodeAsString(itemNode, "./Status")),
                    Quantity = XmlUtils.NodeAsInt(itemNode, "./Qty"),
                    Price = XmlUtils.NodeAsDecimal(itemNode, "./Price"),
                    Condition = default(ConditionCodes).FromString(XmlUtils.NodeAsString(itemNode, "./Condition")),
                    Remarks = XmlUtils.NodeAsString(itemNode, "./Remarks"),
                    LotId = XmlUtils.NodeAsInt(itemNode, "./LotID"),
                    OwlId = XmlUtils.NodeAsInt(itemNode, "./OwlID"),
                    OwlLotId = XmlUtils.NodeAsInt(itemNode, "./OwlLotID")
                };


                retval.Items.Add(item);
            });

            return retval;
        } // !LoadAsync()


        internal static async Task<BrickStoreInventory> LoadAsync(string filename)
        {
            if (!System.IO.File.Exists(filename))
            {
                throw new FileNotFoundException();
            }

            return await LoadAsync(new FileStream(filename, FileMode.Open, FileAccess.Read));
        } // !LoadAsync()
    }
}
