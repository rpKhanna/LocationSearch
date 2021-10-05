using System.ComponentModel.DataAnnotations;

namespace LocationSearch.Models
{
    public class SearchViewModel
    {
        [Required(ErrorMessage = "Please select valid address")]
        public string Address { get; set; }

        [Required(ErrorMessage = "Please select valid distance")]
        public int MaxDistance { get; set; } = 0;

        public int? MaxResult { get; set; }
    }
}
