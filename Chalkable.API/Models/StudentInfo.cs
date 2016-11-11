﻿using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Chalkable.API.Models
{
    public class GradeLevelInfo
    {
        [JsonProperty("id")]
        public int Id { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
    }

    public class StudentInfo
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("gradelevel")]
        public GradeLevelInfo GradeLevel { get; set; }

        [JsonProperty("age")]
        public int Age { get; set; }

        [JsonProperty("birthday")]
        public DateTime Birthday { get; set; }

        [JsonProperty("salutation")]
        public string Salutation { get; set; }

        [JsonProperty("displayname")]
        public string DisplayName { get; set; }

        [JsonProperty("firstname")]
        public string FirstName { get; set; }

        [JsonProperty("lastname")]
        public string LastName { get; set; }

        [JsonProperty("fullname")]
        public string FullName { get; set; }

        [JsonProperty("gender")]
        public string Gender { get; set; }

        [JsonProperty("schoolid")]
        public int SchoolId { get; set; }
        
        [JsonProperty("role")]
        public CoreRole Role { get; set; }

        [JsonProperty("ishispanic")]
        public bool IsHispanic { get; set; }

        [JsonProperty("ethnicity")]
        public Ethnicity Ethnicity { get; set; }

        [JsonProperty("isiepactive")]
        public bool SpecialEducation { get; set; }

        [JsonProperty("limitedenglishid")]
        public int? LimitedEnglishId { get; set; }
    }
}