using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace Chalkable.Data.Master.Model
{
    public class UpdateSql
    {
        public string Sql { get; set; }
        public bool RunOnMaster { get; set; } 
    }

    public class DatabaseUpdateTaskData
    {
        public UpdateSql[] Sqls;

        public DatabaseUpdateTaskData()
        {
            
        }

        public DatabaseUpdateTaskData(IEnumerable<UpdateSql> sqls)
        {
            Sqls = sqls.ToArray();
        }

        public DatabaseUpdateTaskData(string str)
        {
            var serializer = new XmlSerializer(typeof(UpdateSql[]));
            using (var stream = new StringReader(str))
            {
                Sqls = (UpdateSql[])serializer.Deserialize(stream);    
            }
        }

        public override string ToString()
        {
            var serializer = new XmlSerializer(typeof(UpdateSql[]));
            using (var s = new MemoryStream())
            {
                serializer.Serialize(s, Sqls);
                s.Seek(0, SeekOrigin.Begin);
                return new StreamReader(s).ReadToEnd();
            }
        }

    }
}