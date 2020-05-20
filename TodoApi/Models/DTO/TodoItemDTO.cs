using System.Collections.Generic;
using TodoApi.Data;

namespace TodoApi.DTO
{
    public class TodoItemDTO
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public bool IsComplete { get; set; }
        public virtual List<DoNotDoItem> DoNotDoItems { get; set; }
    }
}