using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using Delen.Core.Tasks;

namespace Delen.Agent.Tasks
{
    public class ArtifactCollector  
    {
        private readonly string[] _searchDirectories;
         private readonly string[] _searchPatterns;

        public ArtifactCollector(string[] searchDirectories, string[] searchPatterns)
        {
            _searchDirectories = searchDirectories;
            
            _searchPatterns = searchPatterns;
        }

        public IList<FileInfo> CollectArtifacts()
        {
            var foundFiles = new List<FileInfo>();

            foreach (var searchDirectory in _searchDirectories)
            {
                foreach (var searchPattern in _searchPatterns)
                {
                    IEnumerable<string> files = Directory.EnumerateFiles(searchDirectory, searchPattern, SearchOption.AllDirectories);
                    foundFiles.AddRange(files.Select(file => new FileInfo(file)));
                }
              
               
            }
            return foundFiles;
        }
    }
}