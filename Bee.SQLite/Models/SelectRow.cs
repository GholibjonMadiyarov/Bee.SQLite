using System.Collections.Generic;

namespace Bee.SQLite.Models
{
    public class SelectRow
    {
        public bool execute { get; set; }
        public string message { get; set; }
        public Dictionary<string, object> result { get; set; }

        public SelectRow() 
        {
            execute = false;
            message = null;
            result = new Dictionary<string, object>();
        }
    }
}
