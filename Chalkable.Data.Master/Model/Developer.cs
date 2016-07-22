using System;
using Chalkable.Data.Common;

namespace Chalkable.Data.Master.Model
{
    public class Developer
    {
        public const string ID_FIELD = "Id";
        public const string DISTRICT_REF_FIELD = "DistrictRef";
        
        [PrimaryKeyFieldAttr]
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string WebSite { get; set; }
        public Guid? DistrictRef { get; set; }
        [DataEntityAttr]
        public User User { get; set; }

        public string PayPalLogin { get; set; }

        [NotDbFieldAttr]
        public string DisplayName
        {
            get
            {
                return !string.IsNullOrEmpty(Name) ? Name : Email;
            }
        }
        [NotDbFieldAttr]
        public string Email
        {
            get { return User.Login; }            
        }

    }
}