using Chalkable.Web.Models.PersonViewDatas;

namespace Chalkable.Web.Models
{
    public class HomeroomViewData : IdNameViewData<int>
    {
        private HomeroomViewData(int id, string name) : base(id, name)
        {
        }

        public ShortPersonViewData Teacher { get; set; }

        //public static HomeroomViewData Create()
    }
}