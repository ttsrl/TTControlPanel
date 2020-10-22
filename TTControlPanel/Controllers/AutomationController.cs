using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using TTControlPanel.Filters;
using TTControlPanel.Models.ViewModel;
using TTUtils;

namespace TTControlPanel.Controllers
{
    public class AutomationController : Controller
    {
        [HttpGet]
        [Authentication]
        public async Task<IActionResult> Index()
        {
            var isok = false;
            var online = false;
            var powerR1 = false;
            try
            {
                HttpClient client = new HttpClient();
                client.DefaultRequestHeaders.CacheControl = new CacheControlHeaderValue { NoCache = true };
                client.Timeout = TimeSpan.FromMilliseconds(4000);
                StringContent queryString = new StringContent("id=68c63afb5933&auth_key=MTU4MjB1aWQBA532C5A185416F68277C9D27FE067B08AC8E8EA106E6D2E74572AD6100804A5E900152E2C8E6809");
                queryString.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");
                HttpResponseMessage response = await client.PostAsync(new Uri("https://shelly-15-eu.shelly.cloud/device/status"), queryString);
                response.EnsureSuccessStatusCode();
                string result = await response.Content.ReadAsStringAsync();
                JObject jObject = JObject.Parse(result);
                JToken jData = jObject["data"];
                isok = (bool)jObject["isok"];
                online = (bool)jData["online"];
                JToken jDeviceStatus = jData["device_status"];
                var relays = jDeviceStatus["relays"].ToArray();
                powerR1 = (bool)relays[0]["ison"];
                response.Dispose();
            }
            catch { isok = false; online = false; powerR1 = false; }
            var model = new IndexAutomationModel
            {
                Automation1 = new AutomationItem { State = (AutomationState)Convert.ToInt32(isok && online), RelayValue = powerR1 }
            };
            return View(model);
        }
    }
}
