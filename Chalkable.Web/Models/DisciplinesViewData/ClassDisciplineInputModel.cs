using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Chalkable.Common;

namespace Chalkable.Web.Models.DisciplinesViewData
{
    public class ClassDisciplineInputModel
    {
        public int? Id { get; set; }
        public int? ClassId { get; set; }
        public int StudentId { get; set; }

        public DateTime Date { get; set; }
        public string Description { get; set; }

        public IntList InfractionsIds { get; set; } 
    }
}