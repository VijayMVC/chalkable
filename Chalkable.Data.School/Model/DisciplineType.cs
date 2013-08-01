using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chalkable.Data.Common;

namespace Chalkable.Data.School.Model
{
    public class DisciplineType
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public int Score { get; set; }
        public const string SCORE_FIELD = "Score";
    }

    public class DisciplineTotalPerType
    {
        public Guid PersonId { get; set; }
        public int Total { get; set; }
        [DataEntityAttr]
        public DisciplineType DisciplineType { get; set; }
    }
}
