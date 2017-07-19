using Constellation;
using Constellation.Package;
using System.Xml;
using HtmlAgilityPack;
using System.Text.RegularExpressions;
using Horoscope.Models;
using System.Collections.Generic;
using System;
using System.Threading;
using System.Text;
using System.Globalization;

namespace Horoscope
{
    public class Program : PackageBase
    {

        private DateTime dateProcessed = DateTime.MinValue;

        static void Main(string[] args)
        {
            PackageHost.Start<Program>(args);
        }

        /// <summary>
        /// Called when the package is started.
        /// </summary>
        public override void OnStart()
        {
            PackageHost.WriteInfo("Package starting - IsRunning: {0} - IsConnected: {1}", PackageHost.IsRunning, PackageHost.IsConnected);
            
            //// While Package is running
            while (PackageHost.IsRunning)
            {
                // Each day
                if (DateTime.Now.Date != dateProcessed.Date)
                {
                    //// Compare time with RefreshTime from settings
                    DateTime t1 = DateTime.Now;
                    DateTime t2 = Convert.ToDateTime(PackageHost.GetSettingValue<string>("RefreshTime"));
                    int difference = DateTime.Compare(t1, t2);

                    //// If t1 is superior to t2
                    if(difference == 1)
                    {
                        //// Getting signs settings as xml
                        var xml = PackageHost.GetSettingAsXmlDocument("ZodiacalSigns");
                        foreach (XmlNode sign in xml.ChildNodes[0])
                        {
                            //// If parent node name is sign
                            if (sign.Name == "sign")
                            {
                                //// Getting horoscope for the sign
                                Horoscopes horoscope = GetHoroscope(sign.Attributes["name"].Value);
                                PackageHost.WriteInfo("Getting horoscope for {0}", sign.Attributes["name"].Value);

                                //// Push the SO
                                PackageHost.PushStateObject<Horoscopes>(sign.Attributes["name"].Value, horoscope);
                            }
                             dateProcessed = DateTime.Now;
                        }
                    }                
                    Thread.Sleep(1000);
                }
            }
        }

        /// <summary>
        /// Get horoscope for a zodiacal sign.
        /// </summary>
        /// <param name="ZodiacalSign">Zodiacal sign.</param>
        /// <returns>Horoscope for the day</returns>
        [MessageCallback(Key = "GetHoroscope")]
        public Horoscopes GetHoroscope(string ZodiacalSign)
        {
           try
            {
                //// Create URL with the sign
                string url = String.Format(PackageHost.GetSettingValue<string>("Url"), RemoveDiacritics(ZodiacalSign.ToLower()));

                //// New horoscope class and section class
                Horoscopes horoscope = new Horoscopes();
                List<Section> section = new List<Section>();

                //// Getting xml file from URL
                XmlDocument xml = new XmlDocument();
                XmlNamespaceManager namespaces = new XmlNamespaceManager(xml.NameTable);
                namespaces.AddNamespace("ns", "urn:hl7-org:v3");
                xml.Load(url);
                XmlNode date = xml.SelectSingleNode("/rss/channel/item/title", namespaces);
                horoscope.Date = date.InnerText;
                XmlNode idNode = xml.SelectSingleNode("/rss/channel/item/description", namespaces);

                //// Remove useless stuff
                HtmlDocument doc = new HtmlDocument();
                doc.LoadHtml(idNode.InnerText);
                HtmlAgilityPack.HtmlNodeCollection Nodes = doc.DocumentNode.SelectNodes("//center");
                foreach (HtmlNode node in Nodes)
                {
                    node.Remove();
                }
                foreach (var brtag in doc.DocumentNode.SelectNodes("//br"))
                {
                    brtag.Remove();
                }

                //// Getting Section data
                foreach (var btag in doc.DocumentNode.SelectNodes("//b"))
                {
                    var data = btag.SelectSingleNode("following-sibling::text()[1]");
                    string sectionData = Regex.Replace(data.InnerText, @"\t|\n|\r", "");
                    section.Add(new Section() { Title = btag.InnerText.Substring(btag.InnerText.IndexOf('-') + 1).Trim(), Horoscope = sectionData });
                }
                horoscope.Section = section;

                return horoscope;
            }
            catch (Exception ex)
            {
                //// Return null if error
                PackageHost.WriteError("{0}", ex);
                return null;
            }
        }

        private static string RemoveDiacritics(string text)
        {
            var normalizedString = text.Normalize(NormalizationForm.FormD);
            var stringBuilder = new StringBuilder();

            foreach (var c in normalizedString)
            {
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }

            return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
        }
    }
}
