using AutoMapper;
using LocationSearch.Core.DTO;
using LocationSearch.Data.Models;

namespace LocationSearch.Core.Profiles {
    public class LocationSearchProfile : Profile {
        public LocationSearchProfile() {
            CreateMap<Location, LocationDTO>();
        }
    }
}
