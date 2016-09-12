using System;
using System.Collections.Generic;
using Chalkable.Common;

namespace Chalkable.Data.Master.Model
{
    public class PictureImportTaskData
    {
        public Guid DistrictId { get; set; }
        public IList<int> PersonIds { get; set; }

        public override string ToString()
        {
            return DistrictId.ToString() + "," + PersonIds.JoinString(",");
        }

        public PictureImportTaskData(Guid districtId, IList<int> personIds)
        {
            DistrictId = districtId;
            PersonIds = personIds;
        }

        public PictureImportTaskData(string str)
        {
            var sl = str.Split(',');
            DistrictId = Guid.Parse(sl[0]);
            PersonIds = new List<int>();
            for (int i = 1; i < sl.Length; i++)
                PersonIds.Add(int.Parse(sl[i]));
        }
    }
}