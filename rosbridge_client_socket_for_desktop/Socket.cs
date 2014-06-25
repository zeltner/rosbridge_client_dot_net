using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Rosbridge.Client
{
    public class SocketException : Exception
    {
        public SocketException() : base() { }
        public SocketException(string message) : base(message) { }
        public SocketException(string message, Exception innerException) : base(message, innerException) { }
    }

    public class Socket : ISocket
    {
        private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        private readonly ClientWebSocket _webSocket = new ClientWebSocket();
        private bool _disposed = false;

        /// <summary>
        /// Rosbridge server's IP
        /// </summary>
        public Uri URI { get; private set; }

        /// <summary>
        /// True if the connection to the server is established
        /// </summary>
        public bool Connected
        {
            get
            {
                return _webSocket.State == WebSocketState.Open;
            }
        }

        /// <summary>
        /// Constructs a rosbridge client socket object 
        /// </summary>
        /// <param name="uri">URI of the rosbridge server</param>
        /// <exception cref="System.ArgumentNullException">An argument is null</exception>
        public Socket(Uri uri)
        {
            if (null == uri)
            {
                throw new ArgumentNullException("uri");
            }

            URI = uri;
        }

        /// <summary>
        /// Opens a connection to the rosbridge server
        /// </summary>
        /// <returns>A task which completes once the connect is done</returns>
        /// <exception cref="System.ObjectDisposedException">This socket is already _disposed</exception>
        public Task ConnectAsync()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException("Socket");
            }

            return Task.Run(async () =>
            {
                await _webSocket.ConnectAsync(URI, _cancellationTokenSource.Token);

                if (_webSocket.State != WebSocketState.Open)
                {
                    throw new SocketException("Could not connect to " + URI + " (" + _webSocket.State.ToString() + ")");
                }
            });
        }

        /// <summary>
        /// Closes a connection to the rosbridge server
        /// </summary>
        /// <returns>A task which completes once the disconnect is done</returns>
        /// <exception cref="System.ObjectDisposedException">This socket is already _disposed</exception>
        public Task DisconnectAsync()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException("Socket");
            }

            return Task.Run(async () =>
            {
                await _webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Close", _cancellationTokenSource.Token);

                if (_webSocket.State != WebSocketState.Closed)
                {
                    throw new SocketException("Could not connect to " + URI + " (" + _webSocket.State.ToString() + ")");
                }
            });
        }

        /// <summary>
        /// Sends a message to the rosbridge server
        /// </summary>
        /// <param name="buffer">Mesage to Send</param>
        /// <returns>A task which completes once the send is done</returns>
        /// <exception cref="System.ArgumentNullException">'buffer' is null</exception>
        /// <exception cref="System.ObjectDisposedException">This socket is already _disposed</exception>
        public Task SendAsync(byte[] buffer)
        {
            if (_disposed)
            {
                throw new ObjectDisposedException("Socket");
            }
            if (null == buffer)
            {
                throw new System.ArgumentNullException("buffer");
            }

            return _webSocket.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text,
                true, _cancellationTokenSource.Token);
        }

        /// <summary>
        /// Receives a message from the rosbridge server
        /// </summary>
        /// <returns>Output buffer</returns>
        /// <exception cref="System.ObjectDisposedException">This socket is already _disposed</exception>
        /// <exception cref="Rosbridge.Client.SocketException">Could not receive a message from the server</exception>
        public Task<byte[]> ReceiveAsync()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException("Socket");
            }

            var tcs = new TaskCompletionSource<byte[]>();
            Task.Run(async () =>
            {
                using (var buffer = new MemoryStream())
                {
                    WebSocketReceiveResult result;

                    do
                    {
                        var tmp_buffer = new byte[65535];
                        result = await _webSocket.ReceiveAsync(new ArraySegment<byte>(tmp_buffer), _cancellationTokenSource.Token);

                        // The server closes the connection; reply OK
                        if (result.MessageType == WebSocketMessageType.Close)
                        {
                            await _webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "OK", _cancellationTokenSource.Token);
                            tcs.SetException(new SocketException("Connection closed by the server"));
                            return;
                        }

                        // We don't support binary
                        if (result.MessageType == WebSocketMessageType.Binary)
                        {
                            await _webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "OK", _cancellationTokenSource.Token);
                            tcs.SetException(new SocketException("Server sent binary data"));
                            return;
                        }

                        buffer.Write(tmp_buffer, 0, result.Count);

                    } while (null != result && !result.EndOfMessage);

                    tcs.SetResult(buffer.ToArray());
                }
            });

            return tcs.Task;
        }

        /// <summary>
        /// Closes the socket and clean-up
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            _disposed = true;

            // Dispose should not throw anything
            try
            {
                // we are called from Dispose()
                if (disposing)
                {
                    if (null != _webSocket)
                    {
                        _webSocket.Abort();
                        _webSocket.Dispose();
                    }
                }
            }
            catch { }
        }
    }
}
