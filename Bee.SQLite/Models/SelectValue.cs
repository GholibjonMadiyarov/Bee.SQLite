namespace Bee.SQLite.Models
{
    public class SelectValue
    {
        public bool execute { get; set; }
        public string message { get; set; }
        public object value { get; set; }
        public bool read { get; set; }
        public bool exception { get; set; }

        public SelectValue() 
        {
            execute = false;
            message = null;
            value = null;
            read = false;
            exception = false;
        }
    }
}
