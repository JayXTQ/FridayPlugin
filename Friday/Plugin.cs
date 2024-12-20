﻿using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using Exiled.API.Enums;
using Exiled.API.Features;
using Newtonsoft.Json;
using PluginAPI.Events;

namespace Friday
{
    public class Plugin: Plugin<Config>
    {
        public override string Name => "Friday";
        public override string Author => "JayXTQ";
        public override string Prefix => "Friday";
        public override Version Version => new (1, 2, 4);
        public override Version RequiredExiledVersion => new (8, 11, 0);
        public override PluginPriority Priority => PluginPriority.Default;
        
        public static Plugin Instance;
        
        EventHandler handler;
        
        public override void OnEnabled()
        {
            Instance = this;
            handler = new EventHandler();
            EventManager.RegisterEvents(this, handler);
            base.OnEnabled();
        }
    
        public override void OnDisabled()
        {
            EventManager.UnregisterEvents(this, handler);
            handler = null;
            base.OnDisabled();
        }
        
        public async Task ReportPlayer(PlayerReportEvent ev)
        {
            Log.Info("Report incoming...");
            var jsonData = new
            {
                reporterName = ev.Player.Nickname,
                reporterId = ev.Player.UserId,
                reportedName = ev.Target.Nickname,
                reportedId = ev.Target.UserId,
                reason = ev.Reason,
                serverName = Server.Name,
                serverType = 0
            };
            
            using HttpClient client = new();
            
            client.DefaultRequestHeaders.Authorization = new ("Bearer", Config.Token);
            
            StringContent content = new (JsonConvert.SerializeObject(jsonData));
            
            content.Headers.ContentType = new ("application/json");
            
            HttpResponseMessage response = await client.PostAsync("https://friday.jxtq.moe/report", content);
            
            string responseString = await response.Content.ReadAsStringAsync();
            
            if (response.StatusCode != HttpStatusCode.OK)
            {
                Log.Error("Error from server: " + responseString);
            }
            else
            {
                Log.Info(responseString);
            }
        }
    }
}