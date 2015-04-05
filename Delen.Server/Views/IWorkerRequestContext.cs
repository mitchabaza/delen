using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Delen.Server.Views
{
    public interface IWorkerRequestContext
    {
        Guid? Token { get; }
    }
}