using System;
using System.Linq.Expressions;
using System.Net;
using Newtonsoft.Json;

namespace Delen.Common.Serialization
{
    public static class JsonNetHelper
    {
        private static SerializationOptions _options;
 
        public static string Serialize(object payload)
        {
          
            return JsonConvert.SerializeObject(payload, GetSerializerSettings());
        }

        public static object Deserialize(string json)
        {
            try
            {
                return JsonConvert.DeserializeObject(json, GetDeserializerSettings());
            }
            catch (JsonSerializationException e)
            {
                throw new JsonSerializationException("", e);
            }
        }
        public static T Deserialize<T>(string json)
        {
            try
            {
                return JsonConvert.DeserializeObject<T>(json, GetDeserializerSettings());
            }
            catch (JsonSerializationException e)
            {
                throw new JsonSerializationException("",e);
            }
        }
        private static SerializationOptions Options
        {
            get
            {
                if (_options == null)
                {
                    _options = new SerializationOptions();
                    _options.Ignored.Add(new IgnoredProperty() {OwningType = typeof (IPAddress), Name = "ScopeId"});
                }
                return _options;
            }
        }

        private static JsonSerializerSettings GetSerializerSettings()
        {
            var jsonResolver = new IgnorableSerializerContractResolver();


            foreach (var ignored in Options.Ignored)
            {
                jsonResolver.Ignore(ignored.OwningType, ignored.Name);
            }

            // ignore single property
            return new JsonSerializerSettings()
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                ContractResolver = jsonResolver,
                TypeNameHandling = TypeNameHandling.All,

                MissingMemberHandling = MissingMemberHandling.Error,
                TypeNameAssemblyFormat = System.Runtime.Serialization.Formatters.FormatterAssemblyStyle.Simple
            };
        }

        private static JsonSerializerSettings GetDeserializerSettings()
        {
            var jsonResolver = new IgnorableSerializerContractResolver();


            foreach (var ignored in Options.Ignored)
            {
                jsonResolver.Ignore(ignored.OwningType, ignored.Name);
            }

            // ignore single property
            return new JsonSerializerSettings()
            {
                 
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                ContractResolver = jsonResolver,
                TypeNameHandling = TypeNameHandling.Objects
                
            };
        }
    }
}