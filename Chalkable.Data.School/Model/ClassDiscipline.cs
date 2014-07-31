using System;
using System.Collections.Generic;

namespace Chalkable.Data.School.Model
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
        public Person Student { get; set; }
        public Class Class { get; set; }
    }
}
