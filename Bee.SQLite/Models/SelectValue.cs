namespace Bee.SQLite.Models
{
    public class SelectValue
    {
        public bool execute { get; set; }
        public string message { get; set; }
        public string stackTrace { get; set; }
        public object value { get; set; }
        public bool read { get; set; }
        public bool exception { get; set; }

        public SelectValue() 
        {
            execute = false;
            message = null;
            stackTrace = null;
            value = null;
            read = false;
            exception = false;
        }
    }
}
