﻿/*
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
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.XPath;

namespace BrickStoreSharp
{
    public class XmlUtils
    {
        /// <summary>
        ///  reads a certain attribute value
        /// </summary>
        public static string AttributeText(XmlNode node, string attributeName, string defaultText = "")
        {
            try
            {
                XmlAttribute attrib = node.Attributes[attributeName];
                if (attrib != null)
                {
                    return attrib.Value;
                }
            }
            catch
            {
            }

            return defaultText;
        } // !AttributeText()


        public static bool NodeAsBool(XmlNode node, string xpath, XmlNamespaceManager nsmgr = null, bool defaultValue = true)
        {
            if (node == null)
            {
                return defaultValue;
            }

            string value = NodeAsString(node, xpath, nsmgr);
            if (value == "")
            {
                return defaultValue;
            }
            else
            {
                if ((value.Trim().ToLower() == "true") || (value.Trim() == "1"))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        } // !NodeAsBool()


        public static string NodeAsString(XmlNode node, string xpath, XmlNamespaceManager nsmgr = null, string defaultValue = "")
        {
            if (node == null)
            {
                return defaultValue;
            }            

            try
            {
                XmlNode _node = node.SelectSingleNode(xpath, nsmgr);
                if (_node == null)
                {
                    return defaultValue;
                }
                else
                {
                    return _node.InnerText;
                }
            }
            catch (XPathException)
            {
                return defaultValue;
            }
            catch (Exception ex)
            {
                throw ex;
            };
        } // NodeAsString()


        public static List<string> NodesAsStrings(XmlNode node, string xpath, XmlNamespaceManager nsmgr = null, List<string> defaultValue = null)
        {
            if (node == null)
            {
                return defaultValue;
            }

            try
            {
                List<string> retval = new List<string>();
                foreach(XmlNode _node in node.SelectNodes(xpath, nsmgr))
                {
                    retval.Add(_node.InnerText);
                }
                return retval;
            }
            catch (XPathException)
            {
                return defaultValue;
            }
            catch (Exception ex)
            {
                throw ex;
            };

            return defaultValue;
        } // NodesAsStrings()


        public static int? NodeAsInt(XmlNode node, string xpath, XmlNamespaceManager nsmgr = null, int? defaultValue = null)
        {
            if (node == null)
            {
                return defaultValue;
            }

            string temp = NodeAsString(node, xpath, nsmgr, "");
            int retval;
            if (Int32.TryParse(temp, out retval))
            {
                return retval;
            }
            else
            {
                return defaultValue;
            }
        } // !NodeAsInt()


        public static decimal? NodeAsDecimal(XmlNode node, string xpath, XmlNamespaceManager nsmgr = null, decimal? defaultValue = null)
        {
            if (node == null)
            {
                return defaultValue;
            }

            string temp = NodeAsString(node, xpath, nsmgr, "");
            decimal retval;
            if (decimal.TryParse(temp, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out retval))
            {
                return retval;
            }
            else
            {
                return defaultValue;
            }
        } // !NodeAsDecimal()


        public static DateTime? NodeAsDateTime(XmlNode node, string xpath, XmlNamespaceManager nsmgr = null, DateTime? defaultValue = null)
        {
            if (node == null)
            {
                return defaultValue;
            }

            string temp = NodeAsString(node, xpath, nsmgr, "");
            DateTime retval;
            if (DateTime.TryParse(temp, out retval))
            {
                return retval;
            }
            else
            {
                string format = "yyyy-MM-ddThh:mm:sszzz";
                CultureInfo provider = CultureInfo.InvariantCulture;
                if (DateTime.TryParseExact(temp, format, provider, DateTimeStyles.None, out retval))
                {
                    return retval;
                }

                format = "yyyyMMdd";                
                if (DateTime.TryParseExact(temp, format, provider, DateTimeStyles.None, out retval))
                {
                    return retval;
                }

                format = "yyyy-MM-dd";
                if (DateTime.TryParseExact(temp, format, provider, DateTimeStyles.None, out retval))
                {
                    return retval;
                }

                return defaultValue;
            }
        } // !NodeAsDateTime()



        public static XmlNodeList SelectNodes(XmlNode node, string xpath, XmlNamespaceManager nsmgr = null)
        {
            return node.SelectNodes(xpath, nsmgr);
        } // !SelectNodes()


        public static XmlNode SelectSingleNode(XmlNode node, string xpath, XmlNamespaceManager nsmgr = null)
        {
            return node.SelectSingleNode(xpath, nsmgr);
        } // !SelectNodes()
    }
}
