using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Chalkable.Web.Models.ChalkableApiExplorerViewData
{
    public class ApiExplorerViewData
    {
        public string Token { get; set; }
        public string Role { get; set; }
        public IList<ApiExplorerControllerViewData> Controllers { get; set; }

        private ApiExplorerViewData() { }



        public static ApiExplorerViewData Create(IList<ChalkableApiControllerDescription> apiExplorerDescription, string token, string role)
        {
            return new ApiExplorerViewData
            {
                Role = role,
                Token = token,
                Controllers = apiExplorerDescription.Select(ApiExplorerControllerViewData.Create).ToList()
            };
        }

    }
}