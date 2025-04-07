using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Movies.Contracts.Responses
{
    public abstract class HalResponse
    {
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public List<Link> Links { get; set; } = new();
    }

    public class Link
    {
        public required string Href { get; set; }

        public required string Rel { get; set; }

        public required string Type { get; set; }
    }
}
