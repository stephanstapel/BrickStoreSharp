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
        /// <summary>
        /// Item's identification number
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 	The type of the item 
        /// </summary>
        public ItemTypes ItemType { get; set; }

        /// <summary>
        /// The ID of the color of the item 
        /// </summary>
        public int? ColorId { get; set; }

        /// <summary>
        /// The name of this item 
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Color name of the item 
        /// </summary>
        public string ColorName { get; set; }

        /// <summary>
        /// Id of the main category of the item 
        /// </summary>
        public int CategoryId { get; set; }

        /// <summary>
        /// Name of the main category of the item 
        /// </summary>
        public string CategoryName { get; set; }
        public StatusCodes Status { get; set; }

        /// <summary>
        /// The number of items included in this inventory
        /// </summary>
        public int? Quantity { get; set; }

        /// <summary>
        /// The original price of this item per sale unit 
        /// </summary>
        public decimal? Price { get; set; }

        /// <summary>
        /// Indicates whether the item is new or used
        /// </summary>
        public ConditionCodes Condition { get; set; }

        /// <summary>
        /// User remarks on this inventory, visible only to the user
        /// </summary>
        public string Remarks { get; set; }

        /// <summary>
        /// The ID of the inventory (primarily used for Bricklink
        /// </summary>
        public int? LotId { get; set; }

        /// <summary>
        /// The ID item at Brick Owl
        /// </summary>
        public string OwlId { get; set; }

        /// <summary>
        /// The ID of the inventory (primarily at Brick Owl
        /// </summary>
        public int? OwlLotId { get; set; }

        /// <summary>
        /// Indicates the stockroom that the item to be placed when the user uses multiple stockroom
        /// </summary>
        public string Stockroom { get; set; }

        /// <summary>
        /// Special structure for holding discounts when purchasing large quantities of elements
        /// </summary>
        public TieredPrice TieredPrice { get; set; }

        /// <summary>
        /// 	A short description for this inventorym visible to every customer
        /// </summary>
        public string Comments { get; set; }

        /// <summary>
        /// Indicates whether the item retains in inventory after it is sold out 
        /// </summary>
        public bool Retain { get; set; } = false;

        /// <summary>
        /// Buyers can buy this item only in multiples of the bulk amount
        /// </summary>
        public int Bulk { get; set; } = 1;

        /// <summary>
        /// Sale discount in % (20% => 20)
        /// </summary>
        public int Sale { get; set; } = 0;

        public SubConditions SubCondition { get; set; } = SubConditions.Unknown;
    }
}
