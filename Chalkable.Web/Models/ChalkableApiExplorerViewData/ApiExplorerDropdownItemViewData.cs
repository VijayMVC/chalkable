using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Chalkable.Web.Models.ChalkableApiExplorerViewData
{
    public class ApiExplorerDropdownItemViewData
    {
        public string Name { get; set; }
        public IList<string> RequiredParams { get; set; }
        public bool IsMethod { get; set; }

        public static ApiExplorerDropdownItemViewData Create(string name, bool isMethod, IList<string> requiredParams)
        {
            return new ApiExplorerDropdownItemViewData
            {
                IsMethod = isMethod,
                Name = name,
                RequiredParams = requiredParams
            };
        }

        public static ApiExplorerDropdownItemViewData Create(string name, bool isMethod)
        {
            return new ApiExplorerDropdownItemViewData
            {
                IsMethod = isMethod,
                Name = name,
                RequiredParams = new List<string>()
            };
        }
    }
}