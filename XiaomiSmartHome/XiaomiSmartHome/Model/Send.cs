using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Constellation.Package;
using System.Reflection;
using Newtonsoft.Json;
using XiaomiSmartHome;

namespace XiaomiSmartHome.Model
{
    class Send
    {
        
        
        //// Listening multicast to get reports
        public static  void SendData(Response data)
        {
            //// Equipement name
        string name = null;
            //// Try to get setting based on equipement SID
            if (!PackageHost.TryGetSettingValue<string>(data.Sid, out name))
            {
                name = data.Sid;
            }

            Type modelReportType = Assembly.GetExecutingAssembly().GetTypes().SingleOrDefault(t => t.GetCustomAttribute<Response.XiaomiEquipementAttribute>()?.Model == data.Model + "_report");

            if (modelReportType == null)
            {
                PackageHost.WriteError("{0}_report type not found !", data.Model);
                return;
            }

            dynamic test = XiaomiSmartHome.Program.Equipements[data.Model][data.Sid].Report;

            dynamic test7 = JsonConvert.DeserializeObject(Convert.ToString(data.Data), modelReportType);

            var values = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(data.Data);
            foreach (var value in values)
            {

                string test11 = XiaomiSmartHome.Program.EquipementsName[value.Key];

                PropertyInfo test12 = test.GetType().GetProperty(test11);

                Type test13 = value.Value.GetType();
                dynamic test15 = Convert.ChangeType(value.Value, test12.PropertyType);

                Type test14 = test15.GetType();

                test.GetType().GetProperty(test11).SetValue(test, test15, null);

                Program.Equipements[data.Model][data.Sid].Report = test;
                PackageHost.PushStateObject<dynamic>(name, Program.Equipements[data.Model][data.Sid]);
            }
        }
    }
}
