using System;
using System.Text.Json;

using WebSocketSharp;
using WebSocketSharp.Net;
using WebSocketSharp.Server;

namespace WebSocketTest_Server
{
	public class Server : WebSocketBehavior
	{
		protected override void OnOpen()
		{
			Console.WriteLine("Connected: " + ID);
		}

		protected override void OnMessage(MessageEventArgs e)
		{
			if (!e.IsText) return;

			SocketMessage data = JsonSerializer.Deserialize<SocketMessage>(e.Data, new JsonSerializerOptions { IncludeFields = true });
			Console.WriteLine("Message: {0} | {1} | {2}", ID, data.socketEvent, data.data);

			//string jsonString = JsonSerializer.Serialize(new SocketMessage("Message", "Hello"));
			Send(e.Data);
			//Sessions.Broadcast(e.RawData);
		}

		protected override void OnError(ErrorEventArgs e)
		{
			Console.WriteLine("Error: {0} | {1} ", ID, e.Message);
		}

		protected override void OnClose(CloseEventArgs e)
		{
			Console.WriteLine("Disconnected: {0} ({1}: {2})", ID, e.Code, e.Reason);
		}
	}

	public class Program
	{
		public static void Main(string[] args)
		{
			WebSocketServer socket = new WebSocketServer(System.Net.IPAddress.Any, 6000);

			socket.AuthenticationSchemes = AuthenticationSchemes.Digest;
			socket.UserCredentialsFinder = id => {
				var name = id.Name;

				// name하고 password 반환 (반환된거 가지고 인증함)
				return name == "nobita"
					   ? new NetworkCredential(name, "password")
					   : null; // 해당되는 name이 없을 경우 (인증 거부)
			};

			socket.AddWebSocketService<Server>("/Server");
			socket.Start();
			Console.WriteLine("Server started at: ws://{0}:{1}\nPress any Key to stop Server...\n\n", socket.Address, socket.Port);

			Console.ReadKey(true);
			socket.Stop();
		}
	}
}