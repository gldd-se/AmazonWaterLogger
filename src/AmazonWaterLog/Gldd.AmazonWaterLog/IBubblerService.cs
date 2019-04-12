using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Gldd.AmazonWaterLog
{
    [ServiceContract]
    public interface IBubblerService
    {
        [OperationContract]
        float GetLevelTrunion();

        [OperationContract]
        float GetLevelSuction ();

        [OperationContract]
        double GetCalculatedAngle();
    }

    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class BubblerService : IBubblerService
    {
        public double GetCalculatedAngle()
        {
            return CalculatedAngle;
        }

        public float GetLevelSuction()
        {
            return LevelSuction;
        }

        public float GetLevelTrunion()
        {
            return LevelTrunion;
        }  

        public double CalculatedAngle { get; set; }

        public float LevelSuction { get; set; }

        public float LevelTrunion { get; set; }
    }  
}
