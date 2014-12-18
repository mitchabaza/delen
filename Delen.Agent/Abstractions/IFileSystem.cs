using System.Collections.Generic;
using System.IO;

namespace Delen.Agent.Abstractions
{
    public interface IFileSystem
    {
        bool Exists(string path);
        void ExtractZipFile(byte[] bytes, string extractFolderName);
        void DeleteFolder(string folderName);
        byte[] CreateZip(IEnumerable<FileInfo> fileInfos);
    }
}