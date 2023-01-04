using Net7CoreApiBoilerplate.Infrastructure.DbUtility;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;
using System.Linq;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Net7CoreApiBoilerplate.Services.VueBoilerplate
{

    public class MockSephirothClientAddress : IEntity
    {
        public long Id { get; set; }

        public string Street { get; set; }
        public string Number { get; set; }
        public string NumberAddition { get; set; }
        public string PostCode { get; set; }
        public string Place { get; set; }

        [NotMapped]
        public string AddressLine1 => $"{Street} {Number} {NumberAddition}".Trim();

    }

    public class MockSephirothClientClientType : IEntity
    {
        public long Id { get; set; }

        public string Name { get; set; }
    }

    public class MockSephirothClient : IEntity
    {
        public long Id { get; set; }
        public bool IsActive { get; set; }

        public string Name { get; set; }
        public string Email { get; set; }
        public string Telephone { get; set; }

        public List<MockSephirothClientAddress> Addresses { get; set; }
        public MockSephirothClientClientType ClientTypeNavigation { get; set; }
    }

    public static class MockSephirothData
    {
        static Random rnd = new Random();

        public static IQueryable<MockSephirothClient> GetMockSephirothData()
        {
            // Addresses
            List<MockSephirothClientAddress> addresses = new List<MockSephirothClientAddress>();
            for (long i = 1; i < 1000; i++)
            {
                addresses.Add(new MockSephirothClientAddress
                {
                    Id = i,
                    Street = $"{i} Street",
                    Number = $"{i}",
                    NumberAddition = $"{i} NumberAddition",
                    PostCode = $"{i} PostCode",
                    Place = $"{i} Place",
                });
            }

            // Client types
            List<MockSephirothClientClientType> clientTypes = new List<MockSephirothClientClientType>();
            for (long i = 1; i < 100; i++)
            {
                clientTypes.Add(new MockSephirothClientClientType
                {
                    Id = i,
                    Name = $"Client type {i}",
                });
            }

            // Clients
            List<MockSephirothClient> retList = new List<MockSephirothClient>();
            for (long i = 0; i < 10000; i++)
            {
                retList.Add(new MockSephirothClient
                {
                    Id = i,
                    IsActive = i % 5 == 0, // every 5th is inactive
                    Name = $"Client {i}",
                    Email = $"{i}@client{i}.com",
                    Telephone = $"{i}{i}{i}-{i}{i}{i}",
                    Addresses = new List<MockSephirothClientAddress>(){
                        addresses[rnd.Next(addresses.Count)]
                    },
                    ClientTypeNavigation = clientTypes[rnd.Next(clientTypes.Count)]
                });
            }

            return retList.AsQueryable();
        }
    }

}
