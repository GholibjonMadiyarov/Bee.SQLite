using System.Collections.Generic;

namespace Bee.SQLite.Models
{
    public class Select
    {
        public bool execute { get; set; }
        public string message { get; set; }
        public string queryText { get; set; }
        public List<Dictionary<string, object>> data { get; set; }

        public Select() 
        {
            execute = false;
            message = null;
            queryText = null;
            data = new List<Dictionary<string, object>>();
        }
    }
}
