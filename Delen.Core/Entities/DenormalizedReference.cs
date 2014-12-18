using System;
using Raven.Imports.Newtonsoft.Json;

namespace Delen.Core.Entities
{
    public class DenormalizedReference<T> where T : Entity
    {
        private readonly T _doc;
        private readonly int _id;

        public DenormalizedReference(T doc)
        {
            _doc = doc;
        }
        [JsonConstructor]
        public DenormalizedReference(int id)
        {
            _id= id;
        }

        public static implicit operator DenormalizedReference<T>(T doc)
        {
            return new DenormalizedReference<T>(doc);

        }
        public int Id
        {
            get { return _doc != null ? _doc.Id : _id; }
        }
    }
}