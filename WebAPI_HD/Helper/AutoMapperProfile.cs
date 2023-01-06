using AutoMapper;
using WebAPI_HD.Model;

namespace WebAPI_HD.Helper
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            // User -> AuthenticateResponse
            CreateMap<ApplicationUser, LoginResponse>();

            // RegisterRequest -> User
            CreateMap<RegisterRequest, ApplicationUser>();

            // UpdateRequest -> User
            CreateMap<UpdateRequest, ApplicationUser>()
                .ForAllMembers(x => x.Condition(
                    (src, dest, prop) =>
                    {
                        // ignore null & empty string properties
                        if (prop == null) return false;
                        if (prop.GetType() == typeof(string) && string.IsNullOrEmpty((string)prop)) return false;

                        return true;
                    }
                ));
        }
    }
}
