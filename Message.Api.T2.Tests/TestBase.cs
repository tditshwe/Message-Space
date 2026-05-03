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
    }
}
