using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Chalkable.Common;
using Chalkable.Data.Master.Model;

namespace Chalkable.Web.Models.ApplicationsViewData
{
    public class ApplicationRatingViewData
    {
        //public IList<ApplicationRatingByRoleViewData> RatingByRoles { get; set; }
        //public IList<ApplicationRatingByPersonViewData> RatingByPerson { get; set; }
        //public double Avg { get; set; }
        //private ApplicationRatingViewData() { }

        //public static ApplicationRatingViewData Create(IDictionary<int, IList<ApplicationRating>> ratingsbyrole, IList<ApplicationRating> ratings)
        //{
        //    var res = new ApplicationRatingViewData
        //    {
        //        RatingByPerson = ApplicationRatingByPersonViewData.Create(ratings),
        //        RatingByRoles = ApplicationRatingByRoleViewData.Create(ratingsbyrole)
        //    };
        //    res.Avg = res.RatingByPerson.Count != 0 ? res.RatingByPerson.Average(x => x.Rating) : 0;
        //    return res;
        //}

        //public static ApplicationRatingViewData Create(IDictionary<int, IList<ApplicationRating>> ratingsbyrole, IList<ApplicationRating> ratings, IList<CoreRole> roles)
        //{
        //    var res = new ApplicationRatingViewData
        //    {
        //        RatingByPerson = ApplicationRatingByPersonViewData.Create(ratings),
        //        RatingByRoles = ApplicationRatingByRoleViewData.Create(ratingsbyrole, roles)
        //    };
        //    res.Avg = res.RatingByPerson.Count != 0 ? res.RatingByPerson.Average(x => x.Rating) : 0;
        //    return res;
        //}

    }
}