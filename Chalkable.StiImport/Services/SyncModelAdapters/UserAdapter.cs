using System;
using System.Collections.Generic;
using System.Linq;
using Chalkable.BusinessLogic.Services.Master;
using Chalkable.StiConnector.SyncModel;

namespace Chalkable.StiImport.Services.SyncModelAdapters
{
    public class UserAdapter : SyncModelAdapter<User>
    {
        private const string USER_EMAIL_FMT = "user{0}_{1}@chalkable.com";
        private const string DEF_USER_PASS = "Qwerty1@";
        public UserAdapter(AdapterLocator locator) : base(locator)
        {
        }

        protected override void InsertInternal(IList<User> entities)
        {
            var users = entities.Select(x => new Data.Master.Model.User
            {
                Id = Guid.NewGuid(),
                DistrictRef = ServiceLocatorSchool.Context.DistrictId,
                FullName = x.FullName,
                IsDemoUser = false,
                SisUserId = x.UserID,
                SisUserName = x.UserName,
                Password = UserService.PasswordMd5(DEF_USER_PASS),
                Login = string.Format(USER_EMAIL_FMT, x.UserID, Guid.NewGuid()),
                IsSysAdmin = false
            }).ToList();
            ServiceLocatorMaster.UserService.Add(users);
            Locator.InsertedUserIds = users.Select(x => x.Id).ToList();
        }

        protected override void UpdateInternal(IList<User> entities)
        {
            var users = entities.Select(x => new Data.Master.Model.User
            {
                DistrictRef = ServiceLocatorSchool.Context.DistrictId,
                FullName = x.FullName,
                SisUserId = x.UserID,
                SisUserName = x.UserName
            }).ToList();
            ServiceLocatorMaster.UserService.Edit(users);
        }

        protected override void DeleteInternal(IList<User> entities)
        {
            var ids = entities.Select(x => x.UserID).ToList();
            ServiceLocatorMaster.UserService.DeleteUsers(ids, ServiceLocatorSchool.Context.DistrictId.Value);
        }

        protected override void PrepareToDeleteInternal(IList<User> entities)
        {
            throw new NotImplementedException();
        }
    }
}