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
        public StandardizedTestComponentViewData Component { get; set; }
        public StandardizedTestScoreTypeViewData ScoreType { get; set; }

        protected StandardizedTestViewData(StandardizedTest standardizedTest, StandardizedTestComponent component, StandardizedTestScoreType scoreType)
            :base(standardizedTest)
        {
            Component = StandardizedTestComponentViewData.Create(component);
            ScoreType = StandardizedTestScoreTypeViewData.Create(scoreType);
        }
        public static StandardizedTestViewData Create(StandardizedTest standardizedTest, StandardizedTestComponent component, StandardizedTestScoreType scoreType)
        {
            return new StandardizedTestViewData(standardizedTest, component, scoreType);
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