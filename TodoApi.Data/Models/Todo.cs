namespace TodoApi.Data.Models
{
    public class Todo
    {
        public long Id { get; set; }
        public string Description { get; set; }
        public bool IsCompleted { get; set; }

        public TodoUser User { get; set; }
    }
}
