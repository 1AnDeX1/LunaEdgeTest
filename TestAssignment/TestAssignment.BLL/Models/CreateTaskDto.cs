using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestAssignment.DAL.Enums;

namespace TestAssignment.BLL.Models
{
    public class CreateTaskDto
    {
        public required string Title { get; set; }
        public string? Description { get; set; }
        public DateTime? DueDate { get; set; }
        public StatusEnum Status { get; set; }
        public PriorityEnum Priority { get; set; }
    }
}
