using System.Collections.Generic;
using System.Linq;
using Chalkable.Data.Master.Model;

namespace Chalkable.Web.Models
{
    public class PreferenceViewData
    {
        public string Key { get; set; }
        public string Value { get; set; }
        public bool IsPublicPref { get; set; }
        public int Category { get; set; }
        public int Type { get; set; }
        public string Hint { get; set; }

        private PreferenceViewData() { }

        public static PreferenceViewData Create(Preference preference)
        {
            var res = new PreferenceViewData
                          {
                              Key = preference.Key,
                              Value = preference.Value,
                              IsPublicPref = preference.IsPublic,
                              Category = (int) preference.Category,
                              Type = (int) preference.Type,
                              Hint = preference.Hint
                          };
            return res;
        }

        public static IList<PreferenceViewData> Create(IList<Preference> preferences)
        {
            var res = preferences.Select(Create);
            return res.ToList();
        }
    }
}