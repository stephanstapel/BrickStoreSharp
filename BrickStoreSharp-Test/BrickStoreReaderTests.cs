using System.Globalization;
using BrickStoreSharp;

namespace BrickStoreSharp_Test
{
    [TestClass]
    public class BrickStoreReaderTests
    {
        private string _path = "";


        [TestInitialize]
        public void Setup()
        {
            _path = TestData.CreateTempFile(TestData.SampleInventory);
        }


        [TestCleanup]
        public void Cleanup()
        {
            TestData.DeleteFile(_path);
        }


        [TestMethod]
        public void Load_ReadsInventoryHeader()
        {
            BrickStoreInventory inventory = BrickStoreInventory.Load(_path);

            Assert.AreEqual("EUR", inventory.Currency);
            Assert.AreEqual(4711, inventory.BrickLinkChangelogId);
            Assert.AreEqual(2, inventory.Items.Count);
        }


        [TestMethod]
        public void Load_ReadsAllItemFields()
        {
            BrickStoreInventoryItem item = BrickStoreInventory.Load(_path).Items[0];

            Assert.AreEqual("3001", item.Id);
            Assert.AreEqual(ItemTypes.Part, item.ItemType);
            Assert.AreEqual(11, item.ColorId);
            Assert.AreEqual("Brick 2 x 4", item.Name);
            Assert.AreEqual("Black", item.ColorName);
            Assert.AreEqual(StatusCodes.Include, item.Status);
            Assert.AreEqual(42, item.Quantity);
            Assert.AreEqual(0.125m, item.Price);
            Assert.AreEqual(ConditionCodes.New, item.Condition);
            Assert.AreEqual("Box 7", item.Remarks);
            Assert.AreEqual(123456, item.LotId);
            Assert.AreEqual("987", item.OwlId);
            Assert.AreEqual(654, item.OwlLotId);
        }


        [TestMethod]
        public void Load_ReadsCategory()
        {
            BrickStoreInventoryItem item = BrickStoreInventory.Load(_path).Items[0];

            Assert.AreEqual(5, item.CategoryId);
            Assert.AreEqual("Brick", item.CategoryName);
        }


        [TestMethod]
        public void Load_MissingOptionalFieldsAreNull()
        {
            BrickStoreInventoryItem item = BrickStoreInventory.Load(_path).Items[1];

            Assert.AreEqual(ItemTypes.Minifigure, item.ItemType); // lower case type id
            Assert.IsNull(item.ColorId);
            Assert.IsNull(item.LotId);
            Assert.AreEqual(StatusCodes.Exclude, item.Status);
            Assert.AreEqual(ConditionCodes.Used, item.Condition);
            Assert.AreEqual(1234.5m, item.Price);
            Assert.IsNull(item.CategoryName);
            Assert.IsNull(item.Comments);
            Assert.IsNull(item.Stockroom);
            Assert.IsNull(item.TieredPrice);
            Assert.AreEqual(SubConditions.Unknown, item.SubCondition);
            Assert.IsFalse(item.Retain);
            Assert.AreEqual(1, item.Bulk);
            Assert.AreEqual(0, item.Sale);
        }


        [TestMethod]
        public void Load_MissingInventoryHeaderIsNull()
        {
            string path = TestData.CreateTempFile("<BrickStoreXML><Inventory/></BrickStoreXML>");
            try
            {
                BrickStoreInventory inventory = BrickStoreInventory.Load(path);
                Assert.IsNull(inventory.Currency);
                Assert.IsNull(inventory.BrickLinkChangelogId);
            }
            finally
            {
                TestData.DeleteFile(path);
            }
        }


        [TestMethod]
        public void Load_AcceptsBrickStockRootElement()
        {
            // older files use <BrickStockXML> as root element, see BrickStoreXML.rnc
            string path = TestData.CreateTempFile("<BrickStockXML><Inventory><Item><ItemID>3001</ItemID></Item></Inventory></BrickStockXML>");
            try
            {
                Assert.AreEqual("3001", BrickStoreInventory.Load(path).Items.Single().Id);
            }
            finally
            {
                TestData.DeleteFile(path);
            }
        }


        [TestMethod]
        public void Load_DoesNotResolveExternalEntities()
        {
            string secretPath = TestData.CreateTempFile("secret content");
            string path = TestData.CreateTempFile($"""
                <?xml version="1.0"?>
                <!DOCTYPE BrickStoreXML [<!ENTITY secret SYSTEM "{new Uri(secretPath)}">]>
                <BrickStoreXML><Inventory><Item><ItemID>3001</ItemID><Remarks>&secret;</Remarks></Item></Inventory></BrickStoreXML>
                """);
            try
            {
                BrickStoreInventory? inventory = null;
                try
                {
                    inventory = BrickStoreInventory.Load(path);
                }
                catch (System.Xml.XmlException)
                {
                    // rejecting the undeclared entity is fine as well
                }

                Assert.IsFalse(inventory?.Items.Single().Remarks?.Contains("secret content") ?? false);
            }
            finally
            {
                TestData.DeleteFile(path);
                TestData.DeleteFile(secretPath);
            }
        }


        [DataTestMethod]
        [DataRow("B", ItemTypes.Book)]
        [DataRow("C", ItemTypes.Catalog)]
        [DataRow("G", ItemTypes.Gear)]
        [DataRow("I", ItemTypes.Instruction)]
        [DataRow("M", ItemTypes.Minifigure)]
        [DataRow("O", ItemTypes.OriginalBox)]
        [DataRow("P", ItemTypes.Part)]
        [DataRow("S", ItemTypes.Set)]
        [DataRow("Z", ItemTypes.Unknown)]
        [DataRow("", ItemTypes.Unknown)]
        public void Load_MapsItemTypes(string typeId, ItemTypes expected)
        {
            string path = TestData.CreateTempFile($"<BrickStoreXML><Inventory><Item><ItemTypeID>{typeId}</ItemTypeID></Item></Inventory></BrickStoreXML>");
            try
            {
                Assert.AreEqual(expected, BrickStoreInventory.Load(path).Items[0].ItemType);
            }
            finally
            {
                TestData.DeleteFile(path);
            }
        }


        [TestMethod]
        public void Load_ItemTypeMappingIsCultureIndependent()
        {
            // "I".ToLower() under tr-TR yields a dotless "ı"
            CultureInfo previous = CultureInfo.CurrentCulture;
            string path = TestData.CreateTempFile("<BrickStoreXML><Inventory><Item><ItemTypeID>I</ItemTypeID></Item></Inventory></BrickStoreXML>");
            try
            {
                CultureInfo.CurrentCulture = new CultureInfo("tr-TR");
                Assert.AreEqual(ItemTypes.Instruction, BrickStoreInventory.Load(path).Items[0].ItemType);
            }
            finally
            {
                CultureInfo.CurrentCulture = previous;
                TestData.DeleteFile(path);
            }
        }


        [TestMethod]
        public void Load_EmptyInventoryHasNoItems()
        {
            string path = TestData.CreateTempFile("<BrickStoreXML><Inventory/></BrickStoreXML>");
            try
            {
                Assert.AreEqual(0, BrickStoreInventory.Load(path).Items.Count);
            }
            finally
            {
                TestData.DeleteFile(path);
            }
        }


        [TestMethod]
        public async Task LoadAsync_MissingFileThrowsFileNotFound()
        {
            string path = TestData.CreateTempFileName();
            FileNotFoundException ex = await Assert.ThrowsExceptionAsync<FileNotFoundException>(() => BrickStoreInventory.LoadAsync(path));
            Assert.AreEqual(path, ex.FileName);
        }


        [TestMethod]
        public void Load_MissingFileThrowsFileNotFound()
        {
            Assert.ThrowsException<FileNotFoundException>(() => BrickStoreInventory.Load(TestData.CreateTempFileName()));
        }


        [TestMethod]
        public void Load_ReleasesFileHandle()
        {
            BrickStoreInventory.Load(_path);

            // throws IOException if the reader still holds the file open
            using (FileStream fs = new FileStream(_path, FileMode.Open, FileAccess.ReadWrite, FileShare.None))
            {
            }
        }


        [TestMethod]
        public void Load_FromStream()
        {
            using (FileStream fs = File.OpenRead(_path))
            {
                BrickStoreInventory inventory = BrickStoreInventory.Load(fs);
                Assert.AreEqual(2, inventory.Items.Count);
            }
        }
    }
}
