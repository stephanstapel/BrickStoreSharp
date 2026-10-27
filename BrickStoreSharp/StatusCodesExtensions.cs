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
    public static class StatusCodesExtensions
    {
        /// <summary>
        /// Parses the BrickStore code (I, X, E) or the enum name (Include, Exclude, Extra)
        /// </summary>
        public static StatusCodes FromString(this StatusCodes _c, string s)
        {
            switch (s)
            {
                case "Include":
                case "I": return StatusCodes.Include;
                case "Exclude":
                case "X": return StatusCodes.Exclude;
                case "Extra":
                case "E": return StatusCodes.Extra;
                default: return StatusCodes.Unknown;
            }
        } // !FromString()


        public static string EnumToString(this StatusCodes c)
        {
            return c.ToString("g");
        } // !ToString()


        /// <summary>
        /// Returns the code BrickStore uses in its files (I, X, E), empty string for Unknown
        /// </summary>
        internal static string ToCode(this StatusCodes c)
        {
            switch (c)
            {
                case StatusCodes.Include: return "I";
                case StatusCodes.Exclude: return "X";
                case StatusCodes.Extra: return "E";
                default: return String.Empty;
            }
        } // !ToCode()
    }
}
