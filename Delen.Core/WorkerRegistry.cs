using System;
using System.Linq;
using System.Net;
using AutoMapper;
using Delen.Common;
using Delen.Core.Communication;
using Delen.Core.Entities;
using Delen.Core.Persistence;

namespace Delen.Core
{
    public class WorkerRegistry:IWorkerRegistry
    {
        private readonly IRepository _repository;
        private readonly IMappingEngine _mappingEngine;

        public WorkerRegistry(IRepository repository, IMappingEngine mappingEngine)
        {
            _repository = repository;
            _mappingEngine = mappingEngine;
        }

        public Response Register(RegisterWorkerRequest registerWorkerRequest)
        {
            WorkerRegistration entity =
                _mappingEngine.Map<RegisterWorkerRequest, WorkerRegistration>(registerWorkerRequest);
            var existing = FindExistingRegistration(registerWorkerRequest);
            if (existing != null)
            {
                existing.AsOf = DateTime.Now;
                existing.Activate();
                _repository.Put(existing);
                return new Response(true, string.Empty);
            }
            entity.AsOf = DateTime.Now;
            _repository.Put(entity);
            return new Response (true, string.Empty);
        }

        public Response UnRegister(UnregisterWorkerRequest unregisterWorkerRequest)
        {
            var entity = FindExistingRegistration(unregisterWorkerRequest);

            entity.Deactivate();

            _repository.Put(entity);

            return new Response(true, "Registered Worker with {Id}".FormatWith(new {entity.Id}));
        }

        public WorkerRegistration GetRegistration(IPAddress ipAddress)
        {
            return FindExistingRegistration(ipAddress.ToString());
        }

        public bool IsRegisteredWorker(IPAddress ipAddress)
        {
            return FindExistingRegistration(ipAddress.ToString()) != null;
        }

        private WorkerRegistration FindExistingRegistration(WorkerRegistrationRequestBase registerWorkerRequest)
        {
            return FindExistingRegistration(registerWorkerRequest.IPAddress);
        }
        private WorkerRegistration FindExistingRegistration(string ipaddress)
        {
            var existing =
                _repository.Query<WorkerRegistration>()
                    .FirstOrDefault(w => w.IPAddress.Equals(ipaddress));
            return existing;
        }
    }
}