using System.Globalization;
using System.Xml;
using BrickStoreSharp;

namespace BrickStoreSharp_Test
{
    [TestClass]
    public class BrickStoreWriterTests
    {
        private static BrickStoreInventoryItem _createItem()
        {
            return new BrickStoreInventoryItem()
            {
                Id = "3001",
                ItemType = ItemTypes.Part,
                ColorId = 11,
                Name = "Brick 2 x 4",
                ColorName = "Black",
                CategoryId = 5,
                CategoryName = "Brick",
                Status = StatusCodes.Include,
                Quantity = 42,
                Price = 0.125m,
                Condition = ConditionCodes.New,
                Remarks = "Box 7",
                LotId = 123456
            };
        } // !_createItem()


        private static XmlDocument _save(BrickStoreInventory inventory)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                inventory.Save(ms);
                XmlDocument doc = new XmlDocument();
                doc.Load(ms);
                return doc;
            }
        } // !_save()


        private static XmlNode _saveSingleItem(BrickStoreInventoryItem item)
        {
            BrickStoreInventory inventory = new BrickStoreInventory();
            inventory.Items.Add(item);
            return _save(inventory).SelectSingleNode("/BrickStoreXML/Inventory/Item")!;
        } // !_saveSingleItem()


        [TestMethod]
        public void Save_WritesDocumentStructure()
        {
            BrickStoreInventory inventory = new BrickStoreInventory();
            inventory.Items.Add(_createItem());
            inventory.Items.Add(_createItem());

            XmlDocument doc = _save(inventory);

            Assert.AreEqual("BrickStoreXML", doc.DocumentElement!.Name);
            Assert.AreEqual("BrickStoreXML", doc.DocumentType?.Name);
            Assert.AreEqual(2, doc.SelectNodes("/BrickStoreXML/Inventory/Item")!.Count);
        }


        [TestMethod]
        public void Save_WritesInventoryHeaderAsAttributes()
        {
            XmlDocument doc = _save(new BrickStoreInventory() { Currency = "EUR", BrickLinkChangelogId = 4711 });

            XmlNode inventory = doc.SelectSingleNode("/BrickStoreXML/Inventory")!;
            Assert.AreEqual("EUR", inventory.Attributes?["Currency"]?.Value);
            Assert.AreEqual("4711", inventory.Attributes?["BrickLinkChangelogId"]?.Value);
            Assert.IsNull(doc.SelectSingleNode("/BrickStoreXML/Currency"));
        }


        [TestMethod]
        public void Save_OmitsInventoryHeaderWhenUnset()
        {
            XmlNode inventory = _save(new BrickStoreInventory()).SelectSingleNode("/BrickStoreXML/Inventory")!;
            Assert.AreEqual(0, inventory.Attributes?.Count);
        }


        [TestMethod]
        public void Save_WritesItemFields()
        {
            XmlNode item = _saveSingleItem(_createItem());

            Assert.AreEqual("3001", item.SelectSingleNode("ItemID")?.InnerText);
            Assert.AreEqual("P", item.SelectSingleNode("ItemTypeID")?.InnerText);
            Assert.AreEqual("Part", item.SelectSingleNode("ItemTypeName")?.InnerText);
            Assert.AreEqual("11", item.SelectSingleNode("ColorID")?.InnerText);
            Assert.AreEqual("Brick 2 x 4", item.SelectSingleNode("ItemName")?.InnerText);
            Assert.AreEqual("Black", item.SelectSingleNode("ColorName")?.InnerText);
            Assert.AreEqual("5", item.SelectSingleNode("CategoryID")?.InnerText);
            Assert.AreEqual("Brick", item.SelectSingleNode("CategoryName")?.InnerText);
            Assert.AreEqual("42", item.SelectSingleNode("Qty")?.InnerText);
            Assert.AreEqual("Box 7", item.SelectSingleNode("Remarks")?.InnerText);
            Assert.AreEqual("123456", item.SelectSingleNode("LotID")?.InnerText);
            Assert.IsNull(item.SelectSingleNode("Retain"));
            Assert.IsNull(item.SelectSingleNode("SubCondition"));
        }


        [TestMethod]
        public void Save_UsesBrickStoreCodesForStatusAndCondition()
        {
            // BrickStore itself uses single letter codes: Status I/X/E, Condition N/U
            XmlNode item = _saveSingleItem(_createItem());

            Assert.AreEqual("I", item.SelectSingleNode("Status")?.InnerText);
            Assert.AreEqual("N", item.SelectSingleNode("Condition")?.InnerText);
        }


        [TestMethod]
        public void Save_OmitsOptionalFieldsWhenUnset()
        {
            XmlNode item = _saveSingleItem(new BrickStoreInventoryItem() { Id = "3001", ItemType = ItemTypes.Part });

            Assert.IsNull(item.SelectSingleNode("ColorID"));
            Assert.IsNull(item.SelectSingleNode("Status"));
            Assert.IsNull(item.SelectSingleNode("Qty"));
            Assert.IsNull(item.SelectSingleNode("Price"));
            Assert.IsNull(item.SelectSingleNode("Remarks"));
            Assert.IsNull(item.SelectSingleNode("Comments"));
            Assert.IsNull(item.SelectSingleNode("Stockroom"));
            Assert.IsNull(item.SelectSingleNode("Condition"));
            Assert.IsNull(item.SelectSingleNode("SubCondition"));
        }


        [DataTestMethod]
        [DataRow("en-US")]
        [DataRow("de-DE")]
        [DataRow("fr-FR")]
        public void Save_WritesPriceInvariant(string culture)
        {
            CultureInfo previous = CultureInfo.CurrentCulture;
            try
            {
                CultureInfo.CurrentCulture = new CultureInfo(culture);
                BrickStoreInventoryItem source = _createItem();
                source.Price = 1234.5m;

                XmlNode item = _saveSingleItem(source);

                string price = item.SelectSingleNode("Price")!.InnerText;
                Assert.AreEqual(1234.5m, decimal.Parse(price, NumberStyles.Number, CultureInfo.InvariantCulture), $"written price: '{price}'");
            }
            finally
            {
                CultureInfo.CurrentCulture = previous;
            }
        }


        [TestMethod]
        public void Save_WritesSmallPriceWithDecimalPoint()
        {
            CultureInfo previous = CultureInfo.CurrentCulture;
            try
            {
                CultureInfo.CurrentCulture = new CultureInfo("de-DE");
                XmlNode item = _saveSingleItem(_createItem());
                Assert.AreEqual("0.1250", item.SelectSingleNode("Price")?.InnerText);
            }
            finally
            {
                CultureInfo.CurrentCulture = previous;
            }
        }


        [TestMethod]
        public void Save_WritesRetainSubConditionAndStockroom()
        {
            BrickStoreInventoryItem source = _createItem();
            source.Retain = true;
            source.SubCondition = SubConditions.Sealed;
            source.Stockroom = "B";
            source.Comments = "Visible to buyers";
            source.Bulk = 10;
            source.Sale = 20;

            XmlNode item = _saveSingleItem(source);

            Assert.IsNotNull(item.SelectSingleNode("Retain"));
            Assert.AreEqual("M", item.SelectSingleNode("SubCondition")?.InnerText);
            Assert.AreEqual("B", item.SelectSingleNode("Stockroom")?.InnerText);
            Assert.AreEqual("Visible to buyers", item.SelectSingleNode("Comments")?.InnerText);
            Assert.AreEqual("10", item.SelectSingleNode("Bulk")?.InnerText);
            Assert.AreEqual("20", item.SelectSingleNode("Sale")?.InnerText);
        }


        [TestMethod]
        public void Save_WritesOnlyConfiguredTieredPrices()
        {
            BrickStoreInventoryItem source = _createItem();
            source.TieredPrice = new TieredPrice()
            {
                TieredQuantity1 = 10,
                TieredPrice1 = 0.1m,
                TieredQuantity2 = 100,
                TieredPrice2 = 0.05m
            };

            XmlNode item = _saveSingleItem(source);

            Assert.AreEqual("10", item.SelectSingleNode("TQ1")?.InnerText);
            Assert.AreEqual("0.1000", item.SelectSingleNode("TP1")?.InnerText);
            Assert.AreEqual("100", item.SelectSingleNode("TQ2")?.InnerText);
            Assert.AreEqual("0.0500", item.SelectSingleNode("TP2")?.InnerText);
            Assert.IsNull(item.SelectSingleNode("TQ3"));
            Assert.IsNull(item.SelectSingleNode("TP3"));
        }


        [TestMethod]
        public void Save_WritesCommentAsXmlComment()
        {
            BrickStoreInventory inventory = new BrickStoreInventory() { Comment = "created by test" };

            XmlDocument doc = _save(inventory);

            XmlComment? comment = doc.ChildNodes.OfType<XmlComment>().FirstOrDefault();
            Assert.AreEqual("created by test", comment?.Value);
        }


        [TestMethod]
        public void Save_ResetsStreamToStartPosition()
        {
            using (MemoryStream ms = new MemoryStream())
            {
                ms.Write(new byte[] { 1, 2, 3 });
                new BrickStoreInventory().Save(ms);

                Assert.AreEqual(3, ms.Position);
                Assert.IsTrue(ms.Length > 3);
            }
        }


        [TestMethod]
        public async Task SaveAsync_ReadOnlyStreamThrows()
        {
            using (MemoryStream ms = new MemoryStream(new byte[16], writable: false))
            {
                await Assert.ThrowsExceptionAsync<IllegalStreamException>(() => new BrickStoreInventory().SaveAsync(ms));
            }
        }


        [TestMethod]
        public void Save_ToFileCreatesReadableFile()
        {
            string path = TestData.CreateTempFileName();
            try
            {
                BrickStoreInventory inventory = new BrickStoreInventory();
                inventory.Items.Add(_createItem());
                inventory.Save(path);

                XmlDocument doc = new XmlDocument();
                doc.Load(path);
                Assert.AreEqual(1, doc.SelectNodes("/BrickStoreXML/Inventory/Item")!.Count);
            }
            finally
            {
                TestData.DeleteFile(path);
            }
        }
    }
}
