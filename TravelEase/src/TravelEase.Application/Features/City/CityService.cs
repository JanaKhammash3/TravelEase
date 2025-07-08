using TravelEase.TravelEase.Application.Interfaces;

namespace TravelEase.TravelEase.Application.Features.City
{
    public class CityService : ICityService
    {
        private readonly ICityRepository _cityRepository;

        public CityService(ICityRepository cityRepository)
        {
            _cityRepository = cityRepository;
        }

        public async Task<List<Domain.Entities.City>> GetAllCitiesAsync()
            => await _cityRepository.GetAllAsync();

        public async Task CreateCityAsync(CreateCityCommand cmd)
        {
            var city = new Domain.Entities.City
            {
                Name = cmd.Name,
                Country = cmd.Country,
                PostOffice = cmd.PostOffice
            };
            await _cityRepository.AddAsync(city);
        }

        public async Task DeleteCityAsync(int id)
        {
            var city = await _cityRepository.GetByIdAsync(id);
            if (city != null)
                await _cityRepository.DeleteAsync(city);
        }

        public async Task UpdateCityAsync(UpdateCityCommand cmd)
        {
            var city = await _cityRepository.GetByIdAsync(cmd.Id);
            if (city == null) throw new Exception("City not found");

            city.Name = cmd.Name;
            city.Country = cmd.Country;
            city.PostOffice = cmd.PostOffice;

            await _cityRepository.UpdateAsync(city);
        }

        public async Task<List<Domain.Entities.City>> SearchCitiesAsync(string keyword, int page, int pageSize)
            => await _cityRepository.SearchAsync(keyword, page, pageSize);
    }
}