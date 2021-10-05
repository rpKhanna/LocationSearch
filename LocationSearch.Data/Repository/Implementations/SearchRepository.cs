using CsvHelper;
using LocationSearch.Data.Models;
using LocationSearch.Data.Repository.Interfaces;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace LocationSearch.Data.Repository.Implementations {
    /// <summary>
    /// Reads the locations from excel
    /// </summary>
    public class SearchRepository : ISearchRepository {

        /// <summary>
        /// Reads the locations from csv file
        /// </summary>
        /// <returns>List of locations</returns>
        public IEnumerable<Location> GetLocations() {
            List<Location> locations = new List<Location>();
            using(var streamReader = new StreamReader(@"..\LocationSearch.Data\Data\Locations.csv")) {
                using(var csvReader = new CsvReader(streamReader, CultureInfo.InvariantCulture)) {
                    locations = csvReader.GetRecords<Location>().ToList();
                }
            }
            return locations;
        }
    }
}
