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

namespace BrickStoreSharp
{
    internal static class ItemTypesExtensions
    {
        /// <summary>
        /// Parses the BrickStore item type id (B, C, G, I, M, O, P, S), case insensitive
        /// </summary>
        internal static ItemTypes FromCode(this ItemTypes _t, string s)
        {
            switch ((s ?? String.Empty).ToUpperInvariant())
            {
                case "B": return ItemTypes.Book;
                case "C": return ItemTypes.Catalog;
                case "G": return ItemTypes.Gear;
                case "I": return ItemTypes.Instruction;
                case "M": return ItemTypes.Minifigure;
                case "O": return ItemTypes.OriginalBox;
                case "P": return ItemTypes.Part;
                case "S": return ItemTypes.Set;
                default: return ItemTypes.Unknown;
            }
        } // !FromCode()


        internal static string ToCode(this ItemTypes t)
        {
            switch (t)
            {
                case ItemTypes.Book: return "B";
                case ItemTypes.Catalog: return "C";
                case ItemTypes.Gear: return "G";
                case ItemTypes.Instruction: return "I";
                case ItemTypes.Minifigure: return "M";
                case ItemTypes.OriginalBox: return "O";
                case ItemTypes.Part: return "P";
                case ItemTypes.Set: return "S";
                default: return String.Empty;
            }
        } // !ToCode()


        internal static string ToName(this ItemTypes t)
        {
            switch (t)
            {
                case ItemTypes.Book: return "Book";
                case ItemTypes.Catalog: return "Catalog";
                case ItemTypes.Gear: return "Gear";
                case ItemTypes.Instruction: return "Instruction";
                case ItemTypes.Minifigure: return "Minifig";
                case ItemTypes.OriginalBox: return "OriginalBox";
                case ItemTypes.Part: return "Part";
                case ItemTypes.Set: return "Set";
                default: return String.Empty;
            }
        } // !ToName()
    }
}
