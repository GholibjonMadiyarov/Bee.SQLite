using System.Collections.Generic;

namespace Bee.SQLite.Models
{
    public class SelectString
    {
        public bool execute { get; set; }
        public string message { get; set; }
        public List<Dictionary<string, string>> data { get; set; }

        public SelectString() 
        {
            execute = false;
            message = null;
            data = new List<Dictionary<string, string>>();
        }
    }
}
