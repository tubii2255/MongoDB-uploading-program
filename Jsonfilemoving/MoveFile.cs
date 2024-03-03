using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;


namespace Jsonfilemoving
{
     class MoveFiles
    {
        public void MoveFile(string sourcepath,string destination) 
        {
            string message = "";            

            try
            {
                if (File.Exists(sourcepath))
                {
                    File.Move(sourcepath, destination);
                    message = ($" {sourcepath} File moved successfully to {destination}");
                }
                else
                {
                    message = (sourcepath + "does not exist");
                }
             }
            catch (Exception  ex)
            {
                message = ($" Error moving file: {ex.Message}");
            }

            LogWriting Log = new LogWriting();
            Log.logWriting(message);
        }
    }
}
