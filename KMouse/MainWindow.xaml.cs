using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Runtime.InteropServices;
using Microsoft.Kinect;
using Coding4Fun.Kinect.Wpf;

namespace KMouse {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        [DllImport("User32.dll")]
        private static extern bool SetCursorPos(int X, int Y);
        //[DllImport("User32.dll")]
        //private static extern int ShowCursor(bool bShow);

        private bool isGameLauched = false;

        KinectSensor _sensor;
        public MainWindow() {
            InitializeComponent();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e) {
            _sensor = KinectSensor.KinectSensors.FirstOrDefault();
            //KinectSensor.KinectSensors.Where(x => x.Status == KinectStatus.Connected).FirstOrDefault(); 
            if (_sensor != null) {
                _sensor.SkeletonFrameReady += KinectSkeletonFrameReady;
                _sensor.SkeletonStream.Enable();
                _sensor.Start();
                Status.Text = "KinectStatus : ON";
            }
            else {
                Status.Text = "KinectStatus : NOT CONNECTED";
            }
        }
        private void Window_Unloaded(object sender, RoutedEventArgs e) {
            if (_sensor != null) {
                _sensor.Stop();
                //ShowCursor(true);
            }
        }
        void KinectSkeletonFrameReady(object sender, SkeletonFrameReadyEventArgs e) {
            using (var frame = e.OpenSkeletonFrame()) {
                if (frame != null) {
                    Status.Text = "KinectStatus : ACTIVE";
                    LaunchButton.IsEnabled = true;
                    Skeleton[] skeletons = new Skeleton[frame.SkeletonArrayLength];
                    frame.CopySkeletonDataTo(skeletons);
                    var skeleton = skeletons.Where(s => s.TrackingState == SkeletonTrackingState.Tracked).FirstOrDefault();
                    if (skeleton != null && isGameLauched) {
                        SetCursorPos((int)skeleton.Joints[JointType.HandRight].ScaleTo(1366, 768).Position.X, (int)skeleton.Joints[JointType.HandRight].ScaleTo(1366, 768).Position.Y);
                        //ShowCursor(false);
                    }
                }
            }
        }
        private void LaunchGame(object sender, RoutedEventArgs e) {
            isGameLauched = true;
            System.Diagnostics.Process.Start("Fruit.exe");
        }
    }
}
