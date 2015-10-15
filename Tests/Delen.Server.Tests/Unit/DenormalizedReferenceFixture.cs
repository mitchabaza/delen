using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Delen.Core.Entities;
using Delen.Server.Tests.Common;
using Delen.Test.Common;
using FluentAssertions;
using NUnit.Framework;
using Ploeh.AutoFixture;

namespace Delen.Server.Tests.Unit
{
    [TestFixture]
    [Category("Unit")]

    public class DenormalizedReferenceFixture:FixtureBase
    {
        [Test]
        public void ShouldMaintainLinkWithOriginalDocumentId()
        {
            var job = AutoFixture.Create<Job>();
            var workItem = new WorkItem(job);

            job.Id = 30912831;
            workItem.Job.Id.Should().Be(job.Id);
        }
    }
}
