using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rosbridge.Client
{
    public class Socket : ISocket
    {
        /// <summary>
        /// Rosbridge server's IP
        /// </summary>
        public Uri URI { get; private set; }

        /// <summary>
        /// True if the connection to the server is established
        /// </summary>
        public bool Connected { get; private set; }

        /// <summary>
        /// Opens a connection to the rosbridge server
        /// </summary>
        /// <returns>A task which completes once the connect is done</returns>
        public Task ConnectAsync()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Closes a connection to the rosbridge server
        /// </summary>
        /// <returns>A task which completes once the disconnect is done</returns>
        public Task DisconnectAsync()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Sends a message to the rosbridge server
        /// </summary>
        /// <param name="buffer">Mesage to Send</param>
        /// <returns>A task which completes once the send is done</returns>
        public Task SendAsync(byte[] buffer)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Receives a message from the rosbridge server
        /// </summary>
        /// <returns>Output buffer</returns>
        public Task<byte[]> ReceiveAsync()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Closes the socket and clean-up
        /// </summary>
        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
