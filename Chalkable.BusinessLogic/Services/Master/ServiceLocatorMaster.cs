using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chalkable.BusinessLogic.Services.Master
{
    public interface IServiceLocatorMaster
    {
        UserContext Context { get; }
        IUserService UserService { get; }
    }

    public class ServiceLocatorMaster : ServiceLocator, IServiceLocatorMaster
    {
        private IUserService userService;
        public ServiceLocatorMaster(UserContext context) : base(context)
        {
            userService = new UserService(this);
        }

        public IUserService UserService { get { return userService; } }
    }
}
