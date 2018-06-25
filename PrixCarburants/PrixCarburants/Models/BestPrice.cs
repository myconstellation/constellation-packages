/*
*	 Copyright 2018 Aurélien Chevalier	   	  
*	
*	 Licensed under the Apache License, Version 2.0 (the "License");
*   you may not use this file except in compliance with the License.
*   You may obtain a copy of the License at
*	
*	 http://www.apache.org/licenses/LICENSE-2.0
*	
*	 Unless required by applicable law or agreed to in writing,
*	 software distributed under the License is distributed on an
*	 "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
*	 KIND, either express or implied. See the License for the
*	 specific language governing permissions and limitations
*	 under the License.
*/

namespace PrixCarburants.Models
{
    /// <summary>
    /// Associate a gas station and a fuel price.
    /// </summary>
    public class BestPrice
    {
        /// <summary>
        /// Station latitude
        /// </summary>
        public double Lat;
        /// <summary>
        /// Station longitude
        /// </summary>
        public double Lon;
        /// <summary>
        /// Fuel type
        /// </summary>
        public string Carburant;
        /// <summary>
        /// Fuel price
        /// </summary>
        public double Price;

        /// <inheritdoc />
        public BestPrice(double lat, double lon, string carburant, double price)
        {
            Lat = lat;
            Lon = lon;
            Carburant = carburant;
            Price = price;
        }
    }
}
