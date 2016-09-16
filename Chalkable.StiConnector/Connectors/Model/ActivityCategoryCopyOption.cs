using System.Collections.Generic;

namespace Chalkable.StiConnector.Connectors.Model
{
    public class ActivityCategoryCopyOption
    {
        /// <summary>
        /// List of Category Ids that will be copied
        /// </summary>
        public IEnumerable<int> CategoryIds { get; set; }

        /// <summary>
        /// List of section ids to copy the categories TO
        /// </summary>
        public IEnumerable<int> CopyToSectionIds { get; set; }
    }
}
