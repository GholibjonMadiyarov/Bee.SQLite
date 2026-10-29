using System.Collections.Generic;

namespace Bee.SQLite.Models
{
    public class Insert
    {
        public bool execute { get; set; }
        public string message { get; set; }
        public bool duplicate { get; set; }
        public bool exception { get; set; }
        public string exceptionType { get; set; }
        public int? exceptionNumber { get; set; }

        public long? insertedId { get; set; }
        public List<long?> insertedIds { get; set; }
        public string queryText { get; set; }
        public string stackTrace { get; set; }

        public Insert()
        {
            execute = false;
            message = null;
            duplicate = false;
            exception = false;
            exceptionType = null;
            exceptionNumber = null;
            insertedId = null;
            insertedIds = new List<long?>();
            queryText = null;
            stackTrace = null;
        }
    }
}
