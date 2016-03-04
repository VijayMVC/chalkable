﻿using Newtonsoft.Json;

namespace Chalkable.AcademicBenchmarkConnector.Models
{
    public class RelatedStandard
    {
        [JsonProperty("data")]
        public Standard Data { get; set; }
        [JsonProperty("relations")]
        public StandardRelations Relations { get; set; }
    }

    public class StandardRelations
    {
        [JsonProperty("origin")]
        public StandardRelations Origin { get; set; }
        [JsonProperty("derivative")]
        public StandardRelations Derivative { get; set; }
        [JsonProperty("related_derivative")]
        public StandardRelations RelatedDerivative { get; set; }
    }

    public class StandardRelation
    {
        //TODO ensure do we need these properties
        //[JsonProperty("same_concept")]
        //public string SameConcept { get; set; }
        //[JsonProperty("same_text")]
        //public string SameText { get; set; }

        [JsonProperty("standard")]
        public Standard Standard { get; set; }
    }
}
