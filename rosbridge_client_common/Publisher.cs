using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rosbridge.Client
{
    public class Publisher
    {
        private MessageDispatcher _md;
        private readonly string _uid = MessageDispatcher.GetUID();

        /// <summary>
        /// The topic the subscriber is subscribing to
        /// </summary>
        public string Topic { get; private set; }

        /// <summary>
        /// The type of the topic
        /// </summary>
        public string Type { get; private set; }
        
        /// <summary>
        /// Constructs a new Publisher object
        /// </summary>
        /// <param name="topic">The topic the publisher is publishing to</param>
        /// <param name="type">The type of the topic</param>
        /// <param name="md">The message dispatcher which handles the transactions</param>
        public Publisher(string topic, string type, MessageDispatcher md)
        {
            if (null == topic)
            {
                throw new ArgumentNullException("topic");
            }
            if (null == type)
            {
                throw new ArgumentNullException("type");
            }
            if (null == md)
            {
                throw new ArgumentNullException("md");
            }

            Topic = topic;
            Type = type;
            _md = md;
        }

        /// <summary>
        /// Advertises a topic
        /// </summary>
        /// <returns>An awaitable task</returns>
        public Task AdvertiseAsync()
        {
            return _md.SendAsync(new
            {
                op = "advertise",
                id = _uid,
                topic = Topic,
                type = Type
            });
        }

        /// <summary>
        /// Unadvertises a topic
        /// </summary>
        /// <returns>An awaitable task</returns>
        public Task UnadvertiseAsync()
        {
            return _md.SendAsync(new
            {
                op = "unadvertise",
                id = _uid,
                topic = Topic
            });
        }

        /// <summary>
        /// Publishes to a topic
        /// </summary>
        /// <param name="message">Message to publish</param>
        /// <returns>An awaitable task</returns>
        public Task PublishAsync(dynamic message)
        {
            return _md.SendAsync(new
            {
                op = "publish",
                id = _uid,
                topic = Topic,
                msg = message
            });
        }
    }
}
