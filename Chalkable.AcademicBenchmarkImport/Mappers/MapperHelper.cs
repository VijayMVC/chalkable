using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Configuration;
using System.Text;
using System.Threading.Tasks;
using Chalkable.Common;
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

        public static Standard Map(AcademicBenchmarkConnector.Models.Standard model)
        {
            return new Standard
            {
                Id = model.Id,
                Description = model.Description,
                IsActive = model.IsActive,
                DocumentRef = model.Document.Id,
                CourseRef = model.Course.Id,
                IsDeepest = model.IsDeepest,
                AuthorityRef = model.Authority.Id,
                ParentRef = model.Parent?.Id,
                AdoptYear = int.Parse(model.AdoptYear),
                DateModified = model.DateModified,
                GradeLevelHiRef = model.GradeLevel.High,
                GradeLevelLoRef = model.GradeLevel.Low,
                Label = model.Label,
                Level = model.Level,
                Number = model.Number,
                Seq = model.Seq,
                SubjectDocRef = model.SubjectDocument.Id,
                // We need broad, because subjects have sub tree, and we don't 
                // know about child subjects anything. So we take parent subject
                SubjectRef = model.Subject.Broad, //!!!
                Version = model.Version
            };
        }

        public static IList<StandardDerivative> Map(AcademicBenchmarkConnector.Models.StandardRelations model)
        {
            return model?.Relations?.Derivatives?.Select(x => new StandardDerivative
            {
                StandardRef = model.Data.Id,
                DerivativeRef = x.Standard.Id
            }).ToList();
        }

        public static Topic Map(AcademicBenchmarkConnector.Models.Topic model)
        {
            return new Topic
            {
                Id = model.Id,
                Description = model.Description,
                IsActive = model.IsActive,
                CourseRef = model.Course.Id,
                IsDeepest = model.IsDeepest,
                ParentRef = model.Parent?.Id,
                AdoptYear = int.Parse(model.AdoptYear),
                // We need broad, because subjects have sub tree, and we don't 
                // know about child subjects anything. So we take parent subject
                SubjectRef = model.Subject.Broad, //!!!
                GradeLevelHiRef = model.GradeLevel.High,
                GradeLevelLoRef = model.GradeLevel.Low,
                Level = model.Level,
                SubjectDocRef = model.SubjectDocument.Id
            };
        }
    }
}
