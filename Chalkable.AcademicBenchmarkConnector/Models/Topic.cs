﻿using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Chalkable.AcademicBenchmarkConnector.Models
{
    public class Topic
    {
        [JsonProperty("guid")]
        public Guid Id { get; set; }
        [JsonProperty("number")]
        public string Number { get; set; }
        [JsonProperty("descr")]
        public string Description { get; set; }
        [JsonProperty("deepest")]
        public string Deepest { get; set; }
        [JsonProperty("level")]
        public short Level { get; set; }
        [JsonProperty("status")]
        public string Status { get; set; }
        [JsonProperty("parent")]
        public Parent Parent { get; set; }
        public bool IsDeepest => Deepest == "Y";
        public bool IsActive => Status == "Active";
        [JsonProperty("authority")]
        public Authority Authority { get; set; }
        [JsonProperty("document")]
        public Document Document { get; set; }
    }

    public class TopicStandards
    {
        [JsonProperty("topic")]
        public Topic Topic { get; set; }
        [JsonProperty("standards")]
        public IEnumerable<Standard> Standards { get; set; }
    }
}
