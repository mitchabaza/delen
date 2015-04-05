using System.Text;
using System.Web.Mvc;
using Delen.Core;
using Delen.Core.Communication;
using Delen.Core.Entities;
using Delen.Core.Persistence;
using Newtonsoft.Json;


namespace Delen.Server.Controllers
{
    
    public class RegistrationController : BaseController
    {
        private readonly IWorkerRegistry _registry;
        private readonly IRepository _repository;


        public RegistrationController(IWorkerRegistry registry, IRepository repository)
        {
            _registry = registry;
            _repository = repository;
        }

        [HttpPost]       
        public ActionResult Remove(UnregisterWorkerRequest request)
        {
            return Json(_registry.UnRegister(request));
        }

        [HttpGet]
        public ActionResult Index()
        {
           return RedirectToAction("List");
        }

        [HttpPost]
         public ActionResult Add(RegisterWorkerRequest request)
        {
            return Json(_registry.Register(request));
        }

        public ActionResult List()
        {
            return Json(_repository.Query<WorkerRegistration>());
        }
    }
}