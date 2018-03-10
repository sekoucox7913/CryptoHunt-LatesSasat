﻿using System;
using WebSocketSharp;

namespace EchoClient
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			if (args.Length != 1) {
				Console.WriteLine ("Usage: EchoClient.exe ws://echo.websocket.org");
				return;
			}

			WebSocket ws = new WebSocket (args [0]);
			ws.OnOpen = (Object sender, EventArgs e) => {
				Console.WriteLine ("Connected");
				Console.Write ("-> ");
			};
			ws.OnMessage = (Object sender, MessageEventArgs e) => {
				Console.WriteLine ("<- " + e.Data);
				Console.Write ("-> ");
			};
			ws.OnError = (Object sender, ErrorEventArgs e) => {
				Console.WriteLine ("ERROR: " + e.Message);
				Console.Write ("-> ");
			};
			ws.OnClose = (Object sender, CloseEventArgs e) => {
				Console.WriteLine ("Closed " + e.Code + e.Reason + e.WasClean);
			};
 
			ws.Connect ();

			while (true) {
				string line = Console.ReadLine ();
				ws.Send (line.TrimEnd (new char[] { '\r', '\n' }));
			}
		}
	}
}
