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

        public IntList Permissions
        {
            get
            {
                var res = new IntList();
                res.AddRange(PermissionIds.Select(x => (int)x).ToList());
                return res;
            }
            set { PermissionIds = value.Select(x => (AppPermissionType)x).ToList(); }
        }

        public new IntList GradeLevels
        {
            get
            {
                var res = new IntList();
                res.AddRange(base.GradeLevels);
                return res;
            }
            set { base.GradeLevels = value; }
        }
        public new GuidList Categories
        {
            get
            {
                var res = new GuidList();
                res.AddRange(base.Categories);
                return res;
            }
            set { base.Categories = value; }
        }
        public new GuidList PicturesId
        {
            get
            {
                var res = new GuidList();
                res.AddRange(base.PicturesId);
                return res;
            }
            set { base.PicturesId = value; }
        }
        
        public ApplicationUpdateInputModel()
        {
            Permissions = new IntList();
            GradeLevels = new IntList();
            Categories = new GuidList();
            PicturesId = new GuidList();
        }
    }
}