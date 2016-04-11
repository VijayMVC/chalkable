using System;
using System.Collections.Generic;
using System.Linq;
using Chalkable.API.Models;

namespace Chalkable.Api.SampleApp.Logic
{
    public class ContentStorage
    {
        private static IList<ApplicationContent> _appContents;

        static ContentStorage()
        {
            if(_appContents == null)
                _appContents = GenerateDefaultAppContens();
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
                    Description = $"Create by Publisher{i}\n" +
                                  $"Standards: st{i}, st{i+1}, st{i+2}\n" +
                                  $"Grades: {i} - {i+1}\n" +
                                  $"Rating: {i} Stars",
                    Text = $"Test text {i}",
                    ImageUrl = $"https://chalkable1.blob.core.windows.net/pictureconteiner/pictureconteiner_blob_dd7af2af-dc8d-4053-8225-ce01e2b06002-170x110"
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
            return _appContents;
        }

        public ApplicationContent GetContentById(string contentId)
        {
            return _appContents.FirstOrDefault(x => x.ContentId == contentId);
        }
    }
}