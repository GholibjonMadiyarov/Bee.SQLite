namespace Bee.SQLite.Models
{
    public class SelectValueInteger
    {
        public bool execute { get; set; }
        public string message { get; set; }
        public int? value { get; set; }
        public bool read { get; set; }
        public bool exception { get; set; }

        public SelectValueInteger() 
        {
            execute = false;
            message = null;
            value = null;
            read = false;
            exception = false;
        }
    }
}
