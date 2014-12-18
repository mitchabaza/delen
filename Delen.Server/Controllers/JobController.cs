using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Delen.Core;
using Delen.Core.Communication;
using Delen.Core.Entities;
using Delen.Core.Persistence;
using Delen.Core.Services;
using Delen.Server.ViewModel;

namespace Delen.Server.Controllers
{
    public class JobController : BaseController
    {
        private readonly IJobQueue _queue;
        private readonly IRepository _repository;


        public JobController(IJobQueue queue, IRepository repository)
        {
            _queue = queue;
            _repository = repository;
        }

        public ActionResult List()
        {
            return View("List", _repository.Query<Job>().Select(s => s).ToList());
        }
        public ActionResult Get(int id)
        {
            return Json(_repository.Get<Job>(id));
        }
        public ActionResult GetArtifacts(int id)
        {
             var workItems = _repository.Query<WorkItem>().Where(w => w.Job.Id == id).ToList();
            if (workItems.Count == 0)
            {
                return Json(string.Format("No Job found with Id {0}", id));
            }
            using (var memoryStream = new MemoryStream())
            {
                using (var ziparchive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
                {
                    int i = 0;
                    foreach (var workItem in workItems)
                    {
                        i++;
                        var entry = ziparchive.CreateEntry(string.Format("{0}.zip", i));
                        using (var writer = new BinaryWriter(entry.Open()))
                        {
                            writer.Write(workItem.Artifacts);
                        }
                    }
                 }

                return File(memoryStream.ToArray(), "application/zip", string.Format("Artifacts_Job{0}.zip", id));
            }
        }

   
        public ActionResult Index()
        {
            return RedirectToAction("List");
        }

        public ActionResult Queue(AddJobRequest cmd)
        {
            return Json(_queue.Queue(cmd));
        }

        public ActionResult Cancel(CancelJobRequest cmd)
        {
            return Json(_queue.Cancel(cmd));
        }
    }

}