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

            // elements not interpreted by this library are imported into a separate document,
            // so the loaded document does not stay in memory as long as the inventory is used
            XmlDocument additionalElementsOwner = new XmlDocument();

            if (inventoryNode != null)
            {
                retval.Currency = XmlUtils.AttributeText(inventoryNode, "Currency", null);
                if (Int32.TryParse(XmlUtils.AttributeText(inventoryNode, "BrickLinkChangelogId"), NumberStyles.Integer, CultureInfo.InvariantCulture, out int changelogId))
                {
                    retval.BrickLinkChangelogId = changelogId;
                }

                foreach (XmlElement itemNode in inventoryNode.ChildNodes.OfType<XmlElement>().Where(e => e.Name == "Item"))
                {
                    retval.Items.Add(_readItem(itemNode, additionalElementsOwner));
                }
            }

            // e.g. GuiState, kept to not lose it on save
            retval.AdditionalElements = doc.DocumentElement.ChildNodes.OfType<XmlElement>()
                                                           .Where(e => e != inventoryNode)
                                                           .Select(e => (XmlElement)additionalElementsOwner.ImportNode(e, true))
                                                           .ToList();

            return retval;
        } // !LoadAsync()


        /// <summary>
        /// Reads all child elements of an item in a single pass. Every element that is mapped to a property is
        /// consumed, all remaining elements are kept in AdditionalElements and written back unchanged.
        /// </summary>
        private static BrickStoreInventoryItem _readItem(XmlElement itemNode, XmlDocument additionalElementsOwner)
        {
            Dictionary<string, XmlElement> elements = new Dictionary<string, XmlElement>();
            foreach (XmlElement element in itemNode.ChildNodes.OfType<XmlElement>())
            {
                if (!elements.ContainsKey(element.Name))
                {
                    elements.Add(element.Name, element);
                }
            }

            HashSet<XmlElement> consumed = new HashSet<XmlElement>();

            bool exists(string name)
            {
                if (elements.TryGetValue(name, out XmlElement element))
                {
                    consumed.Add(element);
                    return true;
                }
                return false;
            }

            string text(string name)
            {
                return elements.TryGetValue(name, out XmlElement element) && consumed.Add(element) ? element.InnerText : null;
            }

            int? integer(string name)
            {
                return Int32.TryParse(text(name), NumberStyles.Integer, CultureInfo.InvariantCulture, out int value) ? value : (int?)null;
            }

            decimal? number(string name)
            {
                return Decimal.TryParse(text(name), NumberStyles.Float, CultureInfo.InvariantCulture, out decimal value) ? value : (decimal?)null;
            }

            BrickStoreInventoryItem item = new BrickStoreInventoryItem()
            {
                Id = text("ItemID"),
                ColorId = integer("ColorID"),
                Name = text("ItemName"),
                ColorName = text("ColorName"),
                CategoryId = integer("CategoryID") ?? 0,
                CategoryName = text("CategoryName"),
                Status = default(StatusCodes).FromString(text("Status")),
                Quantity = integer("Qty"),
                Price = number("Price"),
                Condition = default(ConditionCodes).FromString(text("Condition")),
                Comments = text("Comments"),
                Remarks = text("Remarks"),
                Retain = exists("Retain"),
                Reserved = text("Reserved"),
                LotId = integer("LotID"),
                OwlId = text("OwlID"),
                OwlLotId = integer("OwlLotID"),
                TotalWeight = number("TotalWeight"),
                Cost = number("Cost"),
                SubCondition = default(SubConditions).FromCode(text("SubCondition")),
                Stockroom = text("Stockroom"),
                MarkerText = text("MarkerText"),
                MarkerColor = text("MarkerColor")
            };

            // item types this library does not know (e.g. a new BrickLink type) are kept unchanged
            // in AdditionalElements instead of being written back as an empty ItemTypeID
            string itemTypeCode = elements.TryGetValue("ItemTypeID", out XmlElement itemTypeElement) ? itemTypeElement.InnerText : null;
            item.ItemType = default(ItemTypes).FromCode(itemTypeCode);
            if ((item.ItemType != ItemTypes.Unknown) || String.IsNullOrEmpty(itemTypeCode))
            {
                exists("ItemTypeID");
                exists("ItemTypeName"); // derived from ItemType when writing
            }

            // keep the item's defaults if the optional elements are missing
            int? bulk = integer("Bulk");
            if (bulk.HasValue)
            {
                item.Bulk = bulk.Value;
            }

            int? sale = integer("Sale");
            if (sale.HasValue)
            {
                item.Sale = sale.Value;
            }

            int? tq1 = integer("TQ1");
            int? tq2 = integer("TQ2");
            int? tq3 = integer("TQ3");
            decimal? tp1 = number("TP1");
            decimal? tp2 = number("TP2");
            decimal? tp3 = number("TP3");
            if (tq1.HasValue || tq2.HasValue || tq3.HasValue)
            {
                item.TieredPrice = new TieredPrice()
                {
                    TieredQuantity1 = tq1 ?? 0,
                    TieredPrice1 = tp1 ?? 0m,
                    TieredQuantity2 = tq2 ?? 0,
                    TieredPrice2 = tp2 ?? 0m,
                    TieredQuantity3 = tq3 ?? 0,
                    TieredPrice3 = tp3 ?? 0m
                };
            }

            item.AdditionalElements = itemNode.ChildNodes.OfType<XmlElement>()
                                                         .Where(e => !consumed.Contains(e))
                                                         .Select(e => (XmlElement)additionalElementsOwner.ImportNode(e, true))
                                                         .ToList();
            return item;
        } // !_readItem()


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
