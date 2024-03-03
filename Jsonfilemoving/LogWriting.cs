using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Jsonfilemoving
{
    public class LogWriting
    {


        public void logWriting(string message) 
        {
            XmlDocument xmlDocument  = new XmlDocument();
            xmlDocument.Load("Config.xml");
            XmlNode fileSettingsNode = xmlDocument.SelectSingleNode("Configuration/fileSettings");

            string logPath     = fileSettingsNode.SelectSingleNode("logPath").InnerText;
            string currentDate = DateTime.Now.ToString("yyyMMdd");
            string currentdate = DateTime.Now.ToString("G");

            string logpath     = $"{logPath}log_{currentDate}.log";

            using (StreamWriter writer = File.AppendText(logpath))
            {
                writer.WriteLine(currentdate + message);
            }
        }
    }
}
