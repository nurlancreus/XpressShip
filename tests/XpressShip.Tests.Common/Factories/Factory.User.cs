using Bogus;
using SenderEntity = XpressShip.Domain.Entities.Users.Sender;
using AdminEntity = XpressShip.Domain.Entities.Users.Admin;
using ApiClientEntity = XpressShip.Domain.Entities.ApiClient;
using DataConstants = XpressShip.Tests.Common.Constants.Constants;
using XpressShip.Application.Features.Auth.Admin.Register;
using XpressShip.Application.Features.Addresses.DTOs;
using XpressShip.Application.Features.ApiClients.Commands.Create;

namespace XpressShip.Tests.Common.Factories
{
    public static partial class Factory
    {
        public static class ApiClient
        {
            private static readonly Faker _faker = new();

            public static (ApiClientEntity client, string rawSecretkey) CreateFakeApiClient()
            {
                return ApiClientEntity.Create(
                   _faker.Company.CompanyName(),
                    _faker.Internet.Email()
                );
            }

            public static (ApiClientEntity client, string rawSecretkey) GenerateApiClient(bool withAddress = false)
            {
                var companyName = DataConstants.ApiClient.CompanyName;
                var email = DataConstants.ApiClient.Email;

                var (client, rawSecretKey) = ApiClientEntity.Create(companyName, email);


                if (withAddress) client.Address = Address.GenerateOriginAddress();

                return (client, rawSecretKey);
            }

            public static CreateApiClientCommand GenerateApiClientCommand()
            {
                return new CreateApiClientCommand
                {
                    CompanyName = "Test Company",
                    Email = "Test@example.com",
                    Address = Address.GenerateAddressCommand()
                };
            }

            public static CreateApiClientCommand GenerateInValidApiClientCommand()
            {
                return new CreateApiClientCommand
                {
                    CompanyName = string.Empty,
                    Email = "invalid.email",
                    Address = Address.GenerateInValidAddressCommand()
                };
            }
        }

        public static class Admin
        {
            private static readonly Faker _faker = new();

            public static AdminEntity CreateFakeSender()
            {
                return AdminEntity.Create(
                    _faker.Name.FirstName(),
                    _faker.Name.LastName(),
                    _faker.Internet.UserName(),
                    _faker.Internet.Email(),
                    _faker.Phone.PhoneNumber()
                );
            }

            public static RegisterAdminCommand GenerateValidRegisterRequest()
            {
                return new RegisterAdminCommand()
                {
                    FirstName = DataConstants.Admin.FirstName,
                    LastName = DataConstants.Admin.LastName,
                    UserName = DataConstants.Admin.UserName,
                    Email = DataConstants.Admin.Email,
                    Phone = DataConstants.Admin.Phone,
                    Password = "StrongPassword12345$",
                    ConfirmPassword = "StrongPassword12345$"
                };
            }

            public static RegisterAdminCommand GenerateInValidRegisterRequest()
            {
                return new RegisterAdminCommand()
                {
                    FirstName = string.Empty,
                    LastName = string.Empty,
                    UserName = string.Empty,
                    Email = string.Empty,
                    Phone = string.Empty,
                    Password = "Invalid",
                    ConfirmPassword = "Invalid"
                };
            }

            public static AdminEntity GenerateAdmin()
            {
                var firstName = DataConstants.Admin.FirstName;
                var lastName = DataConstants.Admin.LastName;
                var userName = DataConstants.Admin.UserName;
                var email = DataConstants.Admin.Email;
                var phone = DataConstants.Admin.Phone;

                return AdminEntity.Create(firstName, lastName, userName, email, phone);
            }
        }
        public static class Sender
        {
            private static readonly Faker _faker = new();

            public static SenderEntity CreateFakeSender()
            {
                return SenderEntity.Create(
                    _faker.Name.FirstName(),
                    _faker.Name.LastName(),
                    _faker.Internet.UserName(),
                    _faker.Internet.Email(),
                    _faker.Phone.PhoneNumber()
                );
            }

            public static SenderEntity GenerateSender(bool withAddress = false)
            {
                var firstName = DataConstants.Sender.FirstName;
                var lastName = DataConstants.Sender.LastName;
                var userName = DataConstants.Sender.UserName;
                var email = DataConstants.Sender.Email;
                var phone = DataConstants.Sender.Phone;

                var sender = SenderEntity.Create(firstName, lastName, userName, email, phone);

                if (withAddress) sender.Address = Address.GenerateOriginAddress();

                return sender;
            }
        }
    }
}
