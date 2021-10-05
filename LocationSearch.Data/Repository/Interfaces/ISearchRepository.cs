using LocationSearch.Data.Models;
using System.Collections.Generic;

namespace LocationSearch.Data.Repository.Interfaces {
    public interface ISearchRepository {
        IEnumerable<Location> GetLocations();
    }
}
