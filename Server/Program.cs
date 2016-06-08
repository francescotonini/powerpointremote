/*
  _____                                   _       _   _____                      _       
 |  __ \                                 (_)     | | |  __ \                    | |      
 | |__) |____      _____ _ __ _ __   ___  _ _ __ | |_| |__) |___ _ __ ___   ___ | |_ ___ 
 |  ___/ _ \ \ /\ / / _ \ '__| '_ \ / _ \| | '_ \| __|  _  // _ \ '_ ` _ \ / _ \| __/ _ \
 | |  | (_) \ V  V /  __/ |  | |_) | (_) | | | | | |_| | \ \  __/ | | | | | (_) | ||  __/
 |_|   \___/ \_/\_/ \___|_|  | .__/ \___/|_|_| |_|\__|_|  \_\___|_| |_| |_|\___/ \__\___|
                             | |                                                         
                             |_|                                                         
 
Built in less than 10 minutes

 */

using Interceptor;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Server
{
    class Program
    {
        private const int PORT = 4000;
        private static Input input;
        private static TcpListener listener;

        static void Main(string[] args)
        {
            Console.WriteLine("####################################");
            Console.WriteLine("PowerPoint Remote");
            Console.WriteLine("####################################");

            // Shows IP
            ShowLocalIP();

            // Load keyboard drivers
            InitializeDriver();

            // Start server
            StartServer();

            // Good!
            LogSuccess("Initialized completed! Buon esame :)");

            Console.ReadKey(true);
        }

        public static void ShowLocalIP()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                Console.WriteLine(ip.ToString());
            }
        }

        static void LogSuccess(string s)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(s);
            Console.ForegroundColor = ConsoleColor.Gray;
        }

        static void LogError(string s)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(s);
            Console.ForegroundColor = ConsoleColor.Gray;
        }

        static void InitializeDriver()
        {
            try
            {
                input = new Input();
                input.KeyboardFilterMode = KeyboardFilterMode.All;
                input.Load();

                LogSuccess("Keyboard driver OK");
            }
            catch(Exception ex)
            {
                LogError("Unable to initialize driver!");
                LogError(ex.ToString());
            }
        }

        static void StartServer()
        {
            try
            {
                listener = new TcpListener(IPAddress.Any, PORT);
                listener.Start();

                LogSuccess(string.Format("Listening on port {0}", PORT));

                ConnectRemote();
            }
            catch(Exception ex)
            {
                LogError("Unable to initialize server!");
                LogError(ex.ToString());
            }
        }

        static void ConnectRemote()
        {
            Console.WriteLine("Waiting for remote connection");

            // Waits for the remote
            TcpClient client = listener.AcceptTcpClient();
            LogSuccess("Remote connected!");

            // Start remote thread
            Thread remoteT = new Thread(RemoteThread);
            remoteT.Start(client);
        }

        static void RemoteThread(object obj)
        {
            TcpClient client = obj as TcpClient;
            if (client == null)
            {
                LogError("Invalid client!");
                return;
            }

            NetworkStream stream = client.GetStream();
            while (true)
            {
                // Read from network
                byte[] buffer = new byte[4];
                stream.Read(buffer, 0, 4);

                // Convert into string
                string s = System.Text.Encoding.ASCII.GetString(buffer);
                SendToKeyboard(s);
            }
        }

        static void SendToKeyboard(string signal)
        {
            switch(signal)
            {
                case "NEXT":
                    Console.WriteLine("NEXT");
                    input.SendKey(Keys.Right);
                    break;
                case "PREV":
                    Console.WriteLine("PREV");
                    input.SendKey(Keys.Left);
                    break;
                case "ENDP":
                    Console.WriteLine("ENDP");
                    input.SendKey(Keys.End);
                    break;
            }
        }
    }
}
