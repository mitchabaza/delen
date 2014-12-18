using System.Collections.Generic;

namespace Delen.Common.Serialization
{
    public class SerializationOptions
    {
        public SerializationOptions()
        {
            Ignored = new List<IgnoredProperty>();
        }

        public List<IgnoredProperty> Ignored { get; set; }
    }
}