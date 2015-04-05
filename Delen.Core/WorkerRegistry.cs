using System;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using AutoMapper;
using Delen.Common;
using Delen.Core.Communication;
using Delen.Core.Entities;
using Delen.Core.Persistence;
using Raven.Abstractions.Extensions;

namespace Delen.Core
{
    public class WorkerRegistry : IWorkerRegistry
    {
        private readonly IRepository _repository;
        private readonly IMappingEngine _mappingEngine;

        public WorkerRegistry(IRepository repository, IMappingEngine mappingEngine)
        {
            _repository = repository;
            _mappingEngine = mappingEngine;
        }

        //public Response Register(RegisterWorkerRequest registerWorkerRequest)
        //{
        //    WorkerRegistration entity = _mappingEngine.Map<RegisterWorkerRequest, WorkerRegistration>(registerWorkerRequest);
        //    var existing = FindExistingRegistration(registerWorkerRequest);
        //    if (existing != null)
        //    {
        //        existing.AsOf = DateTime.Now;
        //        existing.Activate();
        //        _repository.Put(existing);
        //        return new Response(true, string.Empty);
        //    }
        //    entity.AsOf = DateTime.Now;
        //    _repository.Put(entity);
        //    return new Response (true, string.Empty);
        //}

        public Response<Guid> Register(RegisterWorkerRequest registerWorkerRequest)
        {
        
            CleanUp(registerWorkerRequest);
            WorkerRegistration entity =
                _mappingEngine.Map<RegisterWorkerRequest, WorkerRegistration>(registerWorkerRequest);
            _repository.Put(entity);

            return new Response<Guid>(true, "", entity.Token);
        }

        private void CleanUp(RegisterWorkerRequest request)
        {
            var registrations =
            _repository.Query<WorkerRegistration>().Where(w => w.Name.Equals(request.Name));

            if (registrations.Any())
            {
                registrations.ToList().ForEach(r => _repository.Delete(r));
            }
        }


        public Response UnRegister(UnregisterWorkerRequest request)
        {
            var existing = FindExistingRegistration(request.Token);
            if (existing != null)
            {
                _repository.Delete(existing);
            }
            return new Response(true);
        }

        public WorkerRegistration GetRegistration(Guid token)
        {
            return FindExistingRegistration(token);
        }

        public bool IsRegisteredWorker(Guid token)
        {
            return FindExistingRegistration(token) != null;
        }


        private WorkerRegistration FindExistingRegistration(Guid token)
        {
            var existing =
                _repository.Query<WorkerRegistration>()
                    .FirstOrDefault(w => w.Token.Equals(token));
            return existing;
        }
    }
}