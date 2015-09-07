using System;
using System.Collections.Generic;
using Chalkable.Data.School.Model.Sis;

namespace Chalkable.Data.School.Model.Chlk
{
    public class ClassDiscipline
    {
        public int? Id { get; set; }
        public int? ClassId { get; set; }
        public int StudentId { get; set; }

        public DateTime Date { get; set; }
        public string Description { get; set; }

        public IList<Infraction> Infractions { get; set; } 
    }

    public class ClassDisciplineDetails : ClassDiscipline
    {
        public StudentDetails Student { get; set; }
        public Class Class { get; set; }
    }
}
