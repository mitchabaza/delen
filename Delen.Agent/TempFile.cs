using System;
using System.IO;

namespace Delen.Agent
{
    public class TempFile : IDisposable
    {
        private readonly bool _preserveFiles;
        private bool _hasBeenCalled;
        public string FileName { get; private set; }

        public TempFile() : this(Path.GetTempPath())
        {
        }

        public Stream Stream
        {
            get
            {
                if (_hasBeenCalled)
                {
                    throw new InvalidOperationException("You already called this once moron");
                }
                _hasBeenCalled = true;
                return File.Create(this.FileName);
            }
        }

        public TempFile(string root, bool preserveFiles=false)
        {
            _preserveFiles = preserveFiles;
            this.FileName = Path.Combine(root, Path.GetRandomFileName());

        }

       
        public override string ToString()
        {
            return FileName;
        }

         
        public void Dispose()
        {
            if (_preserveFiles)
            {
                return;
            }
            try
            {
                if (File.Exists(this.FileName))
                    File.Delete(this.FileName);
            }
            catch (Exception)
            {
            }
 
        }
    }
}