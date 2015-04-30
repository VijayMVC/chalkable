using System.Collections.Generic;
using Chalkable.StiConnector.Connectors.Model;
namespace Chalkable.StiConnector.Connectors
{
    public class SectionCommentConnector : ConnectorBase
    {
        public SectionCommentConnector(ConnectorLocator locator) : base(locator)
        {
        }


        public SectionComment CreateComment(int acadSessionId, int teacherId, SectionComment sectionComment)
        {
            return Post(BuildBaseUrl(acadSessionId, teacherId), sectionComment);
        }

        public void UpdateComment(int acadSessionId, int teacherId, SectionComment sectionComment)
        {
            var url = BuildBaseUrl(acadSessionId, teacherId) + sectionComment.Id;
            Put(url, sectionComment);
        }

        public void DeleteComment(int acadSessionId, int teacherId, int sectionCommentId)
        {
            Delete(BuildBaseUrl(acadSessionId, teacherId) + sectionCommentId);
        }

        public SectionComment GetCommentById(int acadSessionId, int teacherId, int sectionCommentId)
        {
            return Call<SectionComment>(BuildBaseUrl(acadSessionId, teacherId) + "/" + sectionCommentId);
        }

        public IList<SectionComment> GetComments(int acadSessionId, int teacherId)
        {
            return Call<IList<SectionComment>>(BuildBaseUrl(acadSessionId, teacherId));
        } 

        private string BuildBaseUrl(int acadSessionId, int teacherId)
        {
            return string.Format("{0}{1}/teachers/{2}/comments", BaseUrl, acadSessionId, teacherId);
        }
    }
}
