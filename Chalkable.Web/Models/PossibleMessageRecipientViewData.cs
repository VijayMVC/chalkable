using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Chalkable.BusinessLogic.Common;
using Chalkable.Data.School.Model;

namespace Chalkable.Web.Models
{
    public class PossibleMessageRecipientViewData
    {
        public int? PersonId { get; set; }
        public string DisplayName { get; set; }

        public int? ClassId { get; set; }
        public string ClassName { get; set; }
        public string ClassNumber { get; set; }

        public static IList<PossibleMessageRecipientViewData> Create(PossibleMessageRecipients possibleMessageRecipients)
        {
            var res = new List<PossibleMessageRecipientViewData>();
            if (possibleMessageRecipients != null)
            {
                if(possibleMessageRecipients.Persons != null && possibleMessageRecipients.Persons.Count > 0)
                    res.AddRange(possibleMessageRecipients.Persons.Select(p=> new PossibleMessageRecipientViewData
                    {
                        PersonId = p.Id,
                        DisplayName = p.DisplayName()
                    }));
                if(possibleMessageRecipients.Classes != null && possibleMessageRecipients.Classes.Count > 0)
                    res.AddRange(possibleMessageRecipients.Classes.Select(c=> new PossibleMessageRecipientViewData
                    {
                        ClassNumber = c.ClassNumber,
                        ClassName = c.Name,
                        ClassId = c.Id
                    }));
            }
            return res;
        }
    }
}