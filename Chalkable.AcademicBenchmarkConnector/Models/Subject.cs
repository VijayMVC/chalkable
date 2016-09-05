using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using Newtonsoft.Json;

namespace Chalkable.AcademicBenchmarkConnector.Models
{
    public class SubjectWrapper
    {
        [JsonProperty("subject")]
        public Subject Subject { get; set; }
    }

    public class Subject
    {
        [JsonProperty("code")]
        public string Code { get; set; }
        [JsonProperty("broad")]
        public string Broad { get; set; }
        [JsonProperty("descr")]
        public string Description { get; set; }

        public override bool Equals(object obj)
        {
            var o = (Subject)obj;
            return o.Code == this.Code;
        }

        public override int GetHashCode()
        {
            return Code.GetHashCode();
        }
    }

    public class SubjectDocumentWrapper
    {
        [JsonProperty("subject_doc")]
        public SubjectDocument SubjectDocument { get; set; }
    }

    public class SubjectDocument
    {
        [JsonProperty("guid")]
        public Guid Id { get; set; }
        [JsonProperty("descr")]
        public string Description { get; set; }

        public override bool Equals(object obj)
        {
            var o = (SubjectDocument) obj;
            return o.Id == this.Id;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}
