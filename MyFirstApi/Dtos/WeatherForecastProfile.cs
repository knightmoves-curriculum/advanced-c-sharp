using AutoMapper;

public class WeatherForecastProfile : Profile
{
    public WeatherForecastProfile()
    {
        CreateMap<WeatherForecastDto, WeatherForecast>()
            .ForMember(dest => dest.TemperatureC, 
                       opt => opt.MapFrom(src => (src.TemperatureF - 32) * 5 / 9))
            .ForMember(dest => dest.Alert, 
                       opt => opt.MapFrom(src => src.Alert != null 
                                                 ? new WeatherAlert { AlertMessage = src.Alert } 
                                                 : null))
            .ForMember(dest => dest.Comments, 
                       opt => opt.MapFrom(src => src.Comments != null 
                                                 ? src.Comments.Select(c => new WeatherComment { CommentMessage = c }).ToList() 
                                                 : new List<WeatherComment>()));
        CreateMap<CityDto, City>();
        CreateMap<UserDtoV1, User>();
        CreateMap<UserDtoV2, User>();
    }
}