using System.Collections.Generic;

namespace APIMS_Api.Models
{
    public class AqiApiResponse
    {
        public List<StationFeature> Features { get; set; } = new();
    }
}
