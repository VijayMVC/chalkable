﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chalkable.Data.Common;

namespace Chalkable.Data.School.Model
{
    public class Person
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime BirthDate { get; set; }
        public string Gender { get; set; }
        public string Salutation { get; set; }
        public bool Active { get; set; }
        public DateTime? LastPasswordReset { get; set; }
        public DateTime? FirstLoginDate { get; set; }
        public int RoleRef { get; set; }
        public DateTime? LastMailNotification { get; set; }
        public int? SisId { get; set; }
        public string Email { get; set; }

        public IList<Address> Addresses { get; set; } 
    }
}
