using System;
using System.Diagnostics;
using System.IO;
using Newtonsoft.Json.Linq;

namespace plugin
{
    [PluginAttribute(PluginName = "Facter")]
    class IFacter : IInputPlugin
    {
        public string Execute(JObject set)
        {
            var pathEnvVar = Environment.GetEnvironmentVariable("Path");
            var paths = pathEnvVar.Split(';');
            var fileName = "";
            foreach (var path in paths)
            {
                if (File.Exists(path + "\\facter.bat"))
                {
                    fileName = path + "\\facter.bat";
                    break;
                }
            }
            if (fileName == "")
            {
                if (Environment.UserInteractive)
                {
                    Console.WriteLine("Couldn't find facter.bat in any directory defined in Path environmental variable.");
                }
                else
                {
                    EventLog eventLog = new EventLog("Application");
                    eventLog.Source = "Winagent";
                    eventLog.WriteEntry("Facter not found", EventLogEntryType.Warning);
                }
                return "";
            }
            var proc = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = fileName,
                    Arguments = "--json",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true
                }
            };

            proc.Start();
            string result = proc.StandardOutput.ReadToEnd();
            return result;
        }
    }
}
