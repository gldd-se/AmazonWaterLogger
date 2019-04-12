using System;
using System.Runtime.InteropServices;

namespace AmazonBubblerInterop
{
    [InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
    [Guid("A9371DD2-145C-4800-A1DC-02FC23F70DE3")]
    [ComVisible(true)]
    public interface IBubblerInterop
    {
        [DispId(1)]
        float LevelTrunion { get; }

        [DispId(2)]
        float LevelSuction { get; }

        [DispId(3)]
        double CalculatedAngle { get; }
    }

    [ClassInterface(ClassInterfaceType.None)]
    [Guid("AC5BBE5F-743C-4FFA-B7DE-DA7A08963058")]
    [ComVisible(true)]
    public class BubblerInterop : IBubblerInterop
    {
        public float LevelTrunion => BubblerServiceChannel.Get(s => s.GetLevelTrunion());

        public float LevelSuction => BubblerServiceChannel.Get(s => s.GetLevelSuction());

        public double CalculatedAngle => BubblerServiceChannel.Get(s => s.GetCalculatedAngle());
    }
}
