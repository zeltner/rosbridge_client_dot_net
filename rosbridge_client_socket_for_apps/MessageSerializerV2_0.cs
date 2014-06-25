using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rosbridge.Client
{
    public class MessageSerializerV2_0 : IMessageSerializer
    {
        /// <summary>
        /// The rosbridge protocol version of this serializer
        /// </summary>
        public string ProtocolVersion { get; private set; }

        /// <summary>
        /// Serialize a message to a buffer
        /// </summary>
        /// <param name="message">The message to serialize</param>
        /// <returns></returns>
        public byte[] Serialize(dynamic message)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Deserialize a buffer and returns a messages.
        /// </summary>
        /// <param name="buffer">The buffer to deserialize</param>
        /// <returnsThe message</returns>
        public JObject Deserialize(byte[] buffer)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Constructs a message serializer for the rosbridge v2.0
        /// </summary>
        public MessageSerializerV2_0()
        {
            ProtocolVersion = "rosbridge v2.0";
        }
    }
}
