using Chalkable.Data.Master.Model;

namespace Chalkable.Web.Models
{
    public class SisSyncViewData
    {
        public string SisDatabaseUrl { get; set; }
        public string SisDatabaseName { get; set; }
        public string SisDatabaseUserName { get; set; }
        public string SisDatabasePassword { get; set; }

        public static SisSyncViewData Create(SisSync sisSync)
        {
            return new SisSyncViewData
            {
                SisDatabaseName = sisSync.SisDatabaseName,
                SisDatabasePassword = sisSync.SisDatabasePassword,
                SisDatabaseUserName = sisSync.SisDatabaseUserName,
                SisDatabaseUrl = sisSync.SisDatabaseUrl
            };
        }
    }
}