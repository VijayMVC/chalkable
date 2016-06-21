using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chalkable.Common;
using Chalkable.StiConnector.Connectors.Model;
using Chalkable.StiConnector.Exceptions;

namespace Chalkable.StiConnector.Connectors
{
    public class PanoramaConnector : ConnectorBase
    {
        public PanoramaConnector(ConnectorLocator locator) : base(locator)
        {
        }

        public SectionPanorama GetSectionPanorama(int sectionId, IList<int> acadSessionIds, IList<int> componentIds,
            IList<int> scoreTypeIds)
        {
            var panoramaUrl = $@"chalkable/sections/{sectionId}/panorama";
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

            return Call<SectionPanorama>($"{BaseUrl}{panoramaUrl}", nvc);
        }

        private void ValidatePanoramaParams(IList<int> componentIds, IList<int> scoreTypeIds)
        {
            if (componentIds.Count != scoreTypeIds.Count)
                throw new ChalkableSisException("ComponentsIds and schoreTypeIds lists should have the same length");
        }
    }
}
