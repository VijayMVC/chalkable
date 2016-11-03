using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Chalkable.API.Models.Panorama;
using Newtonsoft.Json;

namespace Chalkable.API.Endpoints
{
    public class PanoramaEndpoint : Base
    {
        public PanoramaEndpoint(IConnector connector) : base(connector)
        {
        }

        public async Task<ClassPanorama> GetClassPanorama(int classId, PanoramaSettings settings = null, IList<int> selectedStudentIds = null)
        {
            var data = new
            {
                ClassId = classId,
                SelectedStudents = selectedStudentIds ?? new List<int>(),
                Settings = settings ?? new PanoramaSettings()
            };

            using (var stream = new MemoryStream())
            {
                var writer = new StreamWriter(stream);
                JsonSerializer.Create().Serialize(writer, data);
                writer.Flush();
                return await Connector.Put<ClassPanorama>("Class/Panorama.json", stream);
            }

        }
    }
    
}
