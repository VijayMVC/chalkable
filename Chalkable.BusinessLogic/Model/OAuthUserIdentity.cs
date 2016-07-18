using System;
using System.Text;
using Chalkable.Common;
using Chalkable.Common.Exceptions;

namespace Chalkable.BusinessLogic.Model
{
    public class OAuthUserIdentityInfo
    {
        public string UserName { get; set; }
        public string SessionKey { get; set; }
        public int? SchoolYearId { get; set; }
        public CoreRole Role { get; set; }

        private const int EMPTY_SCHOOL_YEAR_ID = -1;

        public static OAuthUserIdentityInfo Create(string userName, CoreRole role, int? schoolYearId, string sessionKey)
        {
            return new OAuthUserIdentityInfo
            {
                UserName = userName,
                Role = role,
                SchoolYearId = schoolYearId,
                SessionKey = sessionKey,
            };
        }

        public static OAuthUserIdentityInfo CreateFromString(string identity)
        {
            var sl = identity.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

            if (sl.Length < 4)
                throw new ChalkableException("Invalid identity parameter. Could not conver to OAuthUserIdentity object");

            var syId = int.Parse(sl[2]);
            return new OAuthUserIdentityInfo
            {
                UserName = sl[0],
                Role = CoreRoles.GetById(int.Parse(sl[1])),
                SchoolYearId = syId == EMPTY_SCHOOL_YEAR_ID ? (int?)null : syId,
                SessionKey = sl[3]
            };
        }

        public override string ToString()
        {
            var nameIdBulder = new StringBuilder();
            nameIdBulder.AppendLine(UserName)
                .AppendLine(Role.Id.ToString())
                .AppendLine((SchoolYearId ?? EMPTY_SCHOOL_YEAR_ID).ToString())
                .AppendLine(SessionKey);
            return nameIdBulder.ToString();
        }
    }

}
