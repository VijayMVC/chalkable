using System.Collections.Generic;
using Chalkable.StiConnector.Connectors.Model;
namespace Chalkable.StiConnector.Connectors
{
    public class SectionCommentConnector : ConnectorBase
    {
        public SectionCommentConnector(ConnectorLocator locator) : base(locator)
        {
        }

        
        public GradebookCommect CreateComment(int acadSessionId, int teacherId, GradebookCommect sectionComment)
        {
            return Post(BuildBaseUrl(acadSessionId, teacherId), sectionComment);
        }
        
        public void UpdateComment(int acadSessionId, int teacherId, GradebookCommect sectionComment)
        {
            var url = BuildBaseUrl(acadSessionId, teacherId) + "/" + sectionComment.Id;
            Put(url, sectionComment);
        }
        
        public void DeleteComment(int acadSessionId, int teacherId, int sectionCommentId)
        {
            Delete(BuildBaseUrl(acadSessionId, teacherId) + "/" + sectionCommentId);
        }
        
        public GradebookCommect GetCommentById(int acadSessionId, int teacherId, int sectionCommentId)
        {
            return Call<GradebookCommect>(BuildBaseUrl(acadSessionId, teacherId) + "/" + sectionCommentId);
        }

        
        public IList<GradebookCommect> GetComments(int acadSessionId, int teacherId)
        {
            return Call<IList<GradebookCommect>>(BuildBaseUrl(acadSessionId, teacherId) + "/");
        }
        
        private string BuildBaseUrl(int acadSessionId, int teacherId)
        {
            return $"{BaseUrl}{acadSessionId}/teachers/{teacherId}/comments";
        }
    }
}
