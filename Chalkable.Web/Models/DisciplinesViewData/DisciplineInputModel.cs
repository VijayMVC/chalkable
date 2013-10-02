﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Chalkable.Common;

namespace Chalkable.Web.Models.DisciplinesViewData
{
    public class DisciplineInputModel
    {
        public Guid ClassPersonId { get; set; }
        public Guid ClassPeriodId { get; set; }
        public DateTime Date { get; set; }
        public GuidList DisciplineTypeIds { get; set; }
        public string Description { get; set; }
    }

    public class DisciplineListInputModel
    {
        public IList<DisciplineInputModel> Disciplines { get; set; }
    }
}