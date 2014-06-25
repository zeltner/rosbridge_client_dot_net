using System;
using System.Threading.Tasks;

namespace Rosbridge.Client
{
    public interface ISocket : IDisposable
    {
        /// <summary>
        /// Rosbridge server's URI
        /// </summary>
        Uri URI { get; }

        /// <summary>
        /// True if the connection to the server is established
        /// </summary>
        bool Connected { get; }

        /// <summary>
        /// Opens a connection to the rosbridge server
        /// </summary>
        /// <returns>True if the connect was successful</returns>
        Task<bool> ConnectAsync();

        /// <summary>
        /// Closes a connection to the rosbridge server
        /// </summary>
        /// <returns>True if the disconnect was successful</returns>
        Task<bool> DisconnectAsync();

        /// <summary>
        /// Sends a message to the rosbridge server
        /// </summary>
        /// <param name="buffer">Mesage to Send</param>
        /// <returns>A task which completes once the send is done</returns>
        Task SendAsync(byte[] buffer);

        /// <summary>
        /// Receives a message from the rosbridge server
        /// </summary>
        /// <returns>Output buffer</returns>
        Task<byte[]> ReceiveAsync();
    }
}
