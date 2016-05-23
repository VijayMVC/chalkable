using System.Collections.Generic;

namespace Chalkable.Data.School.Model
{
    public class StandardizedTest
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public string Description { get; set; }
        public int GradeLevelRef { get; set; }
        public string StateCode { get; set; }
        public bool DisplayOnTranscript { get; set; }
        public string SifCode { get; set; } 
        public string NcesCode { get; set; }
    }

    public class StandardizedTestComponent
    {
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
