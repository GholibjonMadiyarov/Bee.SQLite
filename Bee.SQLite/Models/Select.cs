using System.Collections.Generic;

namespace Bee.SQLite.Models
{
    public class Select
    {
        public bool execute { get; set; }
        public string message { get; set; }
        public List<Dictionary<string, object>> result { get; set; }

        public Select() 
        {
            execute = false;
            message = null;
            result = new List<Dictionary<string, object>>();
        }
    }
}
