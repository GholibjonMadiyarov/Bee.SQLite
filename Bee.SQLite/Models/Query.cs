using System.Collections.Generic;

namespace Bee.SQLite.Models
{
    public class Query
    {
        public bool execute { get; set; }
        public string message { get; set; }
        public bool duplicate { get; set; }
        public bool exception { get; set; }
        public string exceptionType { get; set; }
        public int? exceptionNumber { get; set; }

        public long? insertedId { get; set; }
        public List<long?> insertedIds { get; set; }
        public List<List<Dictionary<string, object>>> data { get; set; }
        public int affectedRowCount { get; set; }
        public string queryText { get; set; }
        public string stackTrace { get; set; }

        public Query()
        {
            execute = false;
            message = null;
            duplicate = false;
            exception = false;
            exceptionType = null;
            exceptionNumber = null;
            insertedId = null;
            insertedIds = new List<long?>();
            data = new List<List<Dictionary<string, object>>>();
            queryText = null;
            stackTrace = null;
            affectedRowCount = 0;
        }
    }
}
