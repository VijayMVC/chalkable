using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chalkable.BusinessLogic.Services.School
{

    public interface IServiceLocatorMaster
    {
        UserContext Context { get; }
        IPersonService PersonService { get; }
    }

    public class ServiceLocatorSchool : ServiceLocator , IServiceLocatorMaster
    {
        private IPersonService personService;
        public ServiceLocatorSchool(UserContext context) : base(context)
        {
            personService = new PersonService(this);
        }

        public IPersonService PersonService { get { return personService; } }
    }
}
