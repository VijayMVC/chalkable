﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chalkable.BusinessLogic.Model.AcademicBenchmark
{
    public class SubjectDocument
    {
        public Guid Id { get; set; }
        public string Description { get; set; }
        public static SubjectDocument Create(AcademicBenchmarkConnector.Models.SubjectDocument subDoc)
        {
            return new SubjectDocument
            {
                Id = subDoc.Id,
                Description = subDoc.Description
            };
        }
    }
}
