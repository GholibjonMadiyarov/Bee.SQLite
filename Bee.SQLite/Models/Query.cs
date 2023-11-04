namespace Bee.SQLite.Models
{
    public class Query
    {
        public bool execute { get; set; }
        public string message { get; set; }
        public int result { get; set; }
        public bool dublicate { get; set; }

        public Query() 
        {
            execute = false;
            message = null;
            result = -1;
            dublicate = false;
        }
    }
}
