using AutoMapper;
using LocationSearch.Core.DTO;
using LocationSearch.Core.Services.Interfaces;
using LocationSearch.Data.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LocationSearch.Core.Services.Implementations {
    /// <summary>
    /// Search service to search locations around source location.
    /// </summary>
    public class SearchService : ISearchService {
        private readonly ISearchRepository _searchRepository;
        private readonly IMapper _mapper;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="searchRepository">Search repository</param>
        /// <param name="mapper">Mapper</param>
        public SearchService(ISearchRepository searchRepository, IMapper mapper) {
            _searchRepository = searchRepository;
            _mapper = mapper;
        }

        /// <summary>
        /// Fetches all the locations starting with prefix.
        /// </summary>
        /// <param name="prefix">String to search</param>
        /// <returns>Returns list of locations starts with specified string</returns>
        public IEnumerable<LocationDTO> GetSearchedLocation(string prefix) {
            var locations = _mapper.Map<IEnumerable<LocationDTO>>(_searchRepository.GetLocations().ToList());
            return locations.Where(obj => obj.Address.ToLower().StartsWith(prefix.ToLower()));
        }

        /// <summary>
        /// Gets the locations for the specified page.
        /// </summary>
        /// <param name="sourceAddress">Address to be searched</param>
        /// <param name="maxDistance">Maximum distance</param>
        /// <param name="maxResult">Maximum Result</param>
        /// <param name="skip">Number of items to be skipped</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="totalRecords">Total records</param>
        /// <returns>Returns list of locations</returns>
        public IEnumerable<LocationDTO> GetPage(string sourceAddress, double maxDistance, int? maxResult, int skip, int pageSize, out double totalRecords) {
            var locationsList = GetNearbyLocations(sourceAddress, Convert.ToDouble(maxDistance), maxResult);
            totalRecords = locationsList.Count();
            return locationsList.Skip(skip).Take(pageSize).ToList();
        }

        /// <summary>
        /// Gets nearby locations around source address that falls under maximum distance and returns max results.
        /// If maxResult is 0 then returns all the records that fulfills the criteria.
        /// If the search criteria is same and the event is just pagination then it fetches the list of locations from cache.
        /// </summary>
        /// <param name="sourceAddress">Source address</param>
        /// <param name="maxDistance">Maximum distance</param>
        /// <param name="maxResult">Maximum results</param>
        /// <returns>returns list of locations that matches the search criteria</returns>
        public IEnumerable<LocationDTO> GetNearbyLocations(string sourceAddress, double maxDistance, int? maxResult) {
            List<LocationDTO> nearByLocationsList = new List<LocationDTO>();

            // If the search criteria is new then calculate the distance and fetch the list accordingly else take it from cache.
            if(IsNewSearch(sourceAddress, maxDistance)) {
                var locationsList = _mapper.Map<IEnumerable<LocationDTO>>(_searchRepository.GetLocations().ToList());
                SearchCacheDTO.Address = sourceAddress;
                SearchCacheDTO.MaxDistance = maxDistance;
                var sourceLocation = locationsList.Where(x => x.Address.ToLower().Equals(sourceAddress.ToLower())).FirstOrDefault();
                var targetLocations = locationsList.Where(x => !x.Address.ToLower().Equals(sourceAddress.ToLower())).ToList();

                if(sourceLocation == null) {
                    return nearByLocationsList;
                }

                Parallel.ForEach(targetLocations, location => {
                    location.Distance = CalculateDistance(sourceLocation, location);
                    if(location.Distance <= maxDistance) {
                        lock(targetLocations) {
                            nearByLocationsList.Add(location);
                        }
                    }
                });

                nearByLocationsList = nearByLocationsList.OrderBy(x => x.Distance).ToList();
                SearchCacheDTO.nearyByLocations = nearByLocationsList;
            } else {
                nearByLocationsList = SearchCacheDTO.nearyByLocations.ToList();
            }

            if(maxResult.HasValue && maxResult.Value != 0) {
                nearByLocationsList = nearByLocationsList.Take(maxResult.Value).ToList();
            }
            return nearByLocationsList;
        }

        /// <summary>
        /// Returns true if the search criteria is changed else false.
        /// </summary>
        /// <param name="address">Address</param>
        /// <param name="maxDistance">Maximum distance</param>
        /// <returns></returns>
        private bool IsNewSearch(string address, double maxDistance) {
            if(string.IsNullOrWhiteSpace(SearchCacheDTO.Address) || !SearchCacheDTO.Address.ToLower().Equals(address.ToLower())
                || maxDistance != SearchCacheDTO.MaxDistance)
                return true;
            return false;
        }

        /// <summary>
        /// Calculates the distance between the source and destination location in meters.
        /// </summary>
        /// <param name="fromLocation">Source location</param>
        /// <param name="toLocation">Destination location</param>
        /// <returns>Distance in meters</returns>
        private double CalculateDistance(LocationDTO fromLocation, LocationDTO toLocation) {
            var rlat1 = Math.PI * fromLocation.Latitude / 180;
            var rlat2 = Math.PI * toLocation.Latitude / 180;
            var theta = fromLocation.Longitude - toLocation.Longitude;
            var rtheta = Math.PI * theta / 180;
            var dist = Math.Sin(rlat1) * Math.Sin(rlat2) + Math.Cos(rlat1) * Math.Cos(rlat2) * Math.Cos(rtheta);
            dist = Math.Acos(dist);
            dist = dist * 180 / Math.PI;
            dist = dist * 60 * 1.1515;

            return dist * 1609.344;
        }
    }
}
