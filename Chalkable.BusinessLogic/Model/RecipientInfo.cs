﻿using System.Collections.Generic;
using System.Linq;

namespace Chalkable.BusinessLogic.Model
{
    public class RecipientInfo
    {
        public bool ToAll { get; set; }
        public int? RoleId { get; set; }
        public int? GradeLevelId { get; set; }
        public int? PersonId { get; set; }
        public int? SchoolId { get; set; }

        public static RecipientInfo Create(bool toAll, int? roleId, int? gradeLevelId, int? personId, int? schoolId)
        {
            var res = new RecipientInfo
            {
                ToAll = toAll,
                RoleId = roleId,
                GradeLevelId = gradeLevelId,
                PersonId = personId,
                SchoolId = schoolId
            };
            return res;
        }
        public static RecipientInfo Create(List<string> recipient)
        {
            var res = new RecipientInfo
            {
                ToAll = int.Parse(recipient[0]) == 1,
                RoleId = int.Parse(recipient[1]) == -1 ? (int?)null : int.Parse(recipient[1]),
                GradeLevelId = (string.IsNullOrEmpty(recipient[2]) || recipient[2] == "-1") ? (int?)null : int.Parse(recipient[2]),
                PersonId = (string.IsNullOrEmpty(recipient[3]) || recipient[3] == "-1") ? (int?)null : int.Parse(recipient[3]),
                SchoolId = (string.IsNullOrEmpty(recipient[4]) || recipient[4] == "-1") ? (int?)null : int.Parse(recipient[4]),
            };
            return res;
        }

        public static List<RecipientInfo> Create(IList<List<string>> recipients)
        {
            return PrepareRecipientInfo(recipients.Select(Create).ToList());
        }

        private static List<RecipientInfo> PrepareRecipientInfo(IList<RecipientInfo> recipientInfos)
        {
            foreach (var recipientInfo in recipientInfos.Where(recipientInfo => recipientInfo.ToAll))
            {
                return new List<RecipientInfo> { recipientInfo };
            }
            var res = new List<RecipientInfo>();
            res.AddRange(recipientInfos.Where(x => x.RoleId.HasValue && !x.GradeLevelId.HasValue).ToList());
            res.AddRange(recipientInfos.Where(x => x.PersonId.HasValue && !x.RoleId.HasValue && !x.GradeLevelId.HasValue).ToList());
            foreach (var recipientInfo in recipientInfos)
            {
                if (recipientInfo.RoleId.HasValue && recipientInfo.GradeLevelId.HasValue
                    && !res.Any(x => x.RoleId == recipientInfo.RoleId && (!x.GradeLevelId.HasValue || x.GradeLevelId == recipientInfo.GradeLevelId)))
                {
                    res.Add(recipientInfo);
                }
            }
            return res;
        }
    }
    
}
