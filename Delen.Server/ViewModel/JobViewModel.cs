using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Delen.Core.Entities;
using Microsoft.WindowsAzure.Storage.Blob.Protocol;

namespace Delen.Server.ViewModel
{
    public class JobViewModel
    {
        public JobViewModel(Job job)
        {
            InitiatedBy = job.InitiatedBy;
            Runner = job.Runner;
            Arguments = job.Arguments;
            Id = job.Id;
            WorkItemsInt = job.WorkItems.Select(d => d.Id);
        }
        public IEnumerable<int> WorkItemsInt { get; set; }
        public string InitiatedBy { get; set; }
        public string Runner { get; set; }
        public string Arguments { get; set; }
        public IEnumerable<WorkItem> WorkItems { get; set; }
        public int Id { get; set; }
    }
}