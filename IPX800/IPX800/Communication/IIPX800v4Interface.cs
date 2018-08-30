/*
 *	 GCE Electronics IPX800 Package for Constellation
 *	 Web site: http://www.myConstellation.io
 *	 Copyright (C) 2018 - Sebastien Warin <http://sebastien.warin.fr>
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

namespace IPX800.Communication
{
    using IPX800.Enumerations;
    using Newtonsoft.Json.Linq;

    /// <summary>
    ///  Defines an IPX800v4 communication interface
    /// </summary>
    public interface IIPX800v4Interface
    {
        /// <summary>
        /// Gets the specified argument.
        /// </summary>
        /// <param name="argument">The argument.</param>
        /// <param name="parameters">The optional parameters.</param>
        /// <returns></returns>
        JObject Get(GetArgument argument = GetArgument.All, string parameters = null);

        /// <summary>
        /// Sets the specified argument.
        /// </summary>
        /// <param name="argument">The argument.</param>
        /// <param name="elementId">The element identifier.</param>
        /// <param name="value">The optional value.</param>
        /// <returns></returns>
        JObject Set(SetArgument argument, int elementId, string value = null);

        /// <summary>
        /// Clears the specified argument.
        /// </summary>
        /// <param name="argument">The argument.</param>
        /// <param name="elementId">The element identifier.</param>
        /// <returns></returns>
        JObject Clear(SetArgument argument, int elementId);

        /// <summary>
        /// Toggles the specified argument.
        /// </summary>
        /// <param name="argument">The argument.</param>
        /// <param name="elementId">The element identifier.</param>
        /// <returns></returns>
        JObject Toggle(SetArgument argument, int elementId);
    }
}
