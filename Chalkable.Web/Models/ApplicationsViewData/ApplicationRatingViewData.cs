using System.Collections.Generic;
using System.Linq;
using Chalkable.Common;
using Chalkable.Data.Master.Model;
using Chalkable.Web.Models.PersonViewDatas;

namespace Chalkable.Web.Models.ApplicationsViewData
{
    public class ApplicationRatingViewData
    {
        public IList<ApplicationRatingByRoleViewData> RatingByRoles { get; set; }
        public IList<ApplicationRatingByPersonViewData> RatingByPerson { get; set; }
        public double Avg { get; set; }
        private ApplicationRatingViewData() { }

        public static ApplicationRatingViewData Create(IList<ApplicationRating> ratings)
        {
            var ratingsbyrole = new Dictionary<CoreRole, List<ApplicationRating>> //we don't have roles in the master db
            {
                {CoreRoles.CHECKIN_ROLE, ratings.ToList()}
            };
            var res = new ApplicationRatingViewData
            {
                RatingByPerson = ApplicationRatingByPersonViewData.Create(ratings),
                RatingByRoles = ApplicationRatingByRoleViewData.Create(ratingsbyrole)
            };
            res.Avg = res.RatingByPerson.Count != 0 ? res.RatingByPerson.Average(x => x.Rating) : 0;
            return res;
        }
    }



    public class ApplicationRatingByPersonViewData
    {
        public UserViewData Person { get; set; }
        public int Rating { get; set; }
        public string Review { get; set; }

        private ApplicationRatingByPersonViewData() { }

        public static ApplicationRatingByPersonViewData Create(ApplicationRating rating)
        {
            var res = new ApplicationRatingByPersonViewData
            {
                Rating = rating.Rating,
                Review = rating.Review,
                Person = UserViewData.Create(rating.User) 
            };
            return res;
        }

        public static IList<ApplicationRatingByPersonViewData> Create(IList<ApplicationRating> ratings)
        {
            return ratings.Select(Create).ToList();
        }
    }

    public class ApplicationRatingByRoleViewData
    {

        public RoleViewData Role { get; set; }
        public double AvgRating { get; set; }
        public int PersonCount { get; set; }
        private ApplicationRatingByRoleViewData() { }


        public static IList<ApplicationRatingByRoleViewData> Create(IDictionary<CoreRole, List<ApplicationRating>> ratingsbyrole)
        {
            var res = new List<ApplicationRatingByRoleViewData>();
            foreach (var rolerating in ratingsbyrole)
            {
                var ratings = rolerating.Value.Select(x => x.Rating).ToList();
                res.Add(new ApplicationRatingByRoleViewData
                {
                    Role = RoleViewData.Create(rolerating.Key),
                    AvgRating = CallsAvgRating(ratings),
                    PersonCount = rolerating.Value.Count
                });
            }
            return res;
        }

        private static double CallsAvgRating(IList<int> ratings)
        {
            int res = ratings.Sum();
            return res == 0 ? 0 : res / ratings.Count;
        }
    }
}