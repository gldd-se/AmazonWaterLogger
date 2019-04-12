using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using Gldd.AmazonWaterLog;

namespace AmazonBubblerInterop
{
    internal static class BubblerServiceChannel
    {
        private static readonly ChannelFactory<IBubblerService> _channelFactory = new ChannelFactory<IBubblerService>(
            new NetNamedPipeBinding() { SendTimeout = TimeSpan.FromMilliseconds(150)}, new Uri("net.pipe://localhost/BubblerService/") + "BubblerService");
        public static void Use(Action<IBubblerService> action)
        {
            var channel = _channelFactory.CreateChannel();
            action(channel);
        }

        public static T Get<T>(Func<IBubblerService, T> func)
        {
            T value = default;

            var channel =(IClientChannel) _channelFactory.CreateChannel();
            bool success = false;
            try
            {
                value = func((IBubblerService)channel);
                channel.Close();
                success = true;
            }
            finally
            {
                if (!success)
                {
                    channel.Abort();
                }
            }

            return value;
        }
    }
}
