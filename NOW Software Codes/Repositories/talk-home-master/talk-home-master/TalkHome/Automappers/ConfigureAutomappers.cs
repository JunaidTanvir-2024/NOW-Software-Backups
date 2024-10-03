using AutoMapper;
using TalkHome.Models;
using TalkHome.Models.ViewModels.Payment;
using TalkHome.Models.WebApi;
using TalkHome.Models.WebApi.App;
using TalkHome.Models.WebApi.DTOs;
using TalkHome.Models.WebApi.Payment;

namespace TalkHome.Automappers
{
    /// <summary>
    /// Contains methods to create automaps
    /// </summary>
    internal static class ConfigureAutomappers
    {
        /// <summary>
        /// Creates maps
        /// </summary>
        internal static void Maps()
        {
            // Payment

            Mapper.CreateMap<StartPaymentViewModel, StartPaymentRequestDTO>();

            Mapper.CreateMap<BillingAddress, AddressModel>()
                .ForMember(dest => dest.addressL1, opt => opt.MapFrom(src => src.addressLine1))
                .ForMember(dest => dest.addressL2, opt => opt.MapFrom(src => src.addressLine2));

            Mapper.CreateMap<JWTPayload, StartPaymentRequestDTO>()
                .ForMember(dest => dest.CurrencyCode, opt => opt.MapFrom(src => src.currency))
                .ForMember(dest => dest.Locale, opt => opt.MapFrom(src => src.TwoLetterISORegionName))
                .ForMember(dest => dest.Msisdn, opt => opt.MapFrom(src => src.Checkout.Reference));

            // App accounts

            Mapper.CreateMap<AppUserModel, FullNameModel>()
                .ForMember(dest => dest.Salutation, opt => opt.MapFrom(src => src.title))
                .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.fname))
                .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.lname))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.email));

            Mapper.CreateMap<AppUserModel, AddressModel>()
                .ForMember(dest => dest.addressL1, opt => opt.MapFrom(src => src.addr1))
                .ForMember(dest => dest.addressL2, opt => opt.MapFrom(src => src.addr2))
                .ForMember(dest => dest.city, opt => opt.MapFrom(src => src.addr4))
                .ForMember(dest => dest.postCode, opt => opt.MapFrom(src => src.postal_code))
                .ForMember(dest => dest.county, opt => opt.MapFrom(src => src.addr5));

            // Website accounts

            Mapper.CreateMap<ProductCodes, AccountCodes>()
                .ForMember(dest => dest.Reference, opt => opt.MapFrom(src => src.reference))
                .ForMember(dest => dest.ProductCode, opt => opt.MapFrom(src => src.productCode));

            Mapper.CreateMap<AccountDetailsResponseDTO, FullNameModel>()
                .ForMember(dest => dest.Salutation, opt => opt.MapFrom(src => src.title))
                .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.firstName))
                .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.lastName))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.email));

            Mapper.CreateMap<MiPayCustomerModel, FullNameModel>()
                .ForMember(dest => dest.Salutation, opt => opt.MapFrom(src => src.salutation))
                .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.firstName))
                .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.lastName))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.emailAddress));

            // Mail order

            Mapper.CreateMap<MailOrderRequest, MailOrderRequestDTO>()
                .ForMember(dest => dest.title, opt => opt.MapFrom(src => src.Salutation))
                .ForMember(dest => dest.firstName, opt => opt.MapFrom(src => src.FirstName))
                .ForMember(dest => dest.lastName, opt => opt.MapFrom(src => src.LastName))
                .ForMember(dest => dest.email, opt => opt.MapFrom(src => src.EmailAddress))
                .ForMember(dest => dest.addressL1, opt => opt.MapFrom(src => src.AddressLine1))
                .ForMember(dest => dest.addressL2, opt => opt.MapFrom(src => src.AddressLine2))
                .ForMember(dest => dest.city, opt => opt.MapFrom(src => src.City))
                .ForMember(dest => dest.county, opt => opt.MapFrom(src => src.CountyOrProvince))
                .ForMember(dest => dest.postCode, opt => opt.MapFrom(src => src.PostalCode))
                .ForMember(dest => dest.country, opt => opt.MapFrom(src => src.CountryCode))
                .ForMember(dest => dest.product, opt => opt.MapFrom(src => src.MailOrderProduct))
                .ForMember(dest => dest.isSimSwap, opt => opt.MapFrom(src => src.IsSimSwap));

            Mapper.CreateMap<CallingCardCreditOrderRequest, MailOrderRequestDTO>()
                .ForMember(dest => dest.title, opt => opt.MapFrom(src => src.Salutation))
                .ForMember(dest => dest.firstName, opt => opt.MapFrom(src => src.FirstName))
                .ForMember(dest => dest.lastName, opt => opt.MapFrom(src => src.LastName))
                .ForMember(dest => dest.email, opt => opt.MapFrom(src => src.EmailAddress))
                .ForMember(dest => dest.addressL1, opt => opt.MapFrom(src => src.AddressLine1))
                .ForMember(dest => dest.addressL2, opt => opt.MapFrom(src => src.AddressLine2))
                .ForMember(dest => dest.city, opt => opt.MapFrom(src => src.City))
                .ForMember(dest => dest.county, opt => opt.MapFrom(src => src.CountyOrProvince))
                .ForMember(dest => dest.postCode, opt => opt.MapFrom(src => src.PostalCode))
                .ForMember(dest => dest.country, opt => opt.MapFrom(src => src.CountryCode))
                .ForMember(dest => dest.product, opt => opt.MapFrom(src => src.MailOrderProduct));

            // Sign up

            Mapper.CreateMap<SignUpRequest, SignUpRequestDTO>()
                .ForMember(dest => dest.email, opt => opt.MapFrom(src => src.EmailAddress))
                .ForMember(dest => dest.password, opt => opt.MapFrom(src => src.Password))
                .ForMember(dest => dest.confirmPassword, opt => opt.MapFrom(src => src.ConfirmPassword))
                .ForMember(dest => dest.title, opt => opt.MapFrom(src => src.Salutation))
                .ForMember(dest => dest.firstName, opt => opt.MapFrom(src => src.FirstName))
                .ForMember(dest => dest.lastName, opt => opt.MapFrom(src => src.LastName))
                .ForMember(dest => dest.isSubscribedToNewsletter, opt => opt.MapFrom(src => src.SubscribeSignUp));

            // Promo sign up

            Mapper.CreateMap<PromoSignUpRequest, PromoSignUpRequestDTO>()
                .ForMember(dest => dest.Msisdn, opt => opt.MapFrom(src => src.Number));


            // Update Address

            Mapper.CreateMap<AddressModel, UpdateAddressRequestDTO>()
              .ForMember(dest => dest.AddressL1, opt => opt.MapFrom(src => src.addressL1))
              .ForMember(dest => dest.AddressL2, opt => opt.MapFrom(src => src.addressL2))
              .ForMember(dest => dest.City, opt => opt.MapFrom(src => src.city))
              .ForMember(dest => dest.PostCode, opt => opt.MapFrom(src => src.postCode))
              .ForMember(dest => dest.County, opt => opt.MapFrom(src => src.county))
              .ForMember(dest => dest.Country, opt => opt.MapFrom(src => src.country));

        }
    }
}
