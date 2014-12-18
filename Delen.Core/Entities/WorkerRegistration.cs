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
         public static WorkerRegistration Create(string ipAddress, string host)
        {
            Requires.NotNull(ipAddress, "ipAddress");
            var worker = new WorkerRegistration(ipAddress, host);
            worker.Activate();
             worker.CreationDate = DateTime.Now;
            return worker;
        }

       
        private WorkerRegistration(string ipAddress, string host)
        {
            IPAddress = ipAddress;
            Host = host;
        }
        
        public string Host { get; private set; }
        public string IPAddress { get; private set; }
        public DateTime AsOf { get; internal set; }
        public IEnumerable<TaskHistory> History { get; private set; }
    }
}