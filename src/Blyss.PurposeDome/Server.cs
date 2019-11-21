// Copyright (c) 2019 Foomf
//
// This software is provided 'as-is', without any express or implied
// warranty. In no event will the authors be held liable for any damages
// arising from the use of this software.
//
// Permission is granted to anyone to use this software for any purpose,
// including commercial applications, and to alter it and redistribute it
// freely, subject to the following restrictions:
//
// 1. The origin of this software must not be misrepresented; you must not
//    claim that you wrote the original software. If you use this software
//    in a product, an acknowledgment in the product documentation would be
//    appreciated but is not required.
// 2. Altered source versions must be plainly marked as such, and must not be
//    misrepresented as being the original software.
// 3. This notice may not be removed or altered from any source distribution.

using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using Serilog;

namespace Blyss.PurposeDome
{
    using System.Threading.Tasks;

    public class Server
    {
        private static ILogger Log = Serilog.Log.ForContext<Server>();

        private ushort port = 3786;
        private IPAddress localAddr = IPAddress.Parse("127.0.0.1");
        private bool stopped = false;
        private List<TcpClient> clients = new List<TcpClient>();

        public void Stop()
        {
            stopped = true;
        }

        public async Task RunAsync()
        {
            var server = new TcpListener(localAddr, port);
            server.Start();
            server.BeginAcceptTcpClient(OnConnect, server);

            Log.Information("Server started. Press Ctrl+C to stop.");
            while (!stopped)
            {
                // do server game loop stuff
                await Task.Delay(TimeSpan.FromMilliseconds(100));
            }

            server.Stop();
        }

        void OnConnect(IAsyncResult r)
        {
            if (!(r.AsyncState is TcpListener server))
            {
                return;
            }

            var client = server.EndAcceptTcpClient(r);
            client.NoDelay = true;
            clients.Add(client);
            Log.Information("Client connected!");

            server.BeginAcceptTcpClient(OnConnect, server);
        }
    }
}
