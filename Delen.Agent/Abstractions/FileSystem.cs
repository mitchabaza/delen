using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;

namespace Delen.Agent.Abstractions
{
      public class FileSystem : IFileSystem
    {
        public bool Exists(string path)
        {
            return FileExists(path);
        }

        
        public void ExtractZipFile(byte[] bytes, string extractToFolderName)
        {
            var tempFileName = Path.GetTempFileName();
            File.WriteAllBytes(tempFileName, bytes);
            ZipFile.ExtractToDirectory(tempFileName, extractToFolderName);
    
        }

        
        public void DeleteFolder(string folderName)
        {
            if (Directory.Exists(folderName))
            Directory.Delete(folderName, true);
        }

          public byte[] CreateZip(IEnumerable<FileInfo> fileInfos)
          {
              using (var tempFile = new TempFile())
              {
                  using (var archive = new ZipArchive(tempFile.Stream, ZipArchiveMode.Update))
                  {
                      foreach (var fileInfo in fileInfos)
                      {
                          ZipArchiveEntry fileEntry = archive.CreateEntry(fileInfo.Name);

                          using (var writer = new BinaryWriter(fileEntry.Open()))
                          {
                              writer.Write(File.ReadAllBytes(fileInfo.FullName));
                          }
                      }
                  }
                  return File.ReadAllBytes(tempFile.FileName);
              }
          }

          private bool FileExists(string fileName)
        {
            if (File.Exists(fileName))
                return true;

            var values = Environment.GetEnvironmentVariable("PATH");
            return values.Split(';').Select(path => Path.Combine(path, fileName)).Any(s => File.Exists(s));
        }
    }
}