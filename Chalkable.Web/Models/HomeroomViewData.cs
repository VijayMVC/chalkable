using Chalkable.Data.School.Model;
using Chalkable.Web.Models.PersonViewDatas;

namespace Chalkable.Web.Models
{
    public class HomeroomViewData : IdNameViewData<int>
    {
        private HomeroomViewData(int id, string name) : base(id, name)
        {
        }

        public ShortPersonViewData Teacher { get; set; }

        public static HomeroomViewData Create(Homeroom homeroom)
        {
            return new HomeroomViewData(homeroom.Id, homeroom.Name)
            {
                Teacher = homeroom.Teacher != null ? ShortPersonViewData.Create(homeroom.Teacher) : null
            };
        }
    }
}