using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rosbridge.Client
{
    public class ServiceClient
    {
        private MessageDispatcher _md;
        private readonly string _uid = MessageDispatcher.GetUID();

        /// <summary>
        /// The topic the subscriber is subscribing to
        /// </summary>
        public string Service { get; private set; }
        
        /// <summary>
        /// Constructs a new ServiceClient object
        /// </summary>
        /// <param name="service">The service to call</param>
        /// <param name="md">The message dispatcher which handles the transactions</param>
        public ServiceClient(string service, MessageDispatcher md)
        {
            if (null == service)
            {
                throw new ArgumentNullException("service");
            }
            if (null == md)
            {
                throw new ArgumentNullException("md");
            }

            Service = service;
            _md = md;
        }

        /// <summary>
        /// Calls the service
        /// </summary>
        /// <param name="arguments">The service arguments</param>
        /// <returns>The result</returns>
        /// <exception cref="System.ArgumentNullException">'arguments' is null</exception>
        public Task<JToken> Call(List<dynamic> arguments)
        {
            if (null == arguments)
            {
                throw new ArgumentNullException("arguments");
            }

            var tcs = new TaskCompletionSource<JToken>();

            MessageReceivedHandler handler = delegate(object sender, MessageReceivedEventArgs e)
            {
                JToken value_id;
                if (e.Message.TryGetValue("id", out value_id))
                {
                    string id = value_id.ToString();
                    if (id.Equals(_uid))
                    {
                        JToken value_values;
                        if (e.Message.TryGetValue("values", out value_values))
                        {
                            tcs.SetResult(value_values);
                        }
                        else
                        {
                            tcs.SetResult(null);
                        }
                    }
                }
            };

            // Register the callback
            _md.MessageReceived += handler;
            
            Task.Run(async () =>
            {
                if (arguments.Count == 0)
                {
                    await _md.SendAsync(new
                    {
                        op = "call_service",
                        id = _uid,
                        service = Service
                    });
                }
                else
                {
                    await _md.SendAsync(new
                    {
                        op = "call_service",
                        id = _uid,
                        service = Service,
                        args = arguments
                    });
                }

                // Wait for the result
                await tcs.Task;

                _md.MessageReceived -= handler;
            });

            return tcs.Task;
        }
    }
}
