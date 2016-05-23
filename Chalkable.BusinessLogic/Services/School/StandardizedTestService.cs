using System.Collections.Generic;
using Chalkable.Data.Common;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.School
{
    public interface IStandardizedTestService
    {
        void AddStandardizedTests(IList<StandardizedTest> standardizedTests);
        void EditStandardizedTests(IList<StandardizedTest> standardizedTests);
        void DeleteStandardizedTests(IList<StandardizedTest> standardizedTests);

        void AddStandardizedTestComponents(IList<StandardizedTestComponent> standardizedTestComponents);
        void EditStandardizedTestComponents(IList<StandardizedTestComponent> standardizedTestComponents);
        void DeleteStandardizedTestComponents(IList<StandardizedTestComponent> standardizedTestComponents);

        void AddStandardizedTestScoreTypes(IList<StandardizedTestScoreType> standardizedTestScoreTypes);
        void EditStandardizedTestScoreTypes(IList<StandardizedTestScoreType> standardizedTestScoreTypes);
        void DeleteStandardizedTestScoreTypes(IList<StandardizedTestScoreType> standardizedTestScoreTypes);

        IList<StandardizedTestDetails> GetListOfStandardizedTestDetails();
    }

    class StandardizedTestService : SchoolServiceBase, IStandardizedTestService
    {
        public StandardizedTestService(IServiceLocatorSchool serviceLocator) : base(serviceLocator)
        {
        }

        public void AddStandardizedTests(IList<StandardizedTest> standardizedTests)
        {
            DoUpdate(u => new DataAccessBase<StandardizedTest>(u).Insert(standardizedTests));
        }

        public void EditStandardizedTests(IList<StandardizedTest> standardizedTests)
        {
            DoUpdate(u => new DataAccessBase<StandardizedTest>(u).Update(standardizedTests));
        }

        public void DeleteStandardizedTests(IList<StandardizedTest> standardizedTests)
        {
            DoUpdate(u => new DataAccessBase<StandardizedTest>(u).Delete(standardizedTests));
        }

        public void AddStandardizedTestComponents(IList<StandardizedTestComponent> standardizedTestComponents)
        {
            DoUpdate(u => new DataAccessBase<StandardizedTestComponent>(u).Insert(standardizedTestComponents));
        }

        public void EditStandardizedTestComponents(IList<StandardizedTestComponent> standardizedTestComponents)
        {
            DoUpdate(u => new DataAccessBase<StandardizedTestComponent>(u).Update(standardizedTestComponents));
        }

        public void DeleteStandardizedTestComponents(IList<StandardizedTestComponent> standardizedTestComponents)
        {
            DoUpdate(u => new DataAccessBase<StandardizedTestComponent>(u).Delete(standardizedTestComponents));
        }

        public void AddStandardizedTestScoreTypes(IList<StandardizedTestScoreType> standardizedTestScoreTypes)
        {
            DoUpdate(u => new DataAccessBase<StandardizedTestScoreType>(u).Insert(standardizedTestScoreTypes));
        }

        public void EditStandardizedTestScoreTypes(IList<StandardizedTestScoreType> standardizedTestScoreTypes)
        {
            DoUpdate(u => new DataAccessBase<StandardizedTestScoreType>(u).Update(standardizedTestScoreTypes));
        }

        public void DeleteStandardizedTestScoreTypes(IList<StandardizedTestScoreType> standardizedTestScoreTypes)
        {
            DoUpdate(u => new DataAccessBase<StandardizedTestScoreType>(u).Delete(standardizedTestScoreTypes));
        }

        public IList<StandardizedTestDetails> GetListOfStandardizedTestDetails()
        {
            return new List<StandardizedTestDetails>
            {
                new StandardizedTestDetails
                {
                    Id = 1,
                    Name = "Test1",
                    DisplayName = "Test1",
                    Components = new List<StandardizedTestComponent>
                    {
                        new StandardizedTestComponent {Id = 1, Name = "Math", StandardizedTestRef = 1},
                        new StandardizedTestComponent {Id = 2, Name = "English", StandardizedTestRef = 1},
                    },
                    ScoreTypes = new List<StandardizedTestScoreType>
                    {
                        new StandardizedTestScoreType {Id = 1, StandardizedTestRef = 1, Name = "Numeric"},
                        new StandardizedTestScoreType {Id = 2, StandardizedTestRef = 1, Name = "Pass"},
                        new StandardizedTestScoreType {Id = 3, StandardizedTestRef = 1, Name = "Raw"}
                    }
                },
                new StandardizedTestDetails
                {
                    Id = 1,
                    Name = "Test2",
                    DisplayName = "Test2",
                    Components = new List<StandardizedTestComponent>
                    {
                        new StandardizedTestComponent {Id = 3, Name = "Math2", StandardizedTestRef = 2},
                        new StandardizedTestComponent {Id = 4, Name = "English2", StandardizedTestRef = 2},
                    },
                    ScoreTypes = new List<StandardizedTestScoreType>
                    {
                        new StandardizedTestScoreType {Id = 4, StandardizedTestRef = 2, Name = "Numeric2"},
                        new StandardizedTestScoreType {Id = 5, StandardizedTestRef = 2, Name = "Pass2"},
                        new StandardizedTestScoreType {Id = 6, StandardizedTestRef = 2, Name = "Raw2"}
                    }
                }
            };
        }
    }
}