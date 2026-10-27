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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace BrickStoreSharp
{
    public class BrickStoreInventory
    {
        public string Comment { get; set; }
        public List<BrickStoreInventoryItem> Items { get; set; } = new List<BrickStoreInventoryItem>();
        public string Currency { get; set; }
        public int? BrickLinkChangelogId { get; set; }

        /// <summary>
        /// Elements next to the inventory this library does not interpret (e.g. BrickStore's GuiState).
        /// They are read and written unchanged so no data gets lost on save.
        /// </summary>
        public List<XmlElement> AdditionalElements { get; set; } = new List<XmlElement>();



        public async static Task<BrickStoreInventory> LoadAsync(Stream stream)
        {
            return await BrickStoreReader.LoadAsync(stream);
        } // !Load()


        public async static Task<BrickStoreInventory> LoadAsync(string filename)
        {
            return await BrickStoreReader.LoadAsync(filename);
        } // !Load()


        public static BrickStoreInventory Load(Stream stream)
        {
            return LoadAsync(stream).GetAwaiter().GetResult();
        } // !Load()


        public static BrickStoreInventory Load(string filename)
        {
            return LoadAsync(filename).GetAwaiter().GetResult();
        } // !Load()


        public async Task SaveAsync(Stream stream)
        {
            BrickStoreWriter writer = new BrickStoreWriter();
            await writer.SaveAsync(this, stream);
        } // !Save()


        public async Task SaveAsync(string filename)
        {
            BrickStoreWriter writer = new BrickStoreWriter();
            await writer.SaveAsync(this, filename);
        } // !Save()


        public void Save(Stream stream)
        {
            SaveAsync(stream).GetAwaiter().GetResult();
        } // !Save()


        public void Save(string filename)
        {
            SaveAsync(filename).GetAwaiter().GetResult();
        } // !Save()
    }
}
