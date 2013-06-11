using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chalkable.BusinessLogic.Services.Master;

namespace Chalkable.BusinessLogic.Services.School
{
    public interface IServiceLocatorSchool
    {
        IServiceLocatorMaster ServiceLocatorMaster { get; }
        UserContext Context { get; }
        IPersonService PersonService { get; }
        IAddressSerivce AddressSerivce { get; }
        IGradeLevelService GradeLevelService { get; }
       
    }
    public class ServiceLocatorSchool : ServiceLocator, IServiceLocatorSchool
    {
        private IServiceLocatorMaster serviceLocatorMaster;
        private IPersonService personService;
        private IAddressSerivce addressSerivce;
        private IGradeLevelService gradeLevelService;

        public ServiceLocatorSchool(IServiceLocatorMaster serviceLocatorMaster)
            : base(serviceLocatorMaster.Context)
        {
            this.serviceLocatorMaster = serviceLocatorMaster;
            personService = new PersonService(this);
            addressSerivce = new AddressService(this);
            gradeLevelService = new GradeLevelService(this);
        }

        public IPersonService PersonService
        {
            get { return personService; }
        }
        public IAddressSerivce AddressSerivce
        {
            get { return addressSerivce; }
        }
        public IGradeLevelService GradeLevelService
        {
            get { return gradeLevelService; }
        }
        public IServiceLocatorMaster ServiceLocatorMaster
        {
            get { return serviceLocatorMaster; }
        }
    }
}
