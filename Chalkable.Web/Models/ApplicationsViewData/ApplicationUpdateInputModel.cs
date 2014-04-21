using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Chalkable.BusinessLogic.Model;
using Chalkable.Common;
using Chalkable.Data.Common.Enums;

namespace Chalkable.Web.Models.ApplicationsViewData
{
    public class ApplicationUpdateInputModel : BaseApplicationInfo
    {
        public Guid ApplicationId { get; set; }
        public bool ForSubmit { get; set; }

        public IList<int> Permissions
        {
            get { return PermissionIds.Select(x => (int) x).ToList(); }
            set { PermissionIds = value.Select(x => (AppPermissionType)x).ToList(); }
        }

       
        public ApplicationUpdateInputModel()
        {
            Permissions = new IntList();
        }
    }
}