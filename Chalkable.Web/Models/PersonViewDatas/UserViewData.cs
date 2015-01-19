using System;
using Chalkable.Data.Master.Model;

namespace Chalkable.Web.Models.PersonViewDatas
{
    public class UserViewData
    {
        public Guid Id { get; set; }
        public string FullName { get; set; }
        public Guid? DistrictId { get; set; }
        public int? SisId { get; set; }

        public static UserViewData Create(User user)
        {
            return new UserViewData
            {
                Id = user.Id,
                FullName = user.FullName,
                DistrictId = user.DistrictRef,
                SisId = user.SisUserId
            };
        }
    }
}