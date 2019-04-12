using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Gldd.AmazonWaterLog.Tests.IntegrationTests
{
    public class AmazonWaterLogClientTests
    {
        //[Fact]
        public void Purge_must_send_purge_command_to_device()
        {
            var client = new AmazonWaterLogClient();
            client.HostName = "10.192.1.137";

            client.Purge();
            //this test will always pass but we should see the actual device purging.
        }

        //[Fact]
        public void MeasureLevel_when_outlet_tube_is_in_water_must_return_a_value_greater_than_0()
        {
            float expectedLevel = 4.67f;

            var client = new AmazonWaterLogClient();
            client.HostName = "10.192.1.137";

            float measuredLevel = client.MeasureLevel();

            Assert.Equal(expectedLevel, measuredLevel);
        }
    }
}
