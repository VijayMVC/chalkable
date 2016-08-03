using System.Collections.Generic;
using System.Linq;
using Chalkable.Data.Common;
using Chalkable.Data.School.DataAccess;
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

        IList<StandardizedTestDetails> GetListOfStandardizedTestDetails(IList<int> ids = null);
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

        public IList<StandardizedTestDetails> GetListOfStandardizedTestDetails(IList<int> ids = null)
        {
            return DoRead(uow => new StandardizedTestDataAccess(uow).GetListOfStandardizedTestDetails(ids));
        }
    }
}