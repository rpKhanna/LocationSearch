using System.Collections.Generic;

namespace LocationSearch.Core.DTO {
    public static class SearchCacheDTO {

        public static string Address { get; set; }

        public static double MaxDistance { get; set; }

        public static IEnumerable<LocationDTO> nearyByLocations { get; set; }
    }
}
