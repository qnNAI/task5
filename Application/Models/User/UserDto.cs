﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Common.Attributes;

namespace Application.Models.User
{

    public class UserDto
    {
        [SkipErrorGeneration]
        public string Id { get; set; } = null!;

        public string FirstName { get; set; } = null!;
        public string MiddleName { get; set; } = null!;
        public string LastName { get; set; } = null!;

        [SkipErrorGeneration]
        public string Gender { get; set; } = null!;
        public string Address { get; set; } = null!;
        public string Phone { get; set; } = null!;
    }
}