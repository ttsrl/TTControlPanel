using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Hosting;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading;
using TTControlPanel.Models;

namespace TTControlPanel.Services
{
    public class KeepAlive
    {
        private bool first = false;
        private static bool started = false;
        private int _count = 0;
        private string host = "";
        private HttpClient client;
        private readonly DBContext _db;
        private readonly Timer _timer;

        private int timerTick = 900; //15 minuti

        public int Count { get => _count; set => _count = value; }
        public static bool Started { get => started; }
        public string Host { get => host; }

        public KeepAlive(IHttpContextAccessor httpContextAccessor, IWebHostEnvironment env)
        {
            if (httpContextAccessor.HttpContext == null || env.IsDevelopment())
                return;
            started = true;
            var httpConnectionFeature = httpContextAccessor.HttpContext.Features.Get<IHttpConnectionFeature>();
            host = "localhost"; //httpConnectionFeature?.LocalIpAddress.ToString();
            client = new HttpClient();
            client.Timeout = TimeSpan.FromSeconds(60); // 1 minuto
            _db = new DBContext(DBContext.Options);
            _count = _db.KeepAliveRequests.Count();
            //timer
            _timer = new Timer(Request, null, TimeSpan.Zero, TimeSpan.FromSeconds(timerTick));
        }

        private async void Request(object state)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            HttpResponseMessage response = await client.GetAsync("http://" + Host + "/home");
            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds;

            KeepAliveRequest kar = new KeepAliveRequest
            {
                DurationRequest = Convert.ToInt32(elapsedMs),
                Response = (int)response.StatusCode,
                Inizialize = !first,
            };

            _db.KeepAliveRequests.Add(kar);
            await _db.SaveChangesAsync();
            _count++;
            first = true;
        }
    }
}
