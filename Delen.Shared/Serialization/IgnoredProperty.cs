using System;

namespace Delen.Common.Serialization
{
    public class IgnoredProperty
    {
        public Type OwningType { get; set; }
        public string Name { get; set; }
    }
}