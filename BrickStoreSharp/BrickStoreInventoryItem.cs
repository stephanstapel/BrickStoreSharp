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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace BrickStoreSharp
{
    public class BrickStoreInventoryItem
    {
        public string Id { get; set; }
        public string ItemTypeId { get; set; }
        public int? ColorId { get; set; }
        public string Name { get; set; }
        public string ItemType { get; set; }
        public string ColorName { get; set; }
        public StatusCodes Status { get; set; }
        public int? Quantity { get; set; }
        public decimal? Price { get; set; }
        public ConditionCodes Condition { get; set; }
        public string Remarks { get; set; }
        public int? LotId { get; set; }
        public int? OwlId { get; set; }
        public int? OwlLotId { get; set; }
}
}
