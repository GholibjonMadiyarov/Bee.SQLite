using System.Collections.Generic;

namespace Bee.SQLite.Models
{
    public class Update
    {
        public bool execute { get; set; }
        public string message { get; set; }
        public int affectedRowCount { get; set; }
        public string queryText { get; set; }
        public Dictionary<string, object> parameter { get; set; }
        public bool exception { get; set; }
        public string stackTrace { get; set; }


        public Update()
        {
            execute = false;
            message = null;
            affectedRowCount = 0;
            exception = false;
            queryText = null;
            parameter = new Dictionary<string, object>();
            stackTrace = null;
        }
    }
}
