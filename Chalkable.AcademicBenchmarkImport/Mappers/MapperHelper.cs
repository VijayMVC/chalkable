using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Configuration;
using System.Text;
using System.Threading.Tasks;
using Chalkable.Data.AcademicBenchmark.Model;

namespace Chalkable.AcademicBenchmarkImport.Mappers
{

    public static class MapperHelper
    {
        public static Authority Map(AcademicBenchmarkConnector.Models.Authority model)
        {
            return new Authority
            {
                Id = model.Id,
                Code = model.Code,
                Description = model.Description
            };
        }

        public static Course Map(AcademicBenchmarkConnector.Models.Course model)
        {
            return new Course
            {
                Id = model.Id,
                Description = model.Description
            };
        }

        public static GradeLevel Map(AcademicBenchmarkConnector.Models.GradeLevel model)
        {
            return new GradeLevel
            {
                Code = model.Code,
                Description = model.Description,
                Low = model.Low,
                High = model.High
            };
        }

        public static Document Map(AcademicBenchmarkConnector.Models.Document model)
        {
            return new Document
            {
                Id = model.Id,
                Title = model.Title,
                Code = model.Code
            };
        }

        public static Subject Map(AcademicBenchmarkConnector.Models.Subject model)
        {
            return new Subject
            {
                Description = model.Description,
                Code = model.Code,
                Broad = model.Broad
            };
        }

        public static SubjectDoc Map(AcademicBenchmarkConnector.Models.SubjectDocument model)
        {
            return new SubjectDoc
            {
                Description = model.Description,
                Id = model.Id
            };
        }
    }
}
