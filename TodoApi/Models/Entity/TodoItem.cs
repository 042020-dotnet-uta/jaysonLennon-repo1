using System.Collections.Generic;

namespace TodoApi.Data
{
    public class TodoItem
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public bool IsComplete { get; set; }
        public string Secret { get; set; }
        public virtual List<DoNotDoItem> DoNotDoItems { get; set; }
    }
}