using System.Collections.Generic;
using Chalkable.Data.Common;

namespace Chalkable.Data.School.Model
{
    public class StandardizedTest
    {
        [PrimaryKeyFieldAttr]
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public string Description { get; set; }
        public int? GradeLevelRef { get; set; }
        public string StateCode { get; set; }
        public bool DisplayOnTranscript { get; set; }
        public string SifCode { get; set; } 
        public string NcesCode { get; set; }
    }

    public class StandardizedTestComponent
    {
         [PrimaryKeyFieldAttr]
         public int Id { get; set; }
         public int StandardizedTestRef { get; set; }
         public string Name { get; set; }
         public bool DisplayOnTranscript { get; set; }
         public string Code { get; set; }
         public string StateCode { get; set; }
         public string SifCode { get; set; }
         public string NcesCode { get; set; }
    }

    public class StandardizedTestScoreType
    {
        [PrimaryKeyFieldAttr]
        public int Id { get; set; }
        public int StandardizedTestRef { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Code { get; set; }
        public string StateCode { get; set; }
        public string SifCode { get; set; }
        public string NcesCode { get; set; }
    }

    public class StandardizedTestDetails : StandardizedTest
    {
        public IList<StandardizedTestComponent> Components { get; set; }
        public IList<StandardizedTestScoreType> ScoreTypes { get; set; } 
    }
}
