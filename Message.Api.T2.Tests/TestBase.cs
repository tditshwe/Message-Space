using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using Message.Api.T2.Tests.Tools;

namespace Message.Api.T2.Tests
{
    internal class TestBase
    {
        protected ApiClient Client;

        [SetUp]
        public void Setup()
        {
            Client = new ApiClient();
        }

        [TearDown]
        public void TearDown()
        {
            Client!.Dispose();
        }

        protected class LoginResponse
        {
            public string? Username { get; set; }
            public string? Name { get; set; }
            public string? Token { get; set; }
            public string? Status { get; set; }
            public string? ImageUrl { get; set; }
        }
    }
}
