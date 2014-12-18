using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Delen.Runners
{
    public class NUnitJobCommand
    {
        private readonly string[] _assemblies;
        private readonly byte[] _archive;

        public NUnitJobCommand(string[] assemblies, byte[] archive)
        {
            _assemblies = assemblies;
            _archive = archive;
        }
    }
}
