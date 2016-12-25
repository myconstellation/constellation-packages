/*
 *	 DayInfo Package for Constellation
 *	 Web site: http://www.myConstellation.io
 *	 Copyright (C) 2014-2016 - Sebastien Warin <http://sebastien.warin.fr>	   	  
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

namespace DayInfo.Utils
{
    using System;
    using System.Collections.Generic;

    public static class NameDayUtils
    {
        private static Dictionary<string, string> nameDays = new Dictionary<string, string>();

        static NameDayUtils()
        {
            string[] lines = Properties.Resources.fetes.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 1; i < lines.Length; i++)
            {
                string[] lineDatas = lines[i].Split('\t');
                if (lineDatas.Length > 1)
                {
                    nameDays.Add(lineDatas[6].Replace("/2013", ""), lineDatas[0]);
                }
            }
        }
        
        public static string GetNameDay()
        {
            return GetNameDay(DateTime.Now);
        }

        public static string GetNameDay(DateTime date)
        {
            string dateStr = date.ToString("dd/MM");
            if (nameDays.ContainsKey(dateStr))
            {
                return nameDays[dateStr];
            }
            else
            { 
                return string.Empty;
            }
        }
    }
}
