//using System;
//using System.Linq;
//using Delen.Core;
//using Delen.Core.Entities;
//using Delen.Server.Tests.Common.RavenDB;
//using FluentAssertions;
//using NUnit.Framework;

//namespace Delen.Server.Tests.Functional
//{ 
//    public class RepositoryFixture : DocumentStoreFixtureBase
//    {
//        [Test]
//        public void ShouldBeAbleToQueryByTask()
//        {
//            DisableDatabaseCleanUp(true);
//            var job = Job.Create(Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), Guid.NewGuid().ToString());
//            job.AddWorkItem(CreateWorkItem(job));
//            job.AddWorkItem(CreateWorkItem(job));
           
           
//            using (var session = DocumentStore.OpenSession())
//            {
//              session.Store(job);
//              session.SaveChanges();
//            }

//            using (var session = DocumentStore.OpenSession())
//            {
//                var job2 = session.Query<Job>().First(q => q.WorkItems.Any(w => w.Status==(WorkItemStatus.Pending)));
//                job2.WorkItems.Count().Should().Be(2);
//            }

            
           
//        }

//        private static WorkItem CreateWorkItem(Job job)
//        {
//            var workItem= new WorkItem("runner", "arguments",job);
//            workItem.SetStatus(WorkItemStatus.Pending);
//            return workItem;
//        }
//    }
//}