using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using NinjaFit.Api.Extensions;
using NinjaFit.Api.Models;
using NinjaFit.Api.Support;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace NinjaFit.Api.Services
{
    public class ScheduleService : Service
    {
        private static string BasePath = HttpContext.Current.Server.MapPath("~/Database/Schedule/");

        public static async Task<WeekSchedule> TryGetWeekScheduleAsync(DateTime startDate, DateTime endDate)
        {
            WeekSchedule schedule = new WeekSchedule
            {
                Start = Constants.Schedule.StartTime,
                End   = Constants.Schedule.  EndTime
            };

            Log.Debug($"TryGetWeekScheduleAsync::called startDate={startDate.ToString()}, endDate={endDate.ToString()}");

            try
            {
                WeekSchedule defaultSchedule = await GetDefaultScheduleAsync();

                if (string.IsNullOrEmpty(schedule.Start)) { schedule.Start = defaultSchedule.Start; }
                if (string.IsNullOrEmpty(schedule.End))   { schedule.End   = defaultSchedule.End;   }
                
                var date = startDate.Copy();

                while (date <= endDate)
                {
                    DaySchedule day = await GetDayAsync(date);

                    if (day == null)
                    {
                        day = defaultSchedule.Days.FirstOrDefault(d => d.Day == date.DayOfWeek)
                            ?? new DaySchedule { Day = date.DayOfWeek };

                        day.Date      = date.DateKey();
                        day.IsDefault = true;
                    }

                    schedule.Days.Add(day);

                    date = date.AddDays(1);
                }

                // NOT LOOKING AT RxSchedule current because it's all sorts of screwed up.
                //var classScheduleStart = DateTime.UtcNow.PreviousDay().StartOfDay(); // previous day to account for UTC timezone
                //var classScheduleEnd   = DateTime.UtcNow.NextDay().AddDays(7).StartOfDay();

                //if (startDate <= classScheduleEnd && endDate >= classScheduleStart)
                //{
                //    var classes = await RxService.TryGetClassScheduleAsync();

                //    foreach (var day in classes)
                //    {
                //        var matches = schedule.Days.Where(d => d.Date == day.Date && d.IsDefault);

                //        foreach (var match in matches)
                //        {
                //            match.Blocks = match.Blocks.Union(day.Blocks).OrderBy(b => b.Start).ToList();
                //        }
                //    }
                //}
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }

            return schedule;
        }

        private static async Task<WeekSchedule> GetDefaultScheduleAsync()
        {
            string filename = $"{BasePath}default.json";

            try
            {
                if (File.Exists(filename))
                {
                    using (StreamReader reader = new StreamReader(filename))
                    {
                        string json = await reader.ReadToEndAsync();

                        return JsonConvert.DeserializeObject<WeekSchedule>(json);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }

            return new WeekSchedule();
        }

        private static async Task<DaySchedule> GetDayAsync(DateTime date)
        {
            int    dateKey  = date.DateKey();
            string filename = $"{BasePath}{dateKey}.json";

            try
            {
                if (File.Exists(filename))
                {
                    using (StreamReader reader = new StreamReader(filename))
                    {
                        string json = await reader.ReadToEndAsync();

                        var day = JsonConvert.DeserializeObject<DaySchedule>(json);

                        day.Date = dateKey;
                        day.Day  = date.DayOfWeek;

                        return day;
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }

            return null;
        }
    }

    
}