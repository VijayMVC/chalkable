using Newtonsoft.Json;

namespace Chalkable.Data.Master.Model
{
    public class DatabaseDacPacUpdateTaskData
    {
        public string DacPacName { get; set; }
        public string MasterDacPacUri { get; set; }
        public string SchoolDacPacUri { get; set; }

        public string ServerName { get; set; }
        public string DatabaseName { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }

        public static DatabaseDacPacUpdateTaskData FromString(string data)
        {
            return JsonConvert.DeserializeObject<DatabaseDacPacUpdateTaskData>(data);
        }
    }
}