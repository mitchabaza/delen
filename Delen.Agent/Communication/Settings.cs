using System;
using System.IO;
using System.Linq;
using Delen.Common;

namespace Delen.Agent.Communication
{
    public class Settings
    {
        public Settings(int port, string server, string application, string workingDirectory)
        {
            Server = server;
            Application = application;
            WorkingDirectory =  Path.Combine(Path.GetDirectoryName(
                System.Reflection.Assembly.GetExecutingAssembly().Location),workingDirectory.Replace(@"\",""));
            Port = port;
        }
        public string Application { get; private set; }
        public string Server { get; private set; }
        public int Port { get; private set; }
        public string WorkingDirectory { get;private  set; }



        public static Settings FromFile(string file)
        {
            using (var textReader = File.OpenText(file))
            {
                var text = textReader.ReadToEnd();
                var lines = text.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
                var dict = lines.Select(line => line.Split(new[] { "=" }, StringSplitOptions.RemoveEmptyEntries)).ToDictionary(keyValue => keyValue[0], keyValue => keyValue[1]);
                return new Settings(Convert.ToInt32(dict["port"]), dict["server"], dict["appName"], dict["workingDirectory"]);
            }
        }
        public static Settings Default
        {
            get { return new Settings(80, "localhost", ServerConfiguration.WebApplication.Name,"work"); }
        }

        
    }
}