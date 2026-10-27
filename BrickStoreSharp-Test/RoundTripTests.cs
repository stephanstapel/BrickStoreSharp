using System.Globalization;
using System.Xml;
using BrickStoreSharp;

namespace BrickStoreSharp_Test
{
    [TestClass]
    public class RoundTripTests
    {
        private static BrickStoreInventory _roundTrip(BrickStoreInventory inventory)
        {
            string path = TestData.CreateTempFileName();
            try
            {
                inventory.Save(path);
                return BrickStoreInventory.Load(path);
            }
            finally
            {
                TestData.DeleteFile(path);
            }
        } // !_roundTrip()


        private static BrickStoreInventoryItem _roundTrip(BrickStoreInventoryItem item)
        {
            BrickStoreInventory inventory = new BrickStoreInventory();
            inventory.Items.Add(item);
            return _roundTrip(inventory).Items.Single();
        } // !_roundTrip()


        [TestMethod]
        public void RoundTrip_PreservesBasicFields()
        {
            BrickStoreInventoryItem result = _roundTrip(new BrickStoreInventoryItem()
            {
                Id = "3001",
                ItemType = ItemTypes.Part,
                ColorId = 11,
                Name = "Brick 2 x 4",
                ColorName = "Black",
                Quantity = 42,
                Price = 0.125m,
                Condition = ConditionCodes.Used,
                Remarks = "Box 7",
                LotId = 123456,
                OwlId = "987",
                OwlLotId = 654
            });

            Assert.AreEqual("3001", result.Id);
            Assert.AreEqual(ItemTypes.Part, result.ItemType);
            Assert.AreEqual(11, result.ColorId);
            Assert.AreEqual("Brick 2 x 4", result.Name);
            Assert.AreEqual("Black", result.ColorName);
            Assert.AreEqual(42, result.Quantity);
            Assert.AreEqual(0.125m, result.Price);
            Assert.AreEqual(ConditionCodes.Used, result.Condition);
            Assert.AreEqual("Box 7", result.Remarks);
            Assert.AreEqual(123456, result.LotId);
            Assert.AreEqual("987", result.OwlId);
            Assert.AreEqual(654, result.OwlLotId);
        }


        [DataTestMethod]
        [DataRow(ItemTypes.Book)]
        [DataRow(ItemTypes.Catalog)]
        [DataRow(ItemTypes.Gear)]
        [DataRow(ItemTypes.Instruction)]
        [DataRow(ItemTypes.Minifigure)]
        [DataRow(ItemTypes.OriginalBox)]
        [DataRow(ItemTypes.Part)]
        [DataRow(ItemTypes.Set)]
        public void RoundTrip_PreservesItemType(ItemTypes itemType)
        {
            Assert.AreEqual(itemType, _roundTrip(new BrickStoreInventoryItem() { Id = "x", ItemType = itemType }).ItemType);
        }


        [DataTestMethod]
        [DataRow(StatusCodes.Include)]
        [DataRow(StatusCodes.Exclude)]
        [DataRow(StatusCodes.Extra)]
        public void RoundTrip_PreservesStatus(StatusCodes status)
        {
            Assert.AreEqual(status, _roundTrip(new BrickStoreInventoryItem() { Id = "x", Status = status }).Status);
        }


        [TestMethod]
        public void RoundTrip_PreservesLargePrice()
        {
            Assert.AreEqual(1234.5m, _roundTrip(new BrickStoreInventoryItem() { Id = "x", Price = 1234.5m }).Price);
        }


        [TestMethod]
        public void RoundTrip_PreservesSellerFields()
        {
            BrickStoreInventoryItem result = _roundTrip(new BrickStoreInventoryItem()
            {
                Id = "x",
                CategoryId = 5,
                CategoryName = "Brick",
                Comments = "Visible to buyers",
                Retain = true,
                Bulk = 10,
                Sale = 20,
                Stockroom = "B",
                SubCondition = SubConditions.Complete,
                TieredPrice = new TieredPrice() { TieredQuantity1 = 10, TieredPrice1 = 0.1m }
            });

            Assert.AreEqual(5, result.CategoryId);
            Assert.AreEqual("Brick", result.CategoryName);
            Assert.AreEqual("Visible to buyers", result.Comments);
            Assert.IsTrue(result.Retain);
            Assert.AreEqual(10, result.Bulk);
            Assert.AreEqual(20, result.Sale);
            Assert.AreEqual("B", result.Stockroom);
            Assert.AreEqual(SubConditions.Complete, result.SubCondition);
            Assert.AreEqual(10, result.TieredPrice?.TieredQuantity1);
            Assert.AreEqual(0.1m, result.TieredPrice?.TieredPrice1);
        }


        [TestMethod]
        public void RoundTrip_PreservesDefaults()
        {
            BrickStoreInventoryItem result = _roundTrip(new BrickStoreInventoryItem() { Id = "x", ItemType = ItemTypes.Part });

            Assert.AreEqual(1, result.Bulk);
            Assert.AreEqual(0, result.Sale);
            Assert.AreEqual(0, result.CategoryId);
            Assert.IsFalse(result.Retain);
            Assert.IsNull(result.LotId);
            Assert.IsNull(result.OwlLotId);
            Assert.IsNull(result.TieredPrice);
            Assert.AreEqual(SubConditions.Unknown, result.SubCondition);
        }


        [TestMethod]
        public void RoundTrip_PreservesInventoryHeader()
        {
            BrickStoreInventory result = _roundTrip(new BrickStoreInventory()
            {
                Currency = "EUR",
                BrickLinkChangelogId = 4711
            });

            Assert.AreEqual("EUR", result.Currency);
            Assert.AreEqual(4711, result.BrickLinkChangelogId);
        }


        [TestMethod]
        public void RoundTrip_PreservesItemCountAndOrder()
        {
            BrickStoreInventory inventory = new BrickStoreInventory();
            for (int i = 0; i < 50; i++)
            {
                inventory.Items.Add(new BrickStoreInventoryItem() { Id = $"item{i}", ItemType = ItemTypes.Part });
            }

            BrickStoreInventory result = _roundTrip(inventory);

            CollectionAssert.AreEqual(inventory.Items.Select(i => i.Id).ToList(), result.Items.Select(i => i.Id).ToList());
        }


        [TestMethod]
        public void RoundTrip_PreservesExtendedFields()
        {
            BrickStoreInventoryItem result = _roundTrip(new BrickStoreInventoryItem()
            {
                Id = "x",
                Reserved = "buyer42",
                Cost = 0.1m,
                TotalWeight = 23.4m,
                MarkerText = "check",
                MarkerColor = "#ff0000"
            });

            Assert.AreEqual("buyer42", result.Reserved);
            Assert.AreEqual(0.1m, result.Cost);
            Assert.AreEqual(23.4m, result.TotalWeight);
            Assert.AreEqual("check", result.MarkerText);
            Assert.AreEqual("#ff0000", result.MarkerColor);
        }


        [TestMethod]
        public void RoundTrip_BrickStoreFileKeepsAllElements()
        {
            string sourcePath = TestData.CreateTempFile(TestData.InventoryWithExtras);
            string targetPath = TestData.CreateTempFileName();
            try
            {
                BrickStoreInventory.Load(sourcePath).Save(targetPath);

                XmlDocument source = new XmlDocument();
                source.Load(sourcePath);
                XmlDocument target = new XmlDocument();
                target.Load(targetPath);

                // every element of the original file must still be there
                foreach (string xpath in new[] { "/BrickStoreXML/Inventory/Item", "/BrickStoreXML" })
                {
                    List<string> sourceNames = source.SelectSingleNode(xpath)!.ChildNodes.OfType<XmlElement>().Select(e => e.Name).ToList();
                    List<string> targetNames = target.SelectSingleNode(xpath)!.ChildNodes.OfType<XmlElement>().Select(e => e.Name).ToList();
                    CollectionAssert.IsSubsetOf(sourceNames, targetNames, xpath);
                }

                Assert.AreEqual("USD", target.SelectSingleNode("/BrickStoreXML/Inventory/@Currency")?.Value);
                Assert.AreEqual("3", target.SelectSingleNode("/BrickStoreXML/GuiState/ItemView/SortColumn")?.InnerText);
                Assert.AreEqual("0.2", target.SelectSingleNode("//DifferenceBaseValues/@Price")?.Value);
                Assert.AreEqual("8", target.SelectSingleNode("//OrigQty")?.InnerText);
            }
            finally
            {
                TestData.DeleteFile(sourcePath);
                TestData.DeleteFile(targetPath);
            }
        }


        [DataTestMethod]
        [DataRow(ConditionCodes.New)]
        [DataRow(ConditionCodes.Used)]
        public void RoundTrip_PreservesCondition(ConditionCodes condition)
        {
            Assert.AreEqual(condition, _roundTrip(new BrickStoreInventoryItem() { Id = "x", Condition = condition }).Condition);
        }


        [DataTestMethod]
        [DataRow(SubConditions.Complete)]
        [DataRow(SubConditions.Incomplete)]
        [DataRow(SubConditions.Sealed)]
        public void RoundTrip_PreservesSubCondition(SubConditions subCondition)
        {
            Assert.AreEqual(subCondition, _roundTrip(new BrickStoreInventoryItem() { Id = "x", SubCondition = subCondition }).SubCondition);
        }


        [DataTestMethod]
        [DataRow("A")]
        [DataRow("B")]
        [DataRow("C")]
        public void RoundTrip_PreservesStockroom(string stockroom)
        {
            Assert.AreEqual(stockroom, _roundTrip(new BrickStoreInventoryItem() { Id = "x", Stockroom = stockroom }).Stockroom);
        }


        [TestMethod]
        public void RoundTrip_PreservesAllTieredPrices()
        {
            TieredPrice result = _roundTrip(new BrickStoreInventoryItem()
            {
                Id = "x",
                TieredPrice = new TieredPrice()
                {
                    TieredQuantity1 = 10,
                    TieredPrice1 = 0.1m,
                    TieredQuantity2 = 100,
                    TieredPrice2 = 0.05m,
                    TieredQuantity3 = 1000,
                    TieredPrice3 = 1234.5m
                }
            }).TieredPrice!;

            Assert.AreEqual(10, result.TieredQuantity1);
            Assert.AreEqual(0.1m, result.TieredPrice1);
            Assert.AreEqual(100, result.TieredQuantity2);
            Assert.AreEqual(0.05m, result.TieredPrice2);
            Assert.AreEqual(1000, result.TieredQuantity3);
            Assert.AreEqual(1234.5m, result.TieredPrice3);
        }


        [TestMethod]
        public void RoundTrip_PreservesLargeCostAndWeight()
        {
            BrickStoreInventoryItem result = _roundTrip(new BrickStoreInventoryItem() { Id = "x", Cost = 1234.5m, TotalWeight = 12345.678m });

            Assert.AreEqual(1234.5m, result.Cost);
            Assert.AreEqual(12345.678m, result.TotalWeight);
        }


        [DataTestMethod]
        [DataRow("en-US")]
        [DataRow("de-DE")]
        [DataRow("fr-FR")]
        public void RoundTrip_IsCultureIndependent(string culture)
        {
            CultureInfo previous = CultureInfo.CurrentCulture;
            try
            {
                CultureInfo.CurrentCulture = new CultureInfo(culture);
                BrickStoreInventoryItem result = _roundTrip(new BrickStoreInventoryItem()
                {
                    Id = "x",
                    Price = 1234.5m,
                    Cost = 0.125m,
                    TotalWeight = 2.5m,
                    TieredPrice = new TieredPrice() { TieredQuantity1 = 10, TieredPrice1 = 1000.25m }
                });

                Assert.AreEqual(1234.5m, result.Price);
                Assert.AreEqual(0.125m, result.Cost);
                Assert.AreEqual(2.5m, result.TotalWeight);
                Assert.AreEqual(1000.25m, result.TieredPrice?.TieredPrice1);
            }
            finally
            {
                CultureInfo.CurrentCulture = previous;
            }
        }


        [TestMethod]
        public void RoundTrip_PreservesSpecialCharacters()
        {
            const string text = "Gr\u00fc\u00dfe <b> & \"quotes\" 'apostrophe' \u2192 \U0001F9F1";
            BrickStoreInventoryItem result = _roundTrip(new BrickStoreInventoryItem()
            {
                Id = "x",
                Name = text,
                ColorName = text,
                CategoryName = text,
                Comments = text,
                Remarks = text,
                Reserved = text,
                MarkerText = text,
                OwlId = text
            });

            Assert.AreEqual(text, result.Name);
            Assert.AreEqual(text, result.ColorName);
            Assert.AreEqual(text, result.CategoryName);
            Assert.AreEqual(text, result.Comments);
            Assert.AreEqual(text, result.Remarks);
            Assert.AreEqual(text, result.Reserved);
            Assert.AreEqual(text, result.MarkerText);
            Assert.AreEqual(text, result.OwlId);
        }


        [TestMethod]
        public void RoundTrip_UnsetValuesStayUnset()
        {
            BrickStoreInventoryItem result = _roundTrip(new BrickStoreInventoryItem() { Id = "x" });

            Assert.AreEqual(StatusCodes.Unknown, result.Status);
            Assert.AreEqual(ConditionCodes.Unknown, result.Condition);
            Assert.IsNull(result.Quantity);
            Assert.IsNull(result.Price);
            Assert.IsNull(result.CategoryName);
            Assert.IsNull(result.Comments);
            Assert.IsNull(result.Stockroom);
            Assert.IsNull(result.Reserved);
            Assert.IsNull(result.Cost);
            Assert.IsNull(result.TotalWeight);
            Assert.IsNull(result.MarkerText);
            Assert.IsNull(result.MarkerColor);
            Assert.AreEqual(0, result.AdditionalElements.Count);
        }


        [TestMethod]
        public void RoundTrip_UnsetColorBecomesNotApplicable()
        {
            // ColorID is required by BrickStoreXML.rnc, unset colors are written as 0 (BrickLink '(Not Applicable)')
            Assert.AreEqual(0, _roundTrip(new BrickStoreInventoryItem() { Id = "x" }).ColorId);
        }


        [TestMethod]
        public void RoundTrip_PreservesAdditionalElements()
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml("""
                <root>
                  <DifferenceBaseValues ItemID="3001" Qty="8" Price="0.2"/>
                  <OrigQty>8</OrigQty>
                  <GuiState Version="2"><ItemView><SortColumn>3</SortColumn></ItemView></GuiState>
                </root>
                """);
            XmlElement[] elements = doc.DocumentElement!.ChildNodes.OfType<XmlElement>().ToArray();

            BrickStoreInventory inventory = new BrickStoreInventory();
            inventory.AdditionalElements.Add(elements[2]);
            BrickStoreInventoryItem item = new BrickStoreInventoryItem() { Id = "x" };
            item.AdditionalElements.Add(elements[0]);
            item.AdditionalElements.Add(elements[1]);
            inventory.Items.Add(item);

            BrickStoreInventory result = _roundTrip(inventory);

            CollectionAssert.AreEqual(new[] { elements[0].OuterXml, elements[1].OuterXml }, result.Items.Single().AdditionalElements.Select(e => e.OuterXml).ToArray());
            Assert.AreEqual(elements[2].OuterXml, result.AdditionalElements.Single().OuterXml);
        }


        [TestMethod]
        public void RoundTrip_SecondSaveIsIdentical()
        {
            string sourcePath = TestData.CreateTempFile(TestData.InventoryWithExtras);
            string firstPath = TestData.CreateTempFileName();
            string secondPath = TestData.CreateTempFileName();
            try
            {
                BrickStoreInventory.Load(sourcePath).Save(firstPath);
                BrickStoreInventory.Load(firstPath).Save(secondPath);

                Assert.AreEqual(File.ReadAllText(firstPath), File.ReadAllText(secondPath));
            }
            finally
            {
                TestData.DeleteFile(sourcePath);
                TestData.DeleteFile(firstPath);
                TestData.DeleteFile(secondPath);
            }
        }
    }
}
