using System.Collections.Generic;
using System.Linq;
using Chalkable.Data.School.Model;

namespace Chalkable.Web.Models
{
    public class ShortStandardizedTestViewData
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string DisplayName { get; set; }

        protected ShortStandardizedTestViewData(StandardizedTest standardizedTest)
        {
            Id = standardizedTest.Id;
            Name = standardizedTest.Name;
            DisplayName = standardizedTest.DisplayName;
        }
        public static ShortStandardizedTestViewData Create(StandardizedTest standardizedTest)
        {
            return new ShortStandardizedTestViewData(standardizedTest);
        }
    }
    public class StandardizedTestViewData : ShortStandardizedTestViewData
    {
        public IList<StandardizedTestComponentViewData> Components { get; set; }
        public IList<StandardizedTestScoreTypeViewData> ScoreTypes { get; set; }

        protected StandardizedTestViewData(StandardizedTest standardizedTest, IList<StandardizedTestComponent> components, IList<StandardizedTestScoreType> scoreTypes)
            :base(standardizedTest)
        {
            Components = components.Select(StandardizedTestComponentViewData.Create).ToList();
            ScoreTypes = scoreTypes.Select(StandardizedTestScoreTypeViewData.Create).ToList();
        }
        public static StandardizedTestViewData Create(StandardizedTest standardizedTest, IList<StandardizedTestComponent> components, IList<StandardizedTestScoreType> scoreTypes)
        {
            return new StandardizedTestViewData(standardizedTest, components, scoreTypes);
        }
    }

    public class StandardizedTestComponentViewData
    {
        public int Id { get; set; }
        public int StandardizedTestId { get; set; }
        public string Name { get; set; }
        
        public static StandardizedTestComponentViewData Create(StandardizedTestComponent component)
        {
            return new StandardizedTestComponentViewData
            {
                Id = component.Id,
                StandardizedTestId = component.StandardizedTestRef,
                Name = component.Name
            };
        }
    }

    public class StandardizedTestScoreTypeViewData
    {
        public int Id { get; set; }
        public int StandardizedTestId { get; set; }
        public string Name { get; set; }
        
        public static StandardizedTestScoreTypeViewData Create(StandardizedTestScoreType scoreType)
        {
            return new StandardizedTestScoreTypeViewData
            {
                Id = scoreType.Id,
                StandardizedTestId = scoreType.StandardizedTestRef,
                Name = scoreType.Name
            };
        }
    }
}