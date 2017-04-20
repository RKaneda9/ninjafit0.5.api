using NinjaFit.Api.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NinjaFit.Api.Services
{
    public class WodService : Service
    {
        public static async Task<List<Wod>> TryGetWodsAsync()
        {
            try
            {
                return await GetWodsAsync();
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }

            return null;
        }

        public static async Task<List<Wod>> GetWodsAsync()
        {
            List<Wod> wods = new List<Wod>();
            var wod = await RxService.GetWodAsync();

            if (wod != null)
            {
                int count = wod.WodCount;
                wods.Add(wod);

                for (var i = 2; i <= count; i++) { wods.Add(await RxService.GetWodAsync(i)); }
            }

            return wods;
        }
    }
}