/*
 *	 S-Sound - Multi-room audio system for Constellation
 *	 Web site: http://sebastien.warin.fr
 *	 Copyright (C) 2014-2018 - Sebastien Warin <http://sebastien.warin.fr>	   	  
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

namespace SSound.Core.Utils
{
    using Constellation.Package;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net;

    /// <summary>
    /// MP3 playlist (M3U) reader
    /// </summary>
    internal static class M3UReader
    {
        /// <summary>
        /// Reads the M3U playlist from the specified URI.
        /// </summary>
        /// <param name="uri">The M3U URI.</param>
        /// <returns>Track's list</returns>
        public static List<string> Read(string uri)
        {
            var playlist = new List<string>();
            try
            {
                if (uri.StartsWith("http", StringComparison.InvariantCultureIgnoreCase))
                {
                    // Download the M3U and read all lines
                    playlist.AddRange(new WebClient().DownloadString(uri).Split('\n'));
                }
                else if (File.Exists(uri))
                {
                    // Open file and read all lines
                    playlist.AddRange(File.ReadAllLines(uri));
                }
                // Remove comments
                playlist = playlist.Where(u => !u.StartsWith("#")).ToList();
            }
            catch (Exception ex)
            {
                PackageHost.WriteError("Can't read the playlist '{0}' : {1}", uri, ex.ToString());
            }

            return playlist;
        }
    }
}
