namespace Bee.SQLite.Models
{
    public class Query
    {
        public bool execute { get; set; }
        public string message { get; set; }
        public int result { get; set; }
        public bool duplicate { get; set; }

        public Query() 
        {
            execute = false;
            message = null;
            duplicate = false;
        }
    }
}
