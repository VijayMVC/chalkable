﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Chalkable.Common;
using Chalkable.Data.School.Model;

namespace Chalkable.Web.Models.PersonViewDatas
{
    public class PersonViewData : ShortPersonViewData
    {
        public DateTime? BirthDate { get; set; }
        public string Salutation { get; set; }
        public bool Active { get; set; }
        public string Email { get; set; }
        public RoleViewData Role { get; set; }

        protected PersonViewData(Person person): base(person)
        {
            BirthDate = person.BirthDate;
            Salutation = person.Salutation;
            Active = person.Active;
            Email = person.Email;
            Role = RoleViewData.Create(CoreRoles.GetById(person.RoleRef));
        }

        public static new PersonViewData Create(Person person)
        {
            return new PersonViewData(person);
        }
        public static IList<PersonViewData> Create(IList<Person> person)
        {
            return person.Select(Create).ToList();
        }
    }
}