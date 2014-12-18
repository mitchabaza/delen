using Delen.Agent.Communication;
using Delen.Core.Communication;
using Delen.Core.Entities;
using Delen.Server.Tests.Common;
using Delen.Test.NUnitSample;
using FluentAssertions;
using NUnit.Framework;
using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading;
using System.Xml.Linq;
using ServerChannel = Delen.Server.Tests.Common.ServerChannel;

namespace Delen.Agent.Tests.Functional
{
    public class WorkFixture : AgentFixtureBase
    {

        private const string FullAssemblyName = BaseFixture.AssemblyName + ".dll";

        [Test]
        public void Agent_ShouldExecuteTestsAndReturnArtifacts()
        {
            const string runner = @"C:\Program Files (x86)\NUnit-2.6.2\bin\nunit-console-x86.exe";
            string arguments = string.Format(@"{0} ", FullAssemblyName);


            var workDirectoryArchive = CreateWorkDirectoryArchive(@"..\..\..\..\Delen.Test.NUnitSample\bin\Debug\");
            var command = new AddJobRequest
            {
                Arguments = arguments,
                InitiatedBy = "Me",
                Runner = runner,
                WorkDirectoryArchive = File.ReadAllBytes(workDirectoryArchive),
                ArtifactSearchFilter = "TestResult.xml"
            };
            var response =
                ServerChannel.SendRequest<CreateEntityResponse<int>>(
                    new UriFactoryForTesting(Settings.Default).Create(EndPoint.QueueJob), command);
           
            var jobProcessed = false;

            DocumentStore.Changes().ForDocumentsStartingWith("workitem").Subscribe(
                new DocumentObserver(a =>
                {
                    using (var session = DocumentStore.OpenSession())
                    {
                        var workItem = session.Query<WorkItem>().First(w => w.Job.Id.Equals(response.EntityIdentifier));
                        if (workItem.Status == WorkItemStatus.Successful)
                        {
                            jobProcessed = true;
                        }
                    }
                }));

            DocumentStore.Changes().WaitForAllPendingSubscriptions();
            StartAgent();

            WaitUntil(() => jobProcessed == false, () => AgentHasBeenRunningFor > TimeSpan.FromSeconds(15));

              using (var session = DocumentStore.OpenSession())
            {
                var workitem = session.Query<WorkItem>().First(s => s.Job.Id.Equals(response.EntityIdentifier));
                AssertArtifactIsCorrect(workitem);
               
                
                
            }
        }

        private void AssertArtifactIsCorrect(WorkItem workitem)
        {
            using (var archive = new ZipArchive(new MemoryStream(workitem.Artifacts)))
            {
                using (var stream = archive.Entries.First(f => f.Name.Equals("TestResult.xml")).Open())
                {
                    var xDocument = XDocument.Load(stream);
                    xDocument.Root.Name.ToString().Should().Be("test-results");

                }
            }
        }

        private void WaitUntil(Func<bool> condition, Func<bool> timeout)
        {
            while (condition())
            {
                if (!timeout())
                {
                    Thread.Sleep(300);
                }
                else
                {
                    throw new TimeoutException("Operation took too long to execute");
                }
            }
        }

        public static string CreateWorkDirectoryArchive(string path)
        {
            var zipPath = CurrentDirectory + @"\" +
                          BaseFixture.AssemblyName + ".zip";

            var folder = CurrentDirectory +
                         path;

            var files = Directory.GetFiles(folder);
            using (var zipToOpen = File.Create(zipPath))
            {
                using (var archive = new ZipArchive(zipToOpen, ZipArchiveMode.Update))
                {
                    foreach (var file in files)
                    {
                        var fileInfo = new FileInfo(file);
                        ZipArchiveEntry entry = archive.CreateEntry(fileInfo.Name);

                        using (var writer = new BinaryWriter(entry.Open()))
                        {
                            writer.Write(File.ReadAllBytes(fileInfo.FullName));
                        }
                    }
                }
            }
            return zipPath;
        }
    }
}