namespace BrickStoreSharp_Test
{
    /// <summary>
    /// Shared sample data and temp file handling for the tests
    /// </summary>
    internal static class TestData
    {
        public const string SampleInventory = """
            <?xml version="1.0" encoding="UTF-8"?>
            <!DOCTYPE BrickStoreXML>
            <BrickStoreXML>
              <Inventory Currency="EUR" BrickLinkChangelogId="4711">
                <Item>
                  <ItemID>3001</ItemID>
                  <ItemTypeID>P</ItemTypeID>
                  <ColorID>11</ColorID>
                  <ItemName>Brick 2 x 4</ItemName>
                  <ItemTypeName>Part</ItemTypeName>
                  <ColorName>Black</ColorName>
                  <CategoryID>5</CategoryID>
                  <CategoryName>Brick</CategoryName>
                  <Status>I</Status>
                  <Qty>42</Qty>
                  <Price>0.125</Price>
                  <Condition>N</Condition>
                  <Remarks>Box 7</Remarks>
                  <LotID>123456</LotID>
                  <OwlID>987</OwlID>
                  <OwlLotID>654</OwlLotID>
                </Item>
                <Item>
                  <ItemID>sw0001</ItemID>
                  <ItemTypeID>m</ItemTypeID>
                  <ItemName>Battle Droid</ItemName>
                  <Status>X</Status>
                  <Qty>1</Qty>
                  <Price>1234.5</Price>
                  <Condition>U</Condition>
                </Item>
              </Inventory>
            </BrickStoreXML>
            """;


        /// <summary>
        /// Inventory using elements this library does not interpret (DifferenceBaseValues, OrigQty, GuiState)
        /// </summary>
        public const string InventoryWithExtras = """
            <?xml version="1.0" encoding="UTF-8"?>
            <BrickStoreXML>
              <Inventory Currency="USD">
                <Item>
                  <ItemID>3001</ItemID>
                  <ItemTypeID>P</ItemTypeID>
                  <ColorID>5</ColorID>
                  <Qty>10</Qty>
                  <Price>0.25</Price>
                  <Condition>N</Condition>
                  <Reserved>buyer42</Reserved>
                  <Cost>0.1</Cost>
                  <TotalWeight>23.4</TotalWeight>
                  <MarkerText>check</MarkerText>
                  <MarkerColor>#ff0000</MarkerColor>
                  <OrigQty>8</OrigQty>
                  <DifferenceBaseValues ItemID="3001" ItemTypeID="P" ColorID="5" Qty="8" Price="0.2"/>
                </Item>
              </Inventory>
              <GuiState Application="BrickStore" Version="2"><ItemView><SortColumn>3</SortColumn></ItemView></GuiState>
            </BrickStoreXML>
            """;


        public static string CreateTempFile(string content)
        {
            string path = Path.Combine(Path.GetTempPath(), $"BrickStoreSharp-Test-{Guid.NewGuid():N}.bsx");
            File.WriteAllText(path, content);
            return path;
        } // !CreateTempFile()


        public static string CreateTempFileName()
        {
            return Path.Combine(Path.GetTempPath(), $"BrickStoreSharp-Test-{Guid.NewGuid():N}.bsx");
        } // !CreateTempFileName()


        public static void DeleteFile(string path)
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        } // !DeleteFile()
    }
}
