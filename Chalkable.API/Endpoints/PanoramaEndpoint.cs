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
            return await Connector.Post<ClassPanorama>("/Class/Panorama.json", data);
        }
    }
    
}
