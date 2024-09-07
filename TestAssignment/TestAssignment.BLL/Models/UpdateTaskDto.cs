using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using TestAssignment.DAL.Enums;

namespace TestAssignment.BLL.Models
{
    public class UpdateTaskDto
    {
        [Required(ErrorMessage = "The title is required.")]
        [StringLength(100, ErrorMessage = "The title cannot be longer than 100 characters.")]
        public required string Title { get; set; }

        [StringLength(500, ErrorMessage = "The description cannot be longer than 500 characters.")]
        public string? Description { get; set; }

        [DataType(DataType.DateTime, ErrorMessage = "Invalid date format.")]
        public DateTime? DueDate { get; set; }

        [Required(ErrorMessage = "The status is required.")]
        [EnumDataType(typeof(StatusEnum), ErrorMessage = "Invalid status value.")]
        public StatusEnum Status { get; set; }

        [Required(ErrorMessage = "The priority is required.")]
        [EnumDataType(typeof(PriorityEnum), ErrorMessage = "Invalid priority value.")]
        public PriorityEnum Priority { get; set; }
    }
}
