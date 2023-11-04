namespace Bee.SQLite.Models
{
    public class Update
    {
        public bool execute { get; set; }
        public string message { get; set; }
        public int affectedRowCount { get; set; }

        public Update() 
        {
            execute = false;
            message = null;
            affectedRowCount = 0;
        }
    }
}
