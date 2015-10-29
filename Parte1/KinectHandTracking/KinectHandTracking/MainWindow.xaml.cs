﻿using Microsoft.Kinect;
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

        Stopwatch countdown;
        Boolean countdownIsStored;

        Boolean gameStarted;

        UIElement[] blockArray;

        #endregion

        #region Constructor

        public MainWindow()
        {
            InitializeComponent();

            countdownIsStored = false;
            gameStarted = false;
            letBoxesAppear();
            
           

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


        void letBoxesAppear()
        {
           /* 
            BitmapImage box = new BitmapImage(new Uri("D:/UNI/WiSe1516/NPI/KinectHandTracking/KinectHandTracking/box.jpg"));

            UIElement hola = new UIElement();

            hola.
            

            canvas.Children.Add()
            */
            Random rnd = new Random();
            int nextmar1= 4;

            if (nextmar1 == 1)
            {
                box1.Visibility = System.Windows.Visibility.Visible;
            }
            if (nextmar1 ==2)
            {
                box2.Visibility = System.Windows.Visibility.Visible;
            }
            if (nextmar1 == 3)
            {
                box3.Visibility = System.Windows.Visibility.Visible;
            }
            if (nextmar1 == 4)
            {
                box4.Visibility = System.Windows.Visibility.Visible;
            }
           //String bla=  Convert.ToString(nextmar1);
        
          // String nextBlock1 = "Block"+ nextmar1;
       
            
            Countdown.Text = nextmar1.ToString();


            if (box4.AreAnyTouchesCaptured)
            {
                Countdown.Text = "Tortuga";
            }
           
           


        }
        void CheckInitialConditions(Point headPoint){
            

            Point invisibleHeadPoint = canvas.PointFromScreen(InvisibleHead.PointToScreen(new Point()));


            invisibleHeadPoint.X = (float)(invisibleHeadPoint.X + InvisibleHead.Width / 2);
            invisibleHeadPoint.Y = (float)(invisibleHeadPoint.Y + InvisibleHead.Height / 2);

            double distance = Math.Sqrt(Math.Pow(headPoint.X > invisibleHeadPoint.X ? invisibleHeadPoint.X - headPoint.X : headPoint.X - invisibleHeadPoint.X, 2) + Math.Pow(headPoint.Y > invisibleHeadPoint.Y ? invisibleHeadPoint.Y - headPoint.Y : headPoint.Y - invisibleHeadPoint.Y, 2));

            if (distance < 61)
            {
                pink.Visibility = System.Windows.Visibility.Hidden;
                green.Visibility = System.Windows.Visibility.Visible;

                //Countdown.Text = "Stay!!!";

                //countdown part :)

                if (countdownIsStored){
                                            

                    int elapsed =  5 - countdown.Elapsed.Seconds;


                    Countdown.Text = elapsed.ToString();

                    if (elapsed < 1)
                    {
                        Countdown.Text = "STAAAART";

                        //Countdown.Foreground =  Control.FontStyleProperty.PropertyType.;

                        //Awesome stuff here

                        green.Visibility = System.Windows.Visibility.Hidden;
                        letBoxesAppear();

                        gameStarted = true;

                    }
                                                
                } 
                else{
                    countdown = new Stopwatch();
                    countdown.Start();
                    countdownIsStored = true;

                }
            }
            else
            {
                countdownIsStored = false;

                pink.Visibility = System.Windows.Visibility.Visible;
                green.Visibility = System.Windows.Visibility.Hidden;

                Countdown.Text = "Depth is OK";
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
                                /*
                                 *  TODO 
                                 * 
                                 *  1. Once in correct position, 7 secs and start game
                                 *  2. Print a huge START!! and remove the silhoute
                                 *  3. Check if head is trackable all the time
                                 *  4. Fancy GUI
                                 *  5. Tutorial
                                 *  6. Problems and solutions
                                 * 
                                 *  lllll - put Super Mario Blocks randomly in the frame appearing
                                 *  llllll - Score
                                 *  lllllll - Time
                                 */


                                //canvas.DrawSkeleton(body, _sensor.CoordinateMapper);

                                Joint head = body.Joints[JointType.Head];
                                Point headPoint = head.Scale(_sensor.CoordinateMapper);

                                //headInfo.Text = headPoint.X.ToString();
                                //marginHead.Text = invisibleHeadPoint.X.ToString();


                                //DistanceBox.Text = distance.ToString();

                                //canvas.DrawLine(head, invisibleHeadPoint, _sensor.CoordinateMapper);


                                if (head.Position.Z > 1.1 && head.Position.Z < 1.31)
                                {
                                    if (!gameStarted) {
                                        CheckInitialConditions(headPoint);

                                        
                                    }
                                    else
                                    {

                                    }
                                }
                                else
                                {
                                    if (!gameStarted) { 
                                        if (head.Position.Z < 1.1)
                                            Countdown.Text = "Back!!!!";
                                        else
                                            Countdown.Text = "Forwardddd!!!!";


                                        countdownIsStored = false;

                                        pink.Visibility = System.Windows.Visibility.Visible;
                                        green.Visibility = System.Windows.Visibility.Hidden;
                                    }

                                }


                                //DepthBox.Text = head.Position.Z.ToString();


                               
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
                                canvas.DrawLine(WristLeftJoint, handLeft, _sensor.CoordinateMapper);
                                canvas.DrawLine(WristRightJoint, handRight, _sensor.CoordinateMapper);

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

        private void TextBox_TextChanged_2(object sender, TextChangedEventArgs e)
        {

        }

        private void TextBox_TextChanged_3(object sender, TextChangedEventArgs e)
        {

        }

        private void First_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void Second_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}
