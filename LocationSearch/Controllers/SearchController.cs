using LocationSearch.Core.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;

namespace LocationSearch.Controllers {
    /// <summary>
    /// Search controller handles all the apis related to searching the locations like location autocomplete, search 
    /// and pagination.
    /// </summary>
    [ResponseCache(CacheProfileName = "Default30")]
    public class SearchController : Controller {
        private readonly ISearchService _searchService;
        private readonly ILogger<SearchController> _logger;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="searchService"></param>
        public SearchController(ISearchService searchService, ILogger<SearchController> logger) {
            _searchService = searchService;
            _logger = logger;
        }

        /// <summary>
        /// Returns search view.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Index() {
            _logger.LogInformation("Search view loaded.");
            return View();
        }

        /// <summary>
        /// Searches and returns data according to search criteria. Supports pagination.
        /// </summary>
        /// <returns>List of locations</returns>
        [HttpPost]
        public ActionResult Search() {
            var draw = Request.Form["draw"].FirstOrDefault();
            var start = Request.Form["start"].FirstOrDefault();
            var length = Request.Form["length"].FirstOrDefault();
            var location = Request.Form["location"].FirstOrDefault();
            var maxDistance = Request.Form["maxdistance"].FirstOrDefault();
            var maxResult = Request.Form["maxresult"].FirstOrDefault();
            int pageSize = length != null ? Convert.ToInt32(length) : 0;
            int skip = start != null ? Convert.ToInt32(start) : 0;

            _logger.LogInformation($"Search criteria is location : {location}, MaxDistance: {maxDistance} and MaxResult: {maxResult}");

            int noOfResults = string.IsNullOrWhiteSpace(maxResult) ? 0 : Convert.ToInt32(maxResult);
            double totalRecords;
            var locations = _searchService.GetPage(location, Convert.ToDouble(maxDistance), noOfResults, skip, pageSize, out totalRecords);

            var jsonData = new { draw = draw, recordsFiltered = totalRecords, recordsTotal = totalRecords, data = locations };
            return Ok(jsonData);
        }

        /// <summary>
        /// Returns all locations that starts with prefix for autocomplete.
        /// </summary>
        /// <param name="prefix">Address</param>
        /// <returns>List of locations in json format</returns>
        [HttpPost]
        public JsonResult GetLocations(string prefix) {
            _logger.LogInformation($"User have searched for locations that starts with {prefix}");
            var result = _searchService.GetSearchedLocation(prefix).ToList();
            return Json(result);
        }
    }
}
