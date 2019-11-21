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

using System.Diagnostics.CodeAnalysis;
using System.Net.Sockets;
using System.Threading.Tasks;
using Serilog;

namespace Blyss.PurposeDome
{
    public class ClientConnector
    {
        private static ILogger Log = Serilog.Log.ForContext<ClientConnector>();

        private TcpListener _server;
        private Task<TcpClient>? _connectTask;

        public ClientConnector(TcpListener server)
        {
            _server = server;
        }

        public bool TryGetClient([NotNullWhen(true)] out TcpClient? client)
        {
            if (_connectTask == null)
            {
                _connectTask = _server.AcceptTcpClientAsync();
            }

            if (_connectTask.IsCompletedSuccessfully)
            {
                client = _connectTask.Result;
                _connectTask = _server.AcceptTcpClientAsync();
                return true;
            }

            if (_connectTask.IsFaulted)
            {
                Log.Error(_connectTask.Exception, "Error waiting for a client to connect!");
                _connectTask = _server.AcceptTcpClientAsync();
            }

            client = null;
            return false;
        }
    }
}
