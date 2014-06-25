using Newtonsoft.Json;
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
        /// <returns>A buffer containing the serialized message</returns>
        /// <exception cref=" System.ArgumentNullException">message is null</exception>
        public byte[] Serialize(dynamic message)
        {
            if (null == message)
            {
                throw new ArgumentNullException("message");
            }

            string json = JsonConvert.SerializeObject(message);
            return Encoding.ASCII.GetBytes(json);
        }

        /// <summary>
        /// Deserialize a buffer and returns a messages.
        /// </summary>
        /// <param name="buffer">The buffer to deserialize</param>
        /// <returnsThe message</returns>
        public JObject Deserialize(byte[] buffer)
        {
            if (null == buffer)
            {
                throw new ArgumentNullException("buffer");
            }

            string json = Encoding.ASCII.GetString(buffer, 0, buffer.Length);
            return JObject.Parse(json);
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
