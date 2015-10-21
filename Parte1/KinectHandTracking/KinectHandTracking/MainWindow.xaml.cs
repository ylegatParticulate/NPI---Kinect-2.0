using Microsoft.Kinect;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace KinectHandTracking
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Members

        KinectSensor _sensor;
        MultiSourceFrameReader _reader;
        IList<Body> _bodies;

        #endregion

        #region Constructor

        public MainWindow()
        {
            InitializeComponent();
        }

        #endregion

        #region Event handlers

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _sensor = KinectSensor.GetDefault();

            if (_sensor != null)
            {
                _sensor.Open();

                _reader = _sensor.OpenMultiSourceFrameReader(FrameSourceTypes.Color | FrameSourceTypes.Depth | FrameSourceTypes.Infrared | FrameSourceTypes.Body);
                _reader.MultiSourceFrameArrived += Reader_MultiSourceFrameArrived;
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            if (_reader != null)
            {
                _reader.Dispose();
            }

            if (_sensor != null)
            {
                _sensor.Close();
            }
        }

        void Reader_MultiSourceFrameArrived(object sender, MultiSourceFrameArrivedEventArgs e)
        {
            var reference = e.FrameReference.AcquireFrame();

            // Color
            using (var frame = reference.ColorFrameReference.AcquireFrame())
            {
                if (frame != null)
                {
                    camera.Source = frame.ToBitmap();
                }
            }

            // Body
            using (var frame = reference.BodyFrameReference.AcquireFrame())
            {
                if (frame != null)
                {
                    canvas.Children.Clear();

                    _bodies = new Body[frame.BodyFrameSource.BodyCount];

                    frame.GetAndRefreshBodyData(_bodies);

                    foreach (var body in _bodies)
                    {
                        if (body != null)
                        {
                            if (body.IsTracked)
                            {
                                //canvas.DrawSkeleton(body, _sensor.CoordinateMapper);


                                Joint head = body.Joints[JointType.Head];
                                headInfo.Text = head.Position.X.ToString();
                                marginHead.Text = InvisibleHead.Margin.Left.ToString();


                               
                                // Find the joints
                                // fucking joints for everything
                               
                                Joint SpineShoulderJoint = body.Joints[JointType.SpineShoulder];
                                Joint ShoulderLeft = body.Joints[JointType.ShoulderLeft];
                                Joint ShoulderRight= body.Joints[JointType.ShoulderRight];

                                Joint ElbowRightJoint = body.Joints[JointType.ElbowRight];
                                Joint ElbowLeftJoint = body.Joints[JointType.ElbowLeft];
                                Joint WristLeftJoint = body.Joints[JointType.WristLeft];
                                Joint WristRightJoint = body.Joints[JointType.WristRight];

                                Joint handRight = body.Joints[JointType.HandRight];
                                Joint thumbRight = body.Joints[JointType.ThumbRight];

                                Joint handLeft = body.Joints[JointType.HandLeft];
                                Joint thumbLeft = body.Joints[JointType.ThumbLeft];

                                canvas.DrawPoint(head,_sensor.CoordinateMapper);
                                canvas.DrawPoint(SpineShoulderJoint,_sensor.CoordinateMapper);
                                canvas.DrawPoint(ShoulderLeft, _sensor.CoordinateMapper);
                                canvas.DrawPoint(ShoulderRight, _sensor.CoordinateMapper);

                                canvas.DrawPoint(ElbowRightJoint, _sensor.CoordinateMapper);
                                canvas.DrawPoint(ElbowLeftJoint, _sensor.CoordinateMapper);
                                canvas.DrawPoint(WristLeftJoint, _sensor.CoordinateMapper);
                                canvas.DrawPoint(WristRightJoint, _sensor.CoordinateMapper);                             
                                
                                // Draw hands and thumbs
                                canvas.DrawPoint(handRight, _sensor.CoordinateMapper);
                                canvas.DrawPoint(handLeft, _sensor.CoordinateMapper);
                                //canvas.DrawPoint(thumbRight, _sensor.CoordinateMapper);
                                //canvas.DrawPoint(thumbLeft, _sensor.CoordinateMapper);

                                canvas.DrawLine(head,SpineShoulderJoint, _sensor.CoordinateMapper);
                                canvas.DrawLine(SpineShoulderJoint, ShoulderLeft, _sensor.CoordinateMapper);
                                canvas.DrawLine(SpineShoulderJoint, ShoulderRight, _sensor.CoordinateMapper);
                                canvas.DrawLine(ShoulderLeft, ElbowLeftJoint, _sensor.CoordinateMapper);
                                canvas.DrawLine(ShoulderRight, ElbowRightJoint, _sensor.CoordinateMapper);
                                canvas.DrawLine(ElbowRightJoint, WristRightJoint, _sensor.CoordinateMapper);
                                canvas.DrawLine(ElbowLeftJoint, WristLeftJoint, _sensor.CoordinateMapper);
                                canvas.DrawLine(ElbowLeftJoint, handLeft, _sensor.CoordinateMapper);
                                canvas.DrawLine(ElbowRightJoint, handRight, _sensor.CoordinateMapper);

                                if (head.Position.Y < handLeft.Position.Y)
                                {
                                    Hola.Text = "HOLAAA";
                                }
                                else
                                {
                                    Hola.Text = "Not hola";

                                }


                                //canvas.DrawSkeleton(body, _sensor.CoordinateMapper);
                                // Find the hand states
                                string rightHandState = "-";
                                string leftHandState = "-";

                                switch (body.HandRightState)
                                {
                                    case HandState.Open:
                                        rightHandState = "Open";
                                        break;
                                    case HandState.Closed:
                                        rightHandState = "Closed";
                                        break;
                                    case HandState.Lasso:
                                        rightHandState = "Lasso";
                                        break;
                                    case HandState.Unknown:
                                        rightHandState = "Unknown...";
                                        break;
                                    case HandState.NotTracked:
                                        rightHandState = "Not tracked";
                                        break;
                                    default:
                                        break;
                                }

                                switch (body.HandLeftState)
                                {
                                    case HandState.Open:
                                        leftHandState = "Open";
                                        break;
                                    case HandState.Closed:
                                        leftHandState = "Closed";
                                        break;
                                    case HandState.Lasso:
                                        leftHandState = "Lasso";
                                        break;
                                    case HandState.Unknown:
                                        leftHandState = "Unknown...";
                                        break;
                                    case HandState.NotTracked:
                                        leftHandState = "Not tracked";
                                        break;
                                    default:
                                        break;
                                }

                                tblRightHandState.Text = rightHandState;
                                tblLeftHandState.Text = leftHandState;
                            }
                        }
                    }
                }
            }
        }

        #endregion

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void headInfo_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void TextBox_TextChanged_1(object sender, TextChangedEventArgs e)
        {

        }
    }
}
