using System.Collections.Generic;

namespace Chalkable.StiConnector.Connectors.Model
{
    public class DisciplineDetailDashboard
    {
        /// <summary>
        /// A list of every discipline referral for the student in the time frame(between start and end dates)
        /// </summary>
        public IEnumerable<DisciplineReferral> Referrals { get; set; }
    }
}
