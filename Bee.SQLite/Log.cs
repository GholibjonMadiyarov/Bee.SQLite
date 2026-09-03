using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Bee.SQLite
{
    internal class Log
    {
        public static void warning(string path, string data)
        {
            try
            {
                using (StreamWriter streamWriter = File.AppendText(path))
                    streamWriter.WriteLine("[" + System.DateTime.Now.ToString() + "] [Warning] " + data);
            }
            catch
            {

            }
        }

        public static void error(string path, string data)
        {
            try
            {
                using (StreamWriter streamWriter = File.AppendText(path))
                    streamWriter.WriteLine("[" + System.DateTime.Now.ToString() + "] [Error] " + data);
            }
            catch
            {

            }
        }

        public static void info(string path, string data)
        {
            try
            {
                using (StreamWriter streamWriter = File.AppendText(path))
                    streamWriter.WriteLine("[" + System.DateTime.Now.ToString() + "] [Info] " + data);
            }
            catch
            {

            }
        }
    }
}
