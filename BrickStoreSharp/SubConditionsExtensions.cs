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
    internal static class SubConditionsExtensions
    {
        /// <summary>
        /// Parses the BrickStore sub condition code (C, I, M), case insensitive
        /// </summary>
        internal static SubConditions FromCode(this SubConditions _s, string s)
        {
            switch ((s ?? String.Empty).ToUpperInvariant())
            {
                case "C": return SubConditions.Complete;
                case "I": return SubConditions.Incomplete;
                case "M": return SubConditions.Sealed;
                default: return SubConditions.Unknown;
            }
        } // !FromCode()


        internal static string ToCode(this SubConditions s)
        {
            switch (s)
            {
                case SubConditions.Complete: return "C";
                case SubConditions.Incomplete: return "I";
                case SubConditions.Sealed: return "M";
                default: return String.Empty;
            }
        } // !ToCode()
    }
}
