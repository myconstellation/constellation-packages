using Constellation;
using Constellation.Package;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace GoogleCalendar
{
    public class Program : PackageBase
    {

        private static string[] Scopes = { CalendarService.Scope.Calendar };
        private static string ApplicationName = "ConstellationCalendar";
        private CalendarService service;

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

            this.CreateTokenFile(PackageHost.GetSettingValue("client_secret"));
            this.Init();

            Task.Factory.StartNew(() =>
            {
                while (PackageHost.IsRunning)
                {
                    this.GetUpcomingEvents(PackageHost.GetSettingValue<int>("interval"));

                    Thread.Sleep(PackageHost.GetSettingValue<int>("refresh"));
                }
            });
        }

        /// <summary>
        /// Create file containing Google Auth Token
        /// </summary>
        /// /// <param name="token">Content of client_secret file</param>
        public void CreateTokenFile(string token)
        {
            File.WriteAllText("client_secret.json", token);
        }

        /// <summary>
        /// Init Google Calendar service
        /// </summary>
        public void Init()
        {
            try
            {
                UserCredential credential;

                using (var stream =
                  new FileStream("client_secret.json", FileMode.Open, FileAccess.Read))
                {
                    string credPath = System.Environment.GetFolderPath(
                      System.Environment.SpecialFolder.Personal);
                    credPath = Path.Combine(credPath, ".credentials/constellation-google-calendar.json");

                    credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                      GoogleClientSecrets.Load(stream).Secrets,
                      Scopes,
                      "user",
                      CancellationToken.None,
                      new FileDataStore(credPath, true)).Result;
                    PackageHost.WriteInfo("Credential file saved to: " + credPath);
                }

                service = new CalendarService(new BaseClientService.Initializer()
                {
                    HttpClientInitializer = credential,
                    ApplicationName = ApplicationName,
                });
            }
            catch(Exception ex)
            {
                PackageHost.WriteError("Error during Google Calendar connection : {0}", ex.ToString());
                Environment.Exit(1);
            }
        }

        /// <summary>
        /// Send the calendar to the Constellation
        /// </summary>
        /// <param name="period">Period of time to sync (in hour)</param>
        public void GetUpcomingEvents(int period)
        {
            try
            {
                EventsResource.ListRequest request = service.Events.List("primary");
                request.TimeMin = DateTime.Now;
                request.TimeMax = DateTime.Now.AddHours(period);
                request.ShowDeleted = false;
                request.SingleEvents = true;
                request.MaxResults = period;
                request.OrderBy = EventsResource.ListRequest.OrderByEnum.StartTime;

                List<string> myEvents = new List<string>();

                Events events = request.Execute();
                if (events.Items != null && events.Items.Count > 0)
                {
                    foreach (var eventItem in events.Items)
                    {
                        string tmp = JsonConvert.SerializeObject(eventItem);
                        myEvents.Add(tmp);
                    }
                    PackageHost.PushStateObject("TodayEvents", myEvents);
                }
                else
                {
                    PackageHost.PushStateObject("TodayEvents", null);
                }

                PackageHost.WriteInfo("Calendar events for the next {0}h are synced", period);
            }
            catch(Exception ex)
            {
                PackageHost.WriteError("Error while trying to sync calendar events : {0}", ex.ToString());
            }
        }
        /// <summary>
        /// Add a new event to the calendar
        /// </summary>
        /// <param name="summary">Summary of the event</param>
        /// <param name="location">Location of the event</param>
        /// <param name="start">Start time</param>
        /// <param name="end">End time</param>
        [MessageCallback]
        public void AddEvent(string summary, string location, Date start, Date end)
        {
            try
            {
                Event tmp = new Event();
                tmp.Summary = summary;
                tmp.Location = location;

                EventDateTime startTime = new EventDateTime();
                startTime.DateTime = new DateTime(start.Year, start.Month, start.Day, start.Hours, start.Minutes, 0);
                tmp.Start = startTime;

                EventDateTime endTime = new EventDateTime();
                endTime.DateTime = new DateTime(end.Year, end.Month, end.Day, end.Hours, end.Minutes, 0);
                tmp.End = endTime;

                service.Events.Insert(tmp, "primary").Execute();

                PackageHost.WriteInfo("Event {0} added to Google Calendar", summary);
            }
            catch (Exception ex)
            {
                PackageHost.WriteError("Error while creating an event : {0}", ex.ToString());
            }
        }
    }

    public class Date
    {
        public int Day { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public int Hours { get; set; }
        public int Minutes { get; set; }
    }
}
