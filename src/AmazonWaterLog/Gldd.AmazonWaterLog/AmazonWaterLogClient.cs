using Modbus.Device;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Gldd.AmazonWaterLog
{
    public interface IAmazonWaterLogClient
    {
        string HostName { get; set; }
        void Purge();
        float MeasureLevel();
    }

    public class AmazonWaterLogClient : IAmazonWaterLogClient
    {
        private const int _portNumber = 502;
        private const byte _slaveId = 1;

        public string HostName { get; set; }

        public void Purge()
        {
            using (TcpClient tcpClient = new TcpClient(HostName, _portNumber))
            using (ModbusIpMaster bubbler = ModbusIpMaster.CreateIp(tcpClient))
            {
                ushort purgeRegister = 24;
                ushort purgeValue = 1;
                bubbler.WriteSingleRegister(_slaveId, purgeRegister, purgeValue);
            }
        }

        public float MeasureLevel()
        {
            using (TcpClient tcpClient = new TcpClient())
            using (ModbusIpMaster bubbler = ModbusIpMaster.CreateIp(tcpClient))
            {
                tcpClient.Connect(HostName, _portNumber);
                ushort startAddress = 29;
                ushort registerCount = 2;

                ushort[] result = bubbler.ReadHoldingRegisters(_slaveId, startAddress, registerCount);

                byte[] byte0 = BitConverter.GetBytes(result[0]);
                byte[] byte1 = BitConverter.GetBytes(result[1]);
                float level = System.BitConverter.ToSingle(byte0.Concat(byte1).ToArray(), 0);
                return level;
            }
        }

        public async Task<float> MeasureLevelAsync()
        {
            using (TcpClient tcpClient = new TcpClient())
            using (ModbusIpMaster bubbler = ModbusIpMaster.CreateIp(tcpClient))
            {
                await tcpClient.ConnectAsync(HostName, _portNumber);

                ushort startAddress = 29;
                ushort registerCount = 2;

                ushort[] result = bubbler.ReadHoldingRegisters(_slaveId, startAddress, registerCount);

                byte[] byte0 = BitConverter.GetBytes(result[0]);
                byte[] byte1 = BitConverter.GetBytes(result[1]);
                float level = System.BitConverter.ToSingle(byte0.Concat(byte1).ToArray(), 0);
                return level;
            }
        }
    }
}
