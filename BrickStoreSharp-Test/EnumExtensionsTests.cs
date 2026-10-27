using BrickStoreSharp;

namespace BrickStoreSharp_Test
{
    [TestClass]
    public class EnumExtensionsTests
    {
        [DataTestMethod]
        [DataRow("N", ConditionCodes.New)]
        [DataRow("New", ConditionCodes.New)]
        [DataRow("U", ConditionCodes.Used)]
        [DataRow("Used", ConditionCodes.Used)]
        [DataRow("", ConditionCodes.Unknown)]
        [DataRow("X", ConditionCodes.Unknown)]
        public void ConditionCodes_FromString(string input, ConditionCodes expected)
        {
            Assert.AreEqual(expected, default(ConditionCodes).FromString(input));
        }


        [DataTestMethod]
        [DataRow("I", StatusCodes.Include)]
        [DataRow("Include", StatusCodes.Include)]
        [DataRow("Exclude", StatusCodes.Exclude)]
        [DataRow("Extra", StatusCodes.Extra)]
        [DataRow("X", StatusCodes.Exclude)]
        [DataRow("E", StatusCodes.Extra)]
        [DataRow("", StatusCodes.Unknown)]
        [DataRow("Q", StatusCodes.Unknown)]
        public void StatusCodes_FromString(string input, StatusCodes expected)
        {
            Assert.AreEqual(expected, default(StatusCodes).FromString(input));
        }


        [TestMethod]
        public void EnumToString_ReturnsEnumName()
        {
            // public API, must stay compatible - the writer uses the internal BrickStore codes instead
            Assert.AreEqual("Include", StatusCodes.Include.EnumToString());
            Assert.AreEqual("New", ConditionCodes.New.EnumToString());
        }


        [TestMethod]
        public void StatusCodes_EnumToStringCanBeParsedBack()
        {
            foreach (StatusCodes status in new[] { StatusCodes.Include, StatusCodes.Exclude, StatusCodes.Extra })
            {
                Assert.AreEqual(status, default(StatusCodes).FromString(status.EnumToString()), $"Status {status}");
            }
        }


        [TestMethod]
        public void ConditionCodes_EnumToStringCanBeParsedBack()
        {
            foreach (ConditionCodes condition in new[] { ConditionCodes.New, ConditionCodes.Used })
            {
                Assert.AreEqual(condition, default(ConditionCodes).FromString(condition.EnumToString()), $"Condition {condition}");
            }
        }
    }
}
