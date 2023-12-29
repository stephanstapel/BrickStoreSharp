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
using System.Xml.Serialization;

namespace BrickStoreSharp
{
    public class BrickStoreInventory
    {
        public List<BrickStoreInventoryItem> Items { get; set; } = new List<BrickStoreInventoryItem>();
        public string Currency { get; set; }
        public int? BrickLinkChangelogId { get; set; }



        public async static Task<BrickStoreInventory> LoadAsync(Stream stream)
        {
            return await BrickStoreInventory.LoadAsync(stream);
        } // !Load()


        public async static Task<BrickStoreInventory> LoadAsync(string filename)
        {
            return await BrickStoreReader.LoadAsync(filename);
        } // !Load()


        public static BrickStoreInventory Load(Stream stream)
        {
            Task<BrickStoreInventory> t = LoadAsync(stream);
            t.Wait();
            return t.Result;
        } // !Load()


        public static BrickStoreInventory Load(string filename)
        {
            Task<BrickStoreInventory> t = LoadAsync(filename);
            t.Wait();
            return t.Result;
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
            Task t = SaveAsync(stream);
            t.Wait();
        } // !Save()


        public void Save(string filename)
        {
            Task t = SaveAsync(filename);
            t.Wait();
        } // !Save()
    }
}
