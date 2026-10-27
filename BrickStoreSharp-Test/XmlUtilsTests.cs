using System.Globalization;
using System.Xml;
using BrickStoreSharp;

namespace BrickStoreSharp_Test
{
    [TestClass]
    public class XmlUtilsTests
    {
        private static XmlNode _createNode(string xml)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml);
            return doc.DocumentElement!;
        } // !_createNode()


        [TestMethod]
        public void NodeAsString_ReturnsInnerText()
        {
            XmlNode node = _createNode("<root><a>hello</a></root>");
            Assert.AreEqual("hello", XmlUtils.NodeAsString(node, "./a"));
        }


        [TestMethod]
        public void NodeAsString_MissingNodeReturnsDefault()
        {
            XmlNode node = _createNode("<root/>");
            Assert.AreEqual("", XmlUtils.NodeAsString(node, "./a"));
            Assert.AreEqual("fallback", XmlUtils.NodeAsString(node, "./a", defaultValue: "fallback"));
        }


        [TestMethod]
        public void NodeAsString_InvalidXPathReturnsDefault()
        {
            XmlNode node = _createNode("<root/>");
            Assert.AreEqual("fallback", XmlUtils.NodeAsString(node, "///[", defaultValue: "fallback"));
        }


        [TestMethod]
        public void NodeAsString_NullNodeReturnsDefault()
        {
            Assert.AreEqual("fallback", XmlUtils.NodeAsString(null, "./a", defaultValue: "fallback"));
        }


        [TestMethod]
        public void NodeAsInt_ParsesValueOrReturnsDefault()
        {
            XmlNode node = _createNode("<root><a>42</a><b>abc</b></root>");
            Assert.AreEqual(42, XmlUtils.NodeAsInt(node, "./a"));
            Assert.IsNull(XmlUtils.NodeAsInt(node, "./b"));
            Assert.IsNull(XmlUtils.NodeAsInt(node, "./missing"));
            Assert.AreEqual(7, XmlUtils.NodeAsInt(node, "./missing", defaultValue: 7));
        }


        [TestMethod]
        public void NodeAsDecimal_UsesInvariantCulture()
        {
            CultureInfo previous = CultureInfo.CurrentCulture;
            try
            {
                CultureInfo.CurrentCulture = new CultureInfo("de-DE");
                XmlNode node = _createNode("<root><a>12.345</a></root>");
                Assert.AreEqual(12.345m, XmlUtils.NodeAsDecimal(node, "./a"));
            }
            finally
            {
                CultureInfo.CurrentCulture = previous;
            }
        }


        [TestMethod]
        public void NodeAsDecimal_InvalidValueReturnsDefault()
        {
            XmlNode node = _createNode("<root><a>abc</a></root>");
            Assert.IsNull(XmlUtils.NodeAsDecimal(node, "./a"));
        }


        [TestMethod]
        public void NodeAsBool_ParsesTrueAndOne()
        {
            XmlNode node = _createNode("<root><a>true</a><b> 1 </b><c>TRUE</c><d>no</d></root>");
            Assert.IsTrue(XmlUtils.NodeAsBool(node, "./a"));
            Assert.IsTrue(XmlUtils.NodeAsBool(node, "./b"));
            Assert.IsTrue(XmlUtils.NodeAsBool(node, "./c"));
            Assert.IsFalse(XmlUtils.NodeAsBool(node, "./d"));
        }


        [TestMethod]
        public void NodeAsBool_MissingNodeReturnsDefault()
        {
            XmlNode node = _createNode("<root/>");
            Assert.IsTrue(XmlUtils.NodeAsBool(node, "./a"));
            Assert.IsFalse(XmlUtils.NodeAsBool(node, "./a", defaultValue: false));
        }


        [TestMethod]
        public void NodeAsDateTime_ParsesCompactFormat()
        {
            XmlNode node = _createNode("<root><a>20240315</a></root>");
            Assert.AreEqual(new DateTime(2024, 3, 15), XmlUtils.NodeAsDateTime(node, "./a"));
        }


        [TestMethod]
        public void NodeAsDateTime_InvalidValueReturnsDefault()
        {
            XmlNode node = _createNode("<root><a>not a date</a></root>");
            Assert.IsNull(XmlUtils.NodeAsDateTime(node, "./a"));
        }


        [TestMethod]
        public void NodesAsStrings_ReturnsAllMatches()
        {
            XmlNode node = _createNode("<root><a>1</a><a>2</a><a>3</a></root>");
            CollectionAssert.AreEqual(new List<string> { "1", "2", "3" }, XmlUtils.NodesAsStrings(node, "./a"));
        }


        [TestMethod]
        public void AttributeText_ReturnsValueOrDefault()
        {
            XmlNode node = _createNode("<root currency=\"EUR\"/>");
            Assert.AreEqual("EUR", XmlUtils.AttributeText(node, "currency"));
            Assert.AreEqual("n/a", XmlUtils.AttributeText(node, "missing", "n/a"));
        }
    }
}
