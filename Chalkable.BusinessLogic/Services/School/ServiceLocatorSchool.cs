using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chalkable.BusinessLogic.Services.School
{
    public interface IServiceLocatorSchool
    {
        UserContext Context { get; }
        IPersonService PersonService { get; }
        IAddressSerivce AddressSerivce { get; }
    }
    public class ServiceLocatorSchool : ServiceLocator, IServiceLocatorSchool
    {
        private IPersonService personService;
        private IAddressSerivce addressSerivce;
        public ServiceLocatorSchool(UserContext context) : base(context)
        {
            personService = new PersonService(this);
            addressSerivce = new AddressService(this);
        }

        public IPersonService PersonService { get { return personService; } }

        public IAddressSerivce AddressSerivce
        {
            get { return addressSerivce; }
        }
    }
}
