using System;
using System.Collections;
using System.Collections.Generic;
using Raven.Database.Plugins.Builtins;
using Validation;

namespace Delen.Core.Entities
{

    public class WorkerRegistration : Entity
    {
        private WorkerRegistration() : base()
        {
        }
        
        public static WorkerRegistration Create(string ipAddress, string name)
        {
            Requires.NotNull(ipAddress, "ipAddress");
            var worker = new WorkerRegistration(ipAddress, name) {CreationDate = DateTime.Now, Token = Guid.NewGuid()};

            return worker;
        }

        public Guid Token { get; private set; }


        private WorkerRegistration(string ipAddress, string name)
        {
            IPAddress = ipAddress;
            Name = name;
        }
        
        public string Name { get; private set; }
        public string IPAddress { get; private set; }
        public DateTime AsOf { get; internal set; }
        public IEnumerable<TaskHistory> History { get; private set; }
    }
}