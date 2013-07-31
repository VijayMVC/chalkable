using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Chalkable.Data.School.Model;

namespace Chalkable.Web.Models.PersonViewDatas
{
    public class PersonScheduleViewData : ShortPersonViewData
    {
        public int ClassesNumber { get; set; }
        protected PersonScheduleViewData(Person person) : base(person)
        {
        }
        public static PersonScheduleViewData Create(Person student, IList<ClassDetails> classes)
        {
            return new PersonScheduleViewData(student) { ClassesNumber = classes.Count };
        } 
    }
}