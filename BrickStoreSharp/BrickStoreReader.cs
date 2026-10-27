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
using System.Threading.Tasks;
using System.Xml;

namespace BrickStoreSharp
{
    internal class BrickStoreReader
    {
        /// <summary>
        /// Item elements mapped to properties of BrickStoreInventoryItem, everything else is kept in AdditionalElements
        /// </summary>
        private static readonly HashSet<string> _KnownItemElements = new HashSet<string>()
        {
            "ItemID", "ItemTypeID", "ColorID", "ItemName", "ItemTypeName", "ColorName", "CategoryID", "CategoryName",
            "Status", "Qty", "Price", "Condition", "Bulk", "Sale", "Comments", "Remarks", "Retain", "Reserved",
            "LotID", "OwlID", "OwlLotID", "TQ1", "TP1", "TQ2", "TP2", "TQ3", "TP3", "TotalWeight", "Cost",
            "SubCondition", "Stockroom", "MarkerText", "MarkerColor"
        };

        internal static BrickStoreInventory Load(string filename)
        {
            return LoadAsync(filename).GetAwaiter().GetResult();
        } // !Load()


        internal static BrickStoreInventory Load(Stream stream)
        {
            return LoadAsync(stream).GetAwaiter().GetResult();
        } // !Load()


        public static async Task<BrickStoreInventory> LoadAsync(Stream stream)
        {
            if (!stream.CanRead)
            {
                throw new IllegalStreamException("Cannot read from stream");
            }

            // read the file, DTDs are not needed and must not resolve external entities
            XmlReaderSettings settings = new XmlReaderSettings()
            {
                DtdProcessing = DtdProcessing.Ignore,
                XmlResolver = null
            };

            XmlDocument doc = new XmlDocument() { XmlResolver = null };
            using (XmlReader reader = XmlReader.Create(stream, settings))
            {
                doc.Load(reader);
            }

            // Currency and BrickLinkChangelogId are attributes of <Inventory>, see BrickStoreXML.rnc
            XmlNode inventoryNode = doc.DocumentElement.SelectSingleNode("./Inventory");
            BrickStoreInventory retval = new BrickStoreInventory()
            {
                // the writer stores Comment as XML comment in front of the root element
                Comment = doc.ChildNodes.OfType<XmlNode>().TakeWhile(n => n != doc.DocumentElement).OfType<XmlComment>().FirstOrDefault()?.Value
            };

            if (inventoryNode != null)
            {
                retval.Currency = XmlUtils.AttributeText(inventoryNode, "Currency", null);
                if (Int32.TryParse(XmlUtils.AttributeText(inventoryNode, "BrickLinkChangelogId"), NumberStyles.Integer, CultureInfo.InvariantCulture, out int changelogId))
                {
                    retval.BrickLinkChangelogId = changelogId;
                }
            }

            foreach (XmlNode itemNode in doc.DocumentElement.SelectNodes("./Inventory/Item"))
            {
                BrickStoreInventoryItem item = new BrickStoreInventoryItem()
                {
                    Id = XmlUtils.NodeAsString(itemNode, "./ItemID"),
                    ItemType = default(ItemTypes).FromCode(XmlUtils.NodeAsString(itemNode, "./ItemTypeID")),
                    ColorId = XmlUtils.NodeAsInt(itemNode, "./ColorID"),
                    Name = XmlUtils.NodeAsString(itemNode, "./ItemName"),
                    ColorName = XmlUtils.NodeAsString(itemNode, "./ColorName"),
                    Status = default(StatusCodes).FromString(XmlUtils.NodeAsString(itemNode, "./Status")),
                    Quantity = XmlUtils.NodeAsInt(itemNode, "./Qty"),
                    Price = XmlUtils.NodeAsDecimal(itemNode, "./Price"),
                    Condition = default(ConditionCodes).FromString(XmlUtils.NodeAsString(itemNode, "./Condition")),
                    Remarks = XmlUtils.NodeAsString(itemNode, "./Remarks"),
                    LotId = XmlUtils.NodeAsInt(itemNode, "./LotID"),
                    OwlId = XmlUtils.NodeAsString(itemNode, "./OwlID"),
                    OwlLotId = XmlUtils.NodeAsInt(itemNode, "./OwlLotID"),
                    CategoryId = XmlUtils.NodeAsInt(itemNode, "./CategoryID") ?? 0,
                    CategoryName = XmlUtils.NodeAsString(itemNode, "./CategoryName", defaultValue: null),
                    Comments = XmlUtils.NodeAsString(itemNode, "./Comments", defaultValue: null),
                    Retain = XmlUtils.SelectSingleNode(itemNode, "./Retain") != null,
                    Stockroom = XmlUtils.NodeAsString(itemNode, "./Stockroom", defaultValue: null),
                    SubCondition = default(SubConditions).FromCode(XmlUtils.NodeAsString(itemNode, "./SubCondition")),
                    TieredPrice = _readTieredPrice(itemNode),
                    Reserved = XmlUtils.NodeAsString(itemNode, "./Reserved", defaultValue: null),
                    Cost = XmlUtils.NodeAsDecimal(itemNode, "./Cost"),
                    TotalWeight = XmlUtils.NodeAsDecimal(itemNode, "./TotalWeight"),
                    MarkerText = XmlUtils.NodeAsString(itemNode, "./MarkerText", defaultValue: null),
                    MarkerColor = XmlUtils.NodeAsString(itemNode, "./MarkerColor", defaultValue: null),
                    AdditionalElements = itemNode.ChildNodes.OfType<XmlElement>().Where(e => !_KnownItemElements.Contains(e.Name)).ToList()
                };

                // keep the item's defaults if the optional elements are missing
                int? bulk = XmlUtils.NodeAsInt(itemNode, "./Bulk");
                if (bulk.HasValue)
                {
                    item.Bulk = bulk.Value;
                }

                int? sale = XmlUtils.NodeAsInt(itemNode, "./Sale");
                if (sale.HasValue)
                {
                    item.Sale = sale.Value;
                }

                retval.Items.Add(item);
            }

            // e.g. GuiState, kept to not lose it on save
            retval.AdditionalElements = doc.DocumentElement.ChildNodes.OfType<XmlElement>().Where(e => e.Name != "Inventory").ToList();

            return retval;
        } // !LoadAsync()


        private static TieredPrice _readTieredPrice(XmlNode itemNode)
        {
            int? tq1 = XmlUtils.NodeAsInt(itemNode, "./TQ1");
            int? tq2 = XmlUtils.NodeAsInt(itemNode, "./TQ2");
            int? tq3 = XmlUtils.NodeAsInt(itemNode, "./TQ3");
            if (!tq1.HasValue && !tq2.HasValue && !tq3.HasValue)
            {
                return null;
            }

            return new TieredPrice()
            {
                TieredQuantity1 = tq1 ?? 0,
                TieredPrice1 = XmlUtils.NodeAsDecimal(itemNode, "./TP1") ?? 0m,
                TieredQuantity2 = tq2 ?? 0,
                TieredPrice2 = XmlUtils.NodeAsDecimal(itemNode, "./TP2") ?? 0m,
                TieredQuantity3 = tq3 ?? 0,
                TieredPrice3 = XmlUtils.NodeAsDecimal(itemNode, "./TP3") ?? 0m
            };
        } // !_readTieredPrice()


        internal static async Task<BrickStoreInventory> LoadAsync(string filename)
        {
            // throws FileNotFoundException including the file name if the file does not exist
            using (FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read))
            {
                return await LoadAsync(fs);
            }
        } // !LoadAsync()
    }
}
