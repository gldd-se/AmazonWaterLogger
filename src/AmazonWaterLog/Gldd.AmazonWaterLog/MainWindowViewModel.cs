using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using System.IO;
using System.Configuration;
using System.Threading;

namespace Gldd.AmazonWaterLog
{
    public class MainWindowViewModel : BindableBase
    {
        private readonly BubblerService _bubblerService;
        private readonly AmazonWaterLogClient _client1 = new AmazonWaterLogClient { HostName = Properties.Settings.Default.IPAddress1 };
        private readonly AmazonWaterLogClient _client2 = new AmazonWaterLogClient { HostName = Properties.Settings.Default.IPAddress2 };
        private readonly DispatcherTimer _autoPurgeTimer = new DispatcherTimer();
        private readonly AutoPurger _autoPurger;

        private float DepthDifference;
        private string _ipAddress1ErrorMessage;


        private void DeleteUserConfigIfCorrupted()
        {
            //https://stackoverflow.com/questions/9572243/what-causes-user-config-to-empty-and-how-do-i-restore-without-restarting
            //https://stackoverflow.com/questions/42708868/user-config-corruption
            try
            {   
                ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.PerUserRoamingAndLocal);
            }
            catch (ConfigurationErrorsException ex)
            {
                string userConfigFileName = ex.Filename;
                if (File.Exists(userConfigFileName))
                {
                    File.Delete(userConfigFileName);
                }
            }
        }
        private void UpgradeUserConfigIfNeeded()
        {
            if (Properties.Settings.Default.UpgradeRequired)
            {   
                Properties.Settings.Default.Upgrade();
                Properties.Settings.Default.UpgradeRequired = false;
                Properties.Settings.Default.Save();
                
            }
        }


        public string IPAddress1ErrorMessage
        {
            get
            {
                return _ipAddress1ErrorMessage;
            }
            set
            {
                _ipAddress1ErrorMessage = value;
                RaisePropertyChanged();
            }
        }

        public string IPAddress1
        {
            get
            {
                return _client1.HostName;
            }
            set
            {
                _client1.HostName = value;
                Properties.Settings.Default.IPAddress1 = value;
                Properties.Settings.Default.Save();
            }
        }

        private float _distanceBetween=Properties.Settings.Default.DistanceBetween;
        public  float DistanceBetween
        {
            get
            {   
                //_distanceBetween = Properties.Settings.Default.DistanceBetween;
                return _distanceBetween;
            }

            set
            {
                _distanceBetween = value;
                Properties.Settings.Default.DistanceBetween = value;
                Properties.Settings.Default.Save();

            }

        }



        private string _ipAddress2ErrorMessage;
        public string IPAddress2ErrorMessage
        {
            get
            {
                return _ipAddress2ErrorMessage;
            }
            set
            {
                _ipAddress2ErrorMessage = value;
                RaisePropertyChanged();
            }
        }

        public string IPAddress2
        {
            get
            {
                return _client2.HostName;
            }
            set
            {
                _client2.HostName = value;
                Properties.Settings.Default.IPAddress2 = value;
                Properties.Settings.Default.Save();
            }
        }

        public MainWindowViewModel(BubblerService bubblerService)
        {
            _bubblerService = bubblerService;

            _autoPurger = new AutoPurger(_client1);
            _autoPurger.PurgeLevelThreshold = 2;

            if (!DesignerProperties.GetIsInDesignMode(new DependencyObject()))
            {
               
                if (string.IsNullOrEmpty(_client1.HostName) || string.IsNullOrEmpty( _client2.HostName))
                {
                    _autoPurgeTimer.Interval = TimeSpan.FromSeconds(10);
                }
                else
                {
                    _autoPurgeTimer.Interval = TimeSpan.FromSeconds(2);
                }
                _autoPurgeTimer.Tick += AutoPurgeTimer_Tick;
                _autoPurgeTimer.Start();
            }
        }

        private void AutoPurgeTimer_Tick(object sender, EventArgs e)
        {
            MeasureLevelTrunionAsync();
            MeasureLevelSuctionAsync();
           // _autoPurger.AutoPurgeIfNecessary();
        }

        public ICommand MeasureLevelSuctionCommand => new DelegateCommand(MeasureLevelSuctionAsync);
        public ICommand MeasureLevelTrunionCommand => new DelegateCommand(MeasureLevelTrunionAsync);

        private async void MeasureLevelTrunionAsync()
        {
            try
            {
             LevelTrunion = await _client1.MeasureLevelAsync();
               _bubblerService.LevelTrunion = LevelTrunion;
                DepthDifference = LevelSuction - LevelTrunion;
                DepthDifference = (float)3.19;  //for testing
                if (_distanceBetween < 1) _distanceBetween = 1;
                if (DepthDifference != 0) CalculatedAngle = Math.Asin(DepthDifference / _distanceBetween) * (180.0 / Math.PI);//difference of suction depth and trunion depth
                CalculatedAngle = Math.Round(CalculatedAngle, 1);               // divided by distance between trunion and suction outlet
                _bubblerService.CalculatedAngle = CalculatedAngle;
                IPAddress1ErrorMessage = "";
                
            }
            catch
            {
                IPAddress1ErrorMessage = "Check Trunion Bubbler";
                
            }
        }

        private float _levelTrunion;
        public float LevelTrunion
        {
            get => _levelTrunion;
            set => SetProperty(ref _levelTrunion, value);
        }
        private double _calculatedAngle;
        public double CalculatedAngle
        {
            get => _calculatedAngle;
            set => SetProperty(ref _calculatedAngle, value);

        }


        private async void MeasureLevelSuctionAsync()
        {
            try
            {
               LevelSuction = await _client2.MeasureLevelAsync();
                _bubblerService.LevelSuction=LevelSuction;
                if (_distanceBetween < 1) _distanceBetween = 1;
                DepthDifference = LevelSuction - LevelTrunion;
                if (DepthDifference != 0) CalculatedAngle = Math.Asin(DepthDifference / _distanceBetween) * (180.0 / Math.PI);
                CalculatedAngle = Math.Round(CalculatedAngle, 1);
                _bubblerService.CalculatedAngle = CalculatedAngle;
                IPAddress2ErrorMessage = "";
              
            }
            catch
            {
                IPAddress2ErrorMessage = "Check Suction Bubbler";
               
            }
        }
        private float _levelSuction;
        public float LevelSuction
        {
            get => _levelSuction;
            set => SetProperty(ref _levelSuction, value);
        }


        public ICommand PurgeCommand => new DelegateCommand(Purge);

        private void Purge()
        {
            _client1.Purge();
        }
    }
}
