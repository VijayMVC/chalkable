using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Chalkable.Data.School.Model;
using Chalkable.Data.School.Model.Sis;

namespace Chalkable.Web.Models.PersonViewDatas
{
    public class PersonScheduleViewData  
    {
        public int ClassesNumber { get; set; }
        public ShortPersonViewData ShortPersonInfo { get; set; }
        protected PersonScheduleViewData(ShortPersonViewData shortPersonData)
        {
            ShortPersonInfo = shortPersonData;
        }
        public static PersonScheduleViewData Create(ShortPersonViewData shortPersonData, IList<ClassDetails> classes)
        {
            return new PersonScheduleViewData(shortPersonData) { ClassesNumber = classes.Count };
        } 
    }
}