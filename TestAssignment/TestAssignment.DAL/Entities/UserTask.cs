using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestAssignment.DAL.Enums;

namespace TestAssignment.DAL.Entities
{
    public class UserTask
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string? Description { get; set; }
        public DateTime? DueDate { get; set; }
        public StatusEnum Status { get; set; } = StatusEnum.Pending;
        public PriorityEnum Priority { get; set; } = PriorityEnum.Medium;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public Guid UserId { get; set; }
        public required User User { get; set; }
    }
}
