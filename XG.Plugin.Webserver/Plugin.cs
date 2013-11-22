// 
//  Plugin.cs
//  This file is part of XG - XDCC Grabscher
//  http://www.larsformella.de/lang/en/portfolio/programme-software/xg
//
//  Author:
//       Lars Formella <ich@larsformella.de>
// 
//  Copyright (c) 2012 Lars Formella
// 
//  This program is free software; you can redistribute it and/or modify
//  it under the terms of the GNU General Public License as published by
//  the Free Software Foundation; either version 2 of the License, or
//  (at your option) any later version.
// 
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
//  GNU General Public License for more details.
// 
//  You should have received a copy of the GNU General Public License
//  along with this program; if not, write to the Free Software
//  Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA 02111-1307 USA
//  

using System;
using System.Linq;
using SharpRobin.Core;
using XG.Config.Properties;
using Microsoft.Owin.Hosting;
using XG.Plugin.Webserver.SignalR;
using XG.Plugin.Webserver.SignalR.Hub;
using System.Reflection;
using System.Net;
using log4net;

namespace XG.Plugin.Webserver
{
	public class Plugin : APlugin
	{
		#region VARIABLES

		static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

		IDisposable _server;
		EventForwarder _eventForwarder;

		public RrdDb RrdDB { get; set; }

		#endregion

		#region AWorker

		protected override void StartRun()
		{
			Helper.Servers = Servers;
			Helper.Files = Files;
			Helper.Searches = Searches;
			Helper.RrdDb = RrdDB;

			var options = new StartOptions
			{
				ServerFactory = "Nowin",
				Port = 8080// Settings.Default.WebserverPort)
			};

			try
			{
				_server = WebApp.Start<Startup>(options);
			}
			catch (TargetInvocationException ex)
			{
#if !__MonoCS__
				var inner = ex.InnerException as HttpListenerException;
				if (inner != null && inner.NativeErrorCode == 5)
				{
					Log.Fatal(@"TO GET XG UP AND RUNNING YOU MUST RUN 'netsh http add urlacl url=http://*:" + Settings.Default.WebserverPort + @"/ user=%USERDOMAIN%\%USERNAME%' AS ADMINISTRATOR");
				}
#endif
				throw ex;
			}

			_eventForwarder = new EventForwarder();
			_eventForwarder.Servers = Servers;
			_eventForwarder.Files = Files;
			_eventForwarder.Searches = Searches;
			_eventForwarder.Start();

			// test
			var firstParts = (from file in Files.All where file.Parts.Any() select file.Parts.First()).ToArray();
			var connectedPackets = (from server in Servers.All from channel in server.Channels from bot in channel.Bots from packet in bot.Packets where packet.Connected select packet).ToArray();

			int a = 0;
			Random random = new Random();
			foreach (var packet in connectedPackets)
			{
				if (a == firstParts.Count())
				{
					break;
				}
				packet.Part = firstParts.Skip(a).First();
				packet.Part.Speed = random.Next(10000, 99999);
				a++;
			}
		}

		protected override void StopRun()
		{
			_eventForwarder.Stop();
			_server.Dispose();
		}

		#endregion
	}
}
