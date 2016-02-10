using Newtonsoft.Json;

namespace Chalkable.API.Models
{
    public class CoreRole
    {
        [JsonProperty("name")]
        public string Name { get; private set; }
        [JsonIgnore]
        public string LoweredName => Name.ToLower();
        [JsonProperty("description")]
        public string Description { get; private set; }
        [JsonProperty("id")]
        public int Id { get; private set; }

        public CoreRole()
        {
        }

        public CoreRole(int id, string name, string description)
        {
            Name = name;
            Id = id;
            Description = description;
        }
    }
}
