﻿using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Chalkable.AcademicBenchmarkConnector.Models
{
    public class Topic
    {
        [JsonProperty("guid")]
        public Guid Id { get; set; }
        [JsonProperty("descr")]
        public string Description { get; set; }
        [JsonProperty("deepest")]
        public string Deepest { get; set; }
        [JsonProperty("level")]
        public short Level { get; set; }
        [JsonProperty("status")]
        public string Status { get; set; }
        [JsonProperty("parent")]
        public GuidIdDto Parent { get; set; }
        [JsonProperty("course")]
        public Course Course { get; set; }
        [JsonProperty("subject_doc")]
        public SubjectDocument SubjectDocument { get; set; }
        public bool IsDeepest => Deepest == "Y";
        public bool IsActive => Status == "Active";
    }

    public class TopicStandards
    {
        [JsonProperty("topic")]
        public Topic Topic { get; set; }
        [JsonProperty("standards")]
        public IEnumerable<Standard> Standards { get; set; }
    }
}
