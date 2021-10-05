using LocationSearch.Core.DTO;
using System.Collections.Generic;

namespace LocationSearch.Core.Services.Interfaces {
    public interface ISearchService {
        IEnumerable<LocationDTO> GetSearchedLocation(string prefix);

        IEnumerable<LocationDTO> GetPage(string sourceAddress, double maxDistance, int? maxResult, int skip, int pageSize, out double totalRecords);

        IEnumerable<LocationDTO> GetNearbyLocations(string sourceAddress, double maxDistance, int? maxResult);
    }
}
