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
    public static class ConditionCodesExtensions
    {
        /// <summary>
        /// Parses the BrickStore code (N, U) or the enum name (New, Used)
        /// </summary>
        public static ConditionCodes FromString(this ConditionCodes _c, string s)
        {
            switch (s)
            {
                case "New":
                case "N": return ConditionCodes.New;
                case "Used":
                case "U": return ConditionCodes.Used;
                default: return ConditionCodes.Unknown;
            }
        } // !FromString()


        public static string EnumToString(this ConditionCodes c)
        {
            return c.ToString("g");
        } // !ToString()


        /// <summary>
        /// Returns the code BrickStore uses in its files (N, U), empty string for Unknown
        /// </summary>
        internal static string ToCode(this ConditionCodes c)
        {
            switch (c)
            {
                case ConditionCodes.New: return "N";
                case ConditionCodes.Used: return "U";
                default: return String.Empty;
            }
        } // !ToCode()
    }
}
