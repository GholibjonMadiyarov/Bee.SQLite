namespace Bee.SQLite.Models
{
    public class SelectValueString
    {
        public bool execute { get; set; }
        public string message { get; set; }
        public string value { get; set; }
        public bool read { get; set; }
        public bool exception { get; set; }

        public SelectValueString() 
        {
            execute = false;
            message = null;
            value = null;
            read = false;
            exception = false;
        }
    }
}
