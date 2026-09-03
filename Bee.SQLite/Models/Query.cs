namespace Bee.SQLite.Models
{
    public class Query
    {
        public bool execute { get; set; }
        public string message { get; set; }
        public string stackTrace { get; set; }
        public int result { get; set; }
        public bool duplicate { get; set; }

        public Query() 
        {
            execute = false;
            message = null;
            stackTrace = null;
            duplicate = false;
        }
    }
}
