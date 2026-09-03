namespace Bee.SQLite.Models
{
    public class Delete
    {
        public bool execute { get; set; }
        public string message { get; set; }
        public string stackTrace { get; set; }
        public int affectedRowCount { get; set; }

        public Delete() 
        {
            execute = false;
            message = null;
            stackTrace = null;
            affectedRowCount = 0;
        }
    }
}
