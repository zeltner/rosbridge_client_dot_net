using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rosbridge.Client
{
    public interface IMessageSerializer
    {
        /// <summary>
        /// The rosbridge protocol version of this serializer
        /// </summary>
        string ProtocolVersion { get; }

        /// <summary>
        /// Serialize a message to a buffer
        /// </summary>
        /// <param name="message">The message to serialize</param>
        /// <returns>The serialized message</returns>
        byte[] Serialize(dynamic message);

        /// <summary>
        /// Deserialize a buffer and returns a messages.
        /// </summary>
        /// <param name="buffer">The buffer to deserialize</param>
        /// <returnsThe message</returns>
        JObject Deserialize(byte[] buffer);
    }
}
