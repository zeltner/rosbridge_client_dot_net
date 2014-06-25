using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rosbridge.Client
{
    public class MessageDispatcherStartedException : Exception
    {
        public MessageDispatcherStartedException() : base() { }
        public MessageDispatcherStartedException(string message) : base(message) { }
        public MessageDispatcherStartedException(string message, Exception innerException) : base(message, innerException) { }
    }

    public class MessageReceivedEventArgs : EventArgs
    {
        public JObject Message { get; private set; }

        public MessageReceivedEventArgs(JObject message)
        {
            Message = message;
        }
    }

    public delegate void MessageReceivedHandler(object sender, MessageReceivedEventArgs e);

    public class MessageDispatcher : IDisposable
    {
        /// <summary>
        /// Get an unique ID
        /// </summary>
        /// <returns>An unique ID</returns>
        public static string GetUID()
        {
            return Guid.NewGuid().ToString();
        }

        private ISocket _socket;
        private IMessageSerializer _serializer;
        private bool _disposed = false;
        private System.Threading.Tasks.Task _receivingTask;

        /// <summary>
        /// This event is fired when a new message is received
        /// </summary>
        public event MessageReceivedHandler MessageReceived;

        public enum State
        {
            Starting,
            Started,
            Stopping,
            Stopped
        }

        /// <summary>
        /// The current state of the dispatcher
        /// </summary>
        public State CurrentState { get; private set; }

        /// <summary>
        /// Creates a new message dispatcher
        /// </summary>
        /// <param name="socket">The underlying socket</param>
        /// <param name="serializer">The serializer</param>
        /// <exception cref="System.ArgumentNullException">An argument is null</exception>
        public MessageDispatcher(ISocket socket, IMessageSerializer serializer)
        {
            if (null == serializer)
            {
                throw new ArgumentNullException("serializer");
            }
            _serializer = serializer;

            if (null == socket)
            {
                throw new ArgumentNullException("socket");
            }
            _socket = socket;

            CurrentState = State.Stopped;
        }

        /// <summary>
        /// Starts the message dispatcher
        /// </summary>
        /// <returns>A task which completes once the start is done</returns>
        /// <exception cref="System.ObjectDisposedException">This socket is already _disposed</exception>
        /// <exception cref="Rosbridge.Client.MessageDispatcherStartedException">The dispatcher was already started</exception>
        public Task StartAsync()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException("Socket");
            }
            if (CurrentState != State.Stopped)
            {
                throw new MessageDispatcherStartedException();
            }

            CurrentState = State.Starting;

            var task = Task.Run(async () =>
            {
                try
                {
                    await _socket.ConnectAsync();
                }
                catch
                {
                    CurrentState = State.Stopped;
                    throw;
                }

                CurrentState = State.Started;
            });

            // start the receiving task
            _receivingTask = Task.Run(async () =>
            {
                // Wait for the connect
                try
                {
                    await task;
                }
                catch
                {
                    // await _socket.ConnectAsync(); throwed an exception
                    return;
                }

                byte[] buffer;
                while (CurrentState == State.Started)
                {
                    try
                    {
                        buffer = await _socket.ReceiveAsync();

                        var message = _serializer.Deserialize(buffer);

                        MessageReceivedHandler tmpMessageReceived = MessageReceived;
                        if (null != tmpMessageReceived)
                        {
                            tmpMessageReceived(this, new MessageReceivedEventArgs(message));
                        }
                    }
                    catch { /* what should we do? */ }
                }
            });

            return task;
        }

        /// <summary>
        /// Stops the message dispatcher
        /// </summary>
        /// <returns>A task which completes once the stop is done</returns>
        /// <exception cref="System.ObjectDisposedException">This socket is already _disposed</exception>
        /// <exception cref="Rosbridge.Client.MessageDispatcherStartedException">The dispatcher was already started</exception>
        public Task StopAsync()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException("Socket");
            }
            if (CurrentState != State.Started)
            {
                throw new MessageDispatcherStartedException();
            }

            CurrentState = State.Stopping;

            return Task.Run(async () =>
            {
                if (null != _socket)
                {
                    await _socket.DisconnectAsync();
                }

                if (null != _receivingTask)
                {
                    await _receivingTask;
                    _receivingTask = null;
                }

                CurrentState = State.Stopped;
            });
        }

        /// <summary>
        /// Sends a message asynchronously. The returned task completes once the send is finished
        /// </summary>
        /// <param name="message">The message tosend</param>
        /// <returns>A task which completes once the send is finished</returns>
        /// <exception cref="System.ObjectDisposedException">This socket is already _disposed</exception>
        /// <exception cref="Rosbridge.Client.MessageDispatcherStartedException">The dispatcher was already started</exception>
        public Task SendAsync(dynamic message)
        {
            if (_disposed)
            {
                throw new ObjectDisposedException("Socket");
            }
            if (CurrentState != State.Started)
            {
                throw new MessageDispatcherStartedException();
            }

            return _socket.SendAsync(_serializer.Serialize(message));
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
            if (_disposed)
            {
                return;
            }

            _disposed = true;
            CurrentState = State.Stopped;

            // create a task which cleans up
            Task.Run(async () =>
            {
                // Dispose should not throw anything
                try
                {
                    // we are called from Dispose()
                    if (disposing)
                    {
                        if (null != _socket)
                        {
                            await _socket.DisconnectAsync();
                        }

                        if (null != _receivingTask)
                        {
                            await _receivingTask;
                            _receivingTask = null;
                        }

                        if (null != _socket)
                        {
                            _socket.Dispose();
                            _socket = null;
                        }

                        if (null != _serializer)
                        {
                            _serializer = null;
                        }
                    }
                }
                catch { }
            });
        }
    }
}
