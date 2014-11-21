﻿using System;
using System.Collections.Generic;
using System.Linq;
using Chalkable.Data.School.Model;

namespace Chalkable.Web.Models.PersonViewDatas
{
    public class PersonViewData : ShortPersonViewData
    {
        public DateTime? BirthDate { get; set; }
        public string Salutation { get; set; }
        public bool Active { get; set; }
        
        
        protected PersonViewData(Person person): base(person)
        {
            BirthDate = person.BirthDate;
            Salutation = person.Salutation;
            Active = person.Active;
        }

        public static new PersonViewData Create(Person person)
        {
            return new PersonViewData(person);
        }

        public static new IList<PersonViewData> Create(IList<Person> person)
        {
            return person.Select(Create).ToList();
        }
    }
}