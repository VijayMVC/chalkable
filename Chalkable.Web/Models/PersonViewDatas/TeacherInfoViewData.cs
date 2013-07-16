using Chalkable.Data.School.Model;


namespace Chalkable.Web.Models.PersonViewDatas
{
    public class TeacherInfoViewData : PersonInfoViewData
    {
        protected TeacherInfoViewData(PersonDetails person)
            : base(person)
        {
            Salutation = person.Salutation;
            FullName = person.SalutationName;
            DisplayName = person.ShortSalutationName;
        }
        public static new  TeacherInfoViewData Create(PersonDetails person)
        {
            return new TeacherInfoViewData(person);
        }
    }
}