using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chalkable.StiConnector.SyncModel;

namespace Chalkable.StiImport.Services.SyncModelAdapters
{
    public class HomeroomAdapter : SyncModelAdapter<Homeroom>
    {
        public HomeroomAdapter(AdapterLocator locator) : base(locator)
        {
        }

        private Data.School.Model.Homeroom Selector(Homeroom x)
        {
            return new Data.School.Model.Homeroom
            {
                Id = x.HomeroomID,
                Name = x.Name,
                SchoolYearRef = x.AcadSessionID,
                RoomRef = x.RoomID,
                TeacherRef = x.TeacherID
            };
        }

        protected override void InsertInternal(IList<Homeroom> entities)
        {
            ServiceLocatorSchool.RoomService.AddHomerooms(entities.Select(Selector).ToList());
        }

        protected override void UpdateInternal(IList<Homeroom> entities)
        {
            ServiceLocatorSchool.RoomService.EditHomerooms(entities.Select(Selector).ToList());
        }

        protected override void DeleteInternal(IList<Homeroom> entities)
        {
            ServiceLocatorSchool.RoomService.DeleteHomerooms(entities.Select(Selector).ToList());
        }

        protected override void PrepareToDeleteInternal(IList<Homeroom> entities)
        {
            ServiceLocatorSchool.RoomService.PrepareToDeleteHomerooms(entities.Select(Selector).ToList());
        }
    }
}
