using System;
using System.IO;

namespace Delen.Server.Controllers
{
    public class TempFile : IDisposable
    {
        private readonly bool _preserveFiles;
        private readonly Stream _stream;
        public string FileName { get; private set; }

        public TempFile()
            : this(Path.GetTempPath())
        {
        }

        

        public Stream Stream
        {
            get { return _stream; }
        }

        public TempFile(string root, bool preserveFiles = false)
        {
            _preserveFiles = preserveFiles;
            this.FileName = Path.Combine(root, Path.GetRandomFileName());
          _stream=  File.Create(this.FileName);
        }


        public override string ToString()
        {
            return FileName;
        }


        public void Dispose()
        {
            _stream.Dispose();
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