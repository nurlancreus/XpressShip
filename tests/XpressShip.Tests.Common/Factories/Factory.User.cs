using Bogus;
using SenderEntity = XpressShip.Domain.Entities.Users.Sender;
using ApiClientEntity = XpressShip.Domain.Entities.ApiClient;
using DataConstants = XpressShip.Tests.Common.Constants.Constants;

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
