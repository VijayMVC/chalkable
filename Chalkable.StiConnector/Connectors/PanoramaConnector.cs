using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using Chalkable.StiConnector.Connectors.Model;
using Chalkable.StiConnector.Connectors.Model.SectionPanorama;
using Chalkable.StiConnector.Connectors.Model.StudentPanorama;
using Chalkable.StiConnector.Exceptions;

namespace Chalkable.StiConnector.Connectors
{
    public class PanoramaConnector : ConnectorBase
    {
        public PanoramaConnector(ConnectorLocator locator) : base(locator)
        {
        }

        public SectionPanorama GetSectionPanorama(int sectionId, IList<int> acadSessionIds, IList<int> componentIds, IList<int> scoreTypeIds)
        {
            EnsureApiVersion("7.3.6.20704");

            var sectionPanoramaUrl = $@"chalkable/sections/{sectionId}/panorama";
            var nvc = new NameValueCollection();

            for (var i = 0; i < acadSessionIds.Count; ++i)
                nvc.Add($"acadSessionIds[{i}]", acadSessionIds[i].ToString());

            if (componentIds != null && componentIds.Count > 0 && scoreTypeIds != null && scoreTypeIds.Count > 0)
            {
                ValidatePanoramaParams(componentIds, scoreTypeIds);

                for (int i = 0; i < componentIds.Count; ++i)
                {
                    nvc.Add($"componentIds[{i}]", componentIds[i].ToString());
                    nvc.Add($"scoreTypeIds[{i}]", scoreTypeIds[i].ToString());
                }
            }

            return Call<SectionPanorama>($"{BaseUrl}{sectionPanoramaUrl}", nvc);
        }

        public StudentPanorama GetStudentPanorama(int studentId, IList<int> acadSessionIds, IList<int> componentIds, IList<int> scoreTypeIds)
        {
            EnsureApiVersion("7.3.6.20704");

            var studentPanoramaUrl = $@"chalkable/students/{studentId}/panorama";
            var nvc = new NameValueCollection();

            for (var i = 0; i < acadSessionIds.Count; ++i)
                nvc.Add($"acadSessionIds[{i}]", acadSessionIds[i].ToString());

            if (componentIds == null || componentIds.Count <= 0 || scoreTypeIds == null || scoreTypeIds.Count <= 0)
                return Call<StudentPanorama>($"{BaseUrl}{studentPanoramaUrl}", nvc);

            ValidatePanoramaParams(componentIds, scoreTypeIds);

            for (var i = 0; i < componentIds.Count; ++i)
            {
                nvc.Add($"componentIds[{i}]", componentIds[i].ToString());
                nvc.Add($"scoreTypeIds[{i}]", scoreTypeIds[i].ToString());
            }

            return Call<StudentPanorama>($"{BaseUrl}{studentPanoramaUrl}", nvc);
        }

        private static void ValidatePanoramaParams(IList<int> componentIds, IList<int> scoreTypeIds)
        {
            if (componentIds.Count != scoreTypeIds.Count)
                throw new ChalkableSisException("ComponentsIds and schoreTypeIds lists should have the same length");
        }
    }
}
