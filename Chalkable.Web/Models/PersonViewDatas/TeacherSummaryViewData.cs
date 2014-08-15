using Chalkable.Data.School.Model;

namespace Chalkable.Web.Models.PersonViewDatas
{
    public class TeacherSummaryViewData : PersonSummaryViewData
    {
        protected TeacherSummaryViewData(Person person, Room room) : base(person, room)
        {
        }
        public static TeacherSummaryViewData Create(Person person, Room room)
        {
            return new TeacherSummaryViewData(person, room);
        }
    }
}