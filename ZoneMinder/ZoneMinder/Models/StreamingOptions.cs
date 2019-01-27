/*
 *	 ZoneMinder package for Constellation
 *	 Web site: http://www.myConstellation.io
 *	 Copyright (C) 2016-2019 - Sebastien Warin <http://sebastien.warin.fr>	   	  
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

namespace ZoneMinder
{
    /// <summary>
    /// Streaming options
    /// </summary>
    public class StreamingOptions
    {
        /// <summary>
        /// Source of the stream (Live or record)
        /// </summary>
        public enum Source { Live, Event }
        /// <summary>
        /// Mode (video or snapshot)
        /// </summary>
        public enum Mode { MJPEG, JPEG }

        /// <summary>
        /// Scaled down the quality in percent (leave empty for no scale)
        /// </summary>
        public int Scale { get; set; }
        /// <summary>
        /// Resized width in pixel (leave empty for no resize)
        /// </summary>
        public int Width { get; set; }
        /// <summary>
        /// Resized height in pixel (leave empty for no resize)
        /// </summary>
        public int Height { get; set; }
        /// <summary>
        /// Buffer stream in millisecond (leave empty for no buffer)
        /// </summary>
        public int Buffer { get; set; }
        /// <summary>
        /// Max FPS (leave empty for no limit)
        /// </summary>
        public int MaxFPS { get; set; }
        /// <summary>
        /// Connkey parameter is essentially a random number which uniquely identifies a stream. If you don’t specify a connkey, ZM will generate its own. It is recommended to generate a connkey because you can then use it to "control" the stream (pause/resume etc.)
        /// </summary>
        public string ConnKey { get; set; }
        /// <summary>
        /// Include or not the root URI (to get an absolute or relative URI)
        /// </summary>
        public bool IncludeRootURI { get; set; }
        /// <summary>
        ///  Include or not the autentification token
        /// </summary>
        public bool IncludeAuthentificationToken { get; set; }
    }
}
