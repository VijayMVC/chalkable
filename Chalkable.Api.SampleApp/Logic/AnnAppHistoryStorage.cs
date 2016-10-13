using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Chalkable.Api.SampleApp.Logic
{
    public class AnnAppHistoryStorage
    {
        private static IList<AnnAppHistory> _histories;

        static AnnAppHistoryStorage()
        {
            if (_histories == null)
                _histories = new List<AnnAppHistory>();
        }
        public static AnnAppHistoryStorage GetStorage()
        {
            return new AnnAppHistoryStorage();
        }

        public void Add(int announcementApplicationId, Guid districtId, AnnAppStatus status)
        {
            _histories.Add(new AnnAppHistory
            {
                Id = Guid.NewGuid(),
                AnnouncementApplicationId = announcementApplicationId,
                DistrictId = districtId,
                Status = status,
                Date = DateTime.Now
            });
        }

        public IList<AnnAppHistory> GetHistory(Guid? districtId)
        {
            return districtId.HasValue ? _histories.Where(x => x.DistrictId == districtId).ToList() : _histories;
        } 
    }

    public class AnnAppHistory
    {
        public Guid Id { get; set; }
        public int AnnouncementApplicationId { get; set; }
        public AnnAppStatus Status { get; set; }
        public Guid DistrictId { get; set; }
        public DateTime Date { get; set; }
    }

    public enum AnnAppStatus
    {
        Attached = 1,
        Removed = 2
    }
}