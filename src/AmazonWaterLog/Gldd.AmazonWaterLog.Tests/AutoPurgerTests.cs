using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Gldd.AmazonWaterLog.Tests
{
    public class AutoPurgerTests
    {
        [Fact]
        public void Auto_purge_if_less_than_or_equal_to_threshold()
        {
            var fakeClient = new FakeClient();
            
            var purger = new AutoPurger(fakeClient);

            purger.PurgeLevelThreshold = 5;
            fakeClient._MeasureLevel = 2;

            purger.AutoPurgeIfNecessary();

            Assert.True(fakeClient.IsPurgeInvoked);
        }

        [Fact]
        public void Should_not_auto_purge_if_level_is_greater_than_theshold()
        {
            var fakeClient = new FakeClient();

            var purger = new AutoPurger(fakeClient);

            purger.PurgeLevelThreshold = 5;
            fakeClient._MeasureLevel = 6;

            purger.AutoPurgeIfNecessary();

            Assert.False(fakeClient.IsPurgeInvoked);
        }
    }

    public class FakeClient : IAmazonWaterLogClient
    {
        public string HostName { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public float _MeasureLevel { get; set; }
        public float MeasureLevel() => _MeasureLevel;

        public bool IsPurgeInvoked { get; private set; }
        public void Purge()
        {
            IsPurgeInvoked = true;
        }
    }
}
