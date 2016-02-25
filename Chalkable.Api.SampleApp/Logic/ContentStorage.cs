using System.Collections.Generic;
using System.Linq;
using Chalkable.API.Models;

namespace Chalkable.Api.SampleApp.Logic
{
    public class ContentStorage
    {
        private static IList<ApplicationContent> appContents_;

        static ContentStorage()
        {
            if(appContents_ == null)
                appContents_ = GenerateDefaultAppContens();
        }

        private static IList<ApplicationContent> GenerateDefaultAppContens()
        {
            const int defaultContentCount = 20;
            var res = new List<ApplicationContent>();
            for (var i = 1; i < defaultContentCount + 1; i++)
            {
                res.Add(new ApplicationContent
                {
                    ContentId = i.ToString(),
                    Text = $"Test text {i}",
                    ImageUrl = $"http://defaultImageUrl{i}.png"
                });
            }
            return res;
        }

        public static ContentStorage GetStorage()
        {
            return new ContentStorage();
        }

        public IList<ApplicationContent> GetContents()
        {
            return appContents_;
        }

        public ApplicationContent GetContentById(string contentId)
        {
            return appContents_.FirstOrDefault(x => x.ContentId == contentId);
        }
    }
}