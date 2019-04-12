using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gldd.AmazonWaterLog
{
    public class AutoPurger
    {
        private readonly IAmazonWaterLogClient _client;

        public AutoPurger(IAmazonWaterLogClient client)
        {
            _client = client;
        }

        /// <summary>
        /// The minimum level where the purge command will be invoked to the bubble.
        /// </summary>
        public float PurgeLevelThreshold { get; set; } = 5;


        public void AutoPurgeIfNecessary()
        {
            if(_client.MeasureLevel() <= PurgeLevelThreshold)
                _client.Purge();
        }














        //public void AutoPurgeIfNecessary()
        //{
        //    var client = new AmazonWaterLogClient();
        //    float measuredLevel = client.MeasureLevel();
        //    if(measuredLevel < PurgeLevelThreshold)
        //    {
        //        client.Purge();
        //    }
        //}
    }
}
