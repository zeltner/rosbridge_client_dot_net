using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rosbridge.Client
{
    public class Subscriber
    {
        private MessageDispatcher _md;
        private readonly string _uid = MessageDispatcher.GetUID();

        /// <summary>
        /// This event is fired when a new message is received
        /// </summary>
        public event MessageReceivedHandler MessageReceived;

        /// <summary>
        /// The topic the subscriber is subscribing to
        /// </summary>
        public string Topic { get; private set; }

        /// <summary>
        /// The type of the topic
        /// </summary>
        public string Type { get; private set; }

        /// <summary>
        /// Constructs a new Subscriber object
        /// </summary>
        /// <param name="topic">The topic the subscriber is subscribing to</param>
        /// <param name="type">The type of the topic</param>
        /// <param name="md">The message dispatcher which handles the transactions</param>
        public Subscriber(string topic, string type, MessageDispatcher md)
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

            _md.MessageReceived += _md_MessageReceived;
        }

        /// <summary>
        /// Subscribes to a topic
        /// </summary>
        /// <returns>An awaitable task</returns>
        public Task SubscribeAsync()
        {
            return _md.SendAsync(new
                {
                    op = "subscribe",
                    id = _uid,
                    topic = Topic,
                    type = Type
                });
        }

        /// <summary>
        /// Unsubscribes to a topic
        /// </summary>
        /// <returns>An awaitable task</returns>
        public Task UnsubscribeAsync()
        {
            return _md.SendAsync(new
                {
                    op = "unsubscribe",
                    id = _uid,
                    topic = Topic
                });
        }

        private void _md_MessageReceived(object sender, MessageReceivedEventArgs e)
        {
            MessageReceivedHandler tmp = MessageReceived;
            if (null != tmp)
            {
                JToken value;
                if (e.Message.TryGetValue("topic", out value))
                {
                    string topic = value.ToString();
                    if (topic.Equals(Topic))
                    {
                        tmp(this, new MessageReceivedEventArgs(e.Message));
                    }
                }
            }
        }
    }
}
