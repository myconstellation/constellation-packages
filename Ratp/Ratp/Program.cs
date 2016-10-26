/*
 *	 RATP Package for Constellation
 *	 Web site: http://www.myConstellation.io
 *	 Copyright (C) 2016 - Hydro
 *	 Copyright (C) 2016 - Pierre Grimaud <https://github.com/pgrimaud>
 *	 Copyright (C) 2016 - Sebastien Warin <http://sebastien.warin.fr>	   	  
 *	
 *	 Licensed to Constellation under one or more contributor
 *	 license agreements. Constellation licenses this file to you under
 *	 the Apache License, Version 2.0 (the "License"); you may
 *	 not use this file except in compliance with the License.
 *	 You may obtain a copy of the License at
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

namespace Ratp
{
    using Constellation.Package;
    using Newtonsoft.Json;
    using Ratp.Models;
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Text;

    /// <summary>
    /// RATP Package for Constellation
    /// </summary>
    /// <seealso cref="Constellation.Package.PackageBase" />
    public class Program : PackageBase
    {
        private const string ROOT_URI = "https://api-ratp.pierre-grimaud.fr/v2/";

        static void Main(string[] args)
        {
            PackageHost.Start<Program>(args);
        }

        /// <summary>
        /// Récupére le trafic du réseau ferré RATP.
        /// </summary>
        /// <param name="type">Le type de transport dont vous souhaitez avoir les informations.</param>
        /// <param name="ligneId">Le nom de la ligne du type de transport spécifié.</param>
        /// <returns></returns>
        [MessageCallback]
        Response<TrafficResult> GetTraffic(LineType type, int ligneId)
        {
            return this.GetResponse<TrafficResult>($"traffic/{type}/{ligneId}");
        }

        /// <summary>
        /// Récupére les stations d'une ligne de Rer, Métro, Tramway, Bus ou Noctilien.
        /// </summary>
        /// <param name="type">Le type de transport dont vous souhaitez avoir les informations..</param>
        /// <param name="ligneId">Le nom de la ligne du type de transport spécifié.</param>
        /// <returns></returns>
        [MessageCallback]
        Response<StationList> GetStations(LineType type, int ligneId)
        {
            return this.GetResponse<StationList>($"{type}/{ligneId}/stations");
        }

        /// <summary>
        /// Récupére les temps d'attente des prochains trains d'une ligne de Rer, Métro, Tramway, Bus ou Noctilien en fonction de la destination et de la station.
        /// </summary>
        /// <param name="type">Le type de transport dont vous souhaitez avoir les informations..</param>
        /// <param name="ligneId">Le nom de la ligne du type de transport spécifié.</param>
        /// <param name="stationId">L'id ou l'indicatif de la station désirée.</param>
        /// <param name="destinationId">L'id ou l'indicatif de la destination désirée.</param>
        /// <returns></returns>
        [MessageCallback]
        Response<ScheduleResult> GetSchedule(LineType type, int ligneId, string stationId, string destinationId)
        {
            return this.GetResponse<ScheduleResult>($"{type}/{ligneId}/stations/{stationId}?destination={destinationId}");
        }

        private Response<TResult> GetResponse<TResult>(string path)
        {
            try
            {
                string AddressURL = ROOT_URI + path;
                using (WebClient webClient = new WebClient() { Encoding = Encoding.UTF8 })
                {
                    var result = webClient.DownloadString(AddressURL);
                    return JsonConvert.DeserializeObject<Response<TResult>>(result, new JsonSerializerSettings()
                    {
                        Converters = new List<JsonConverter>()
                    {
                        new PropertyNamesMatchingConverter()
                    }
                    });
                }
            }
            catch (Exception ex)
            {
                PackageHost.WriteError($"Error on getting {path} : {ex.ToString()}");
                return new Response<TResult>()
                {
                    Error = ex.Message,
                    HasError = true,
                    Metadatas = new Metadata() { ResponseDate = DateTime.Now.ToString(), RequestURI = path }
                };
            }
        }
    }
}