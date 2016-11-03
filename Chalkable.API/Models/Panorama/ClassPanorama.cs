using System.Collections.Generic;
using Newtonsoft.Json;

namespace Chalkable.API.Models.Panorama
{
    public class ClassPanorama
    {
        [JsonProperty("filtersettings")]
        public PanoramaSettings FilterSettings { get; set; }
        [JsonProperty("classdistributionsection")]
        public ClassDistributionSection ClassDistributionSection { get; set; }
        [JsonProperty("students")]
        public IList<StudentStandardizedTestStats> Students { get; set; }
    }

    public class ClassDistributionSection
    {
        [JsonProperty("gradeaveragedistribution")]
        public ClassDistributionStats GradeAverageDistribution { get; set; }
        [JsonProperty("absencesdistribution")]
        public ClassDistributionStats AbsencesDistribution { get; set; }
        [JsonProperty("disciplinedistribution")]
        public ClassDistributionStats DisciplineDistribution { get; set; }
    }

    public class ClassDistributionStats
    {
        [JsonProperty("classavg")]
        public decimal ClassAvg { get; set; }
        [JsonProperty("distributionstats")]
        public IList<DistributionItem> DistributionStats { get; set; }
    }
    public class DistributionItem
    {
        [JsonProperty("studentids")]
        public IList<int> StudentIds { get; set; }
        [JsonProperty("count")]
        public decimal Count { get; set; }
        [JsonProperty("summary")]
        public string Summary { get; set; }
        [JsonProperty("startinterval")]
        public decimal StartInterval { get; set; }
        [JsonProperty("endinterval")]
        public decimal EndInterval { get; set; }
    }


    public class StudentStandardizedTestStats
    {
        [JsonProperty("student")]
        public StudentInfo Student { get; set; }
    }


    public class PanoramaSettings
    {
        [JsonProperty("acadyears")]
        public IList<int> AcadYears { get; set; }
        [JsonProperty("standardizedtestfilters")]
        public IList<StandardizedTestFilter> StandardizedTestFilters { get; set; }
    }

    public class StandardizedTestFilter
    {       
    }
}
