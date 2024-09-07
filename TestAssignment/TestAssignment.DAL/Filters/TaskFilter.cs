using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestAssignment.DAL.Enums;

namespace TestAssignment.Dal.Filters
{
    public class TaskFilter
    {
        public StatusEnum? Status { get; set; }
        public DateTime? DueDate { get; set; }
        public PriorityEnum? Priority { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
