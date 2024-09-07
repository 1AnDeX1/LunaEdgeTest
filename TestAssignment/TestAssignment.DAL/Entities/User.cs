﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestAssignment.DAL.Entities
{
    public class User
    {
        public Guid Id { get; set; }

        public required string Username { get; set; }

        public required string Email { get; set; }

        public required string PasswordHash { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<UserTask>? Tasks { get; set; }
    }
}
