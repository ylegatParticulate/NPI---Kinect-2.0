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

        public static KinectSensor _sensor;
        MultiSourceFrameReader _reader;
        IList<Body> _bodies;

        static Stopwatch countdown;
        Boolean countdownIsStored;

//        Boolean gameStarted;
	
	    enum GameState { Initialize, MainScreen, InitialPosition, ShowStart, Running, Ended };
	    GameState actualState;

	//Hello World gesture

        struct WaveHand {
		    public static Boolean isWaving;

		    public Joint hand;
		    public Joint elbow;

		    public Boolean waveRight;
		    public Boolean waveLeft;
		    public int waveCount;
		    public Stopwatch waveWatch;
        }

        WaveHand waveRightHand;
        WaveHand waveLeftHand;

        //hand Rect
        public static Rect bodyRect;

        //Intersection Rect
        public static Rect intersectionRect;
        public static Size intersectionSize;

        //Random
        Random randomGenerator;

        //Struct for Rectangles and their elapsed time
        class structRectangle{
            public Rectangle rect;
            public TimeSpan beginTime;
            public HandState handState;
            public string position;
            public Boolean gestureTried;

            public structRectangle() {
                rect = new Rectangle();
                beginTime = new TimeSpan();
                handState = new HandState();
                position = "";
                gestureTried = false;
            }

            public structRectangle(Color color)
            {
                rect = new Rectangle
                {
                    Width = 200,
                    Height = 200,

                    StrokeThickness = 8,
                    Stroke = new SolidColorBrush(color)
                };

                beginTime = countdown.Elapsed;
                position = "";

            }
        }

        //Struct Array
        structRectangle[] structRectangleArray;

        //Joint array
        Joint[] jointArray;

        //HandStates
        HandState leftHandState;
        HandState rightHandState;

        //Rectangle position List
        List<string> positionList;

        //Score
        int score;

        //Rectangle Speed timer lol
        int lowRandom;

        #endregion

        #region Constructor

        public MainWindow()
        {
            InitializeComponent();

            countdownIsStored = false;

	        actualState = GameState.Initialize;

	        countdown = new Stopwatch();

           
	        //Hello World gesture
            waveRightHand = new WaveHand();

            waveRightHand.waveRight = false;
            waveRightHand.waveLeft = false;
            waveRightHand.waveCount = 0;
            waveRightHand.waveWatch = new Stopwatch();
            


            waveLeftHand = new WaveHand();

            waveLeftHand.waveRight = false;
            waveLeftHand.waveLeft = false;
            waveLeftHand.waveCount = 0;
            waveLeftHand.waveWatch = new Stopwatch();

            WaveHand.isWaving = false;

            randomGenerator = new Random();

            structRectangleArray = new structRectangle[3];

            bodyRect = new Rect(new Size(40, 40));

            intersectionSize = new Size(0,0);
            intersectionRect = new Rect(0,0,0,0);

            structRectangleArray[0] = new structRectangle(Colors.Pink);
            structRectangleArray[0].handState = HandState.Closed;
            structRectangleArray[0].position = "3:4";

            structRectangleArray[1] = new structRectangle(Colors.Blue);
            structRectangleArray[1].handState = HandState.Lasso;
            structRectangleArray[1].position = "5:4";

            structRectangleArray[2] = new structRectangle(Colors.Green);
            structRectangleArray[2].handState = HandState.Open;
            structRectangleArray[0].position = "7:4";


            jointArray = new Joint[3];
            positionList = new List<string>();

            score = 0;
            lowRandom = 11;
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

        //kalibrierung
        void CheckInitialConditions(Joint head){            
		    if (head.Position.Z > 1.1 && head.Position.Z < 1.31){

                Point headPoint = head.Scale(_sensor.CoordinateMapper);
			    Point invisibleHeadPoint = canvas.PointFromScreen(InvisibleHead.PointToScreen(new Point()));

			    invisibleHeadPoint.X = (float)(invisibleHeadPoint.X + InvisibleHead.Width / 2);
			    invisibleHeadPoint.Y = (float)(invisibleHeadPoint.Y + InvisibleHead.Height / 2);

			    double distance = Math.Sqrt(
				    Math.Pow(
					    headPoint.X > invisibleHeadPoint.X ?
					    invisibleHeadPoint.X - headPoint.X :
					    headPoint.X - invisibleHeadPoint.X
				    , 2)
				     +
				    Math.Pow(
					    headPoint.Y > invisibleHeadPoint.Y ?
					    invisibleHeadPoint.Y - headPoint.Y :
					    headPoint.Y - invisibleHeadPoint.Y
				    , 2)
			    );

			    if (distance < 61){
				    pink.Visibility = System.Windows.Visibility.Hidden;
				    green.Visibility = System.Windows.Visibility.Visible;

				    if (countdownIsStored){
				        int elapsed =  5 - countdown.Elapsed.Seconds;

				        Countdown.Text = elapsed.ToString();

				        if (elapsed < 1){
					        countdown.Reset();
					        countdown.Start();

					        Countdown.Text = "STAAAART";

					        green.Visibility = System.Windows.Visibility.Hidden;
					        

					        actualState = GameState.ShowStart;
				        }
				    } 
				    else{
		                countdown.Reset();
				        countdown.Start();
				        countdownIsStored = true;
				    }
			    }
			    else{
				    countdownIsStored = false;

				    pink.Visibility = System.Windows.Visibility.Visible;
				    green.Visibility = System.Windows.Visibility.Hidden;

				    Countdown.Text = "Depth is OK";
			    }

		    }
		    else{
			    if (head.Position.Z < 1.1)
			        Countdown.Text = "Back!!!!";
			    else
			        Countdown.Text = "Forwardddd!!!!";

			    countdownIsStored = false;

			    pink.Visibility = System.Windows.Visibility.Visible;
			    green.Visibility = System.Windows.Visibility.Hidden;
		    }
        }

	    Boolean InGameConditions(Joint head){  //Check correct depth during the game
		    if(head.Position.Z < 1.0)
			    Countdown.Text = "Please Go Back";
            else if (head.Position.Z > 1.35)
                Countdown.Text = "Please move forward";
            else
            {
                countdown.Start();
                Countdown.Text = "";
                return true;
            }

            countdown.Stop();

		    return false;
	    }

	    void WaveWorld(ref WaveHand waveHand){
        	    if (waveHand.hand.Position.Y > waveHand.elbow.Position.Y && waveHand.waveWatch.Elapsed.Seconds < 1){

			        if (waveHand.hand.Position.X > waveHand.elbow.Position.X){

				        if (!waveHand.waveRight){
				            waveHand.waveWatch.Reset();
				            waveHand.waveWatch.Start();

				            waveHand.waveRight = true;
				            waveHand.waveLeft = false;

				            waveHand.waveCount++;      
				        }
			        }
			        else{
				        if (!waveHand.waveLeft){
				            waveHand.waveWatch.Reset();
				            waveHand.waveWatch.Start();

				            waveHand.waveRight = false;
				            waveHand.waveLeft = true;

				            waveHand.waveCount++;
				    
				        }
			        }

			        if (waveHand.waveCount > 4) {
				        Countdown.Text = "Hello World!!";

				        WaveHand.isWaving = true;
			        }
		    }
		    else{
		        waveHand.waveRight = false;
		        waveHand.waveLeft = false;
		        waveHand.waveCount = 0;

		        if(!WaveHand.isWaving)
			        Countdown.Text = "Wave!!";

		        waveHand.waveWatch.Reset();

		        WaveHand.isWaving = false;
		    }	
	    }


        // Methods only used in the actual game (not main screen)
        void UpdateUIRectangle(ref structRectangle rectangle)
        {

            double width = canvas.ActualWidth == 0 ? canvas.Width : canvas.ActualWidth;
            double height = canvas.ActualHeight == 0 ? canvas.Height : canvas.ActualHeight;

            //int x = randomGenerator.Next(0, 2) == 0 ? randomGenerator.Next((int)(width / 10), (int)(2 * width / 10)) : randomGenerator.Next((int)(7 * width / 10), (int)(9 * width / 10));
            //int y = randomGenerator.Next((int)(height / 10), (int)(8 * height / 10 ));

            Rectangle rect = rectangle.rect;

            double rectWidth = rect.ActualWidth == 0 ? rect.Width : rect.ActualWidth;
            double rectHeight = rect.ActualHeight == 0 ? rect.Height : rect.ActualHeight;

            double maxX = width / rectWidth;
            double maxY = height / rectHeight;

            int x = randomGenerator.Next(0, 2) == 0 ? (int)rectWidth * randomGenerator.Next((int)(maxX / 10), (int)(2 * maxX / 10)) : (int)rectWidth * randomGenerator.Next((int)(8 * maxX / 10), (int)(maxX));
            int y = (int)rectHeight * randomGenerator.Next((int)(2 * maxY / 10), (int)(9 * maxY / 10));

            positionList.Remove(rectangle.position);

            string cadena = x.ToString() + ":" + y.ToString();

            while (positionList.Contains(cadena))
            {
                x = randomGenerator.Next(0, 2) == 0 ? (int)rectWidth * randomGenerator.Next((int)(maxX / 10), (int)(2 * maxX / 10)) : (int)rectWidth * randomGenerator.Next((int)(8 * maxX / 10), (int)(maxX));
                y = (int)rectHeight * randomGenerator.Next((int)(2 * maxY / 10), (int)(9 * maxY / 10));

                cadena = x.ToString() + ":" + y.ToString();
            }

            positionList.Add(cadena);

            rectangle.position = cadena;

            Canvas.SetLeft(rect, x);
            Canvas.SetTop(rect, y);

        }

        void UpdateRectangleThings(ref structRectangle rectangle)
        {
            UpdateUIRectangle(ref rectangle);
            canvas.Children.Remove(rectangle.rect);
            rectangle.beginTime = countdown.Elapsed;
            rectangle.gestureTried = false;
        }

        void InteractionRectangle(ref structRectangle rectangle, Joint[] joints)
        {
            if ((countdown.Elapsed - rectangle.beginTime).Seconds > 1)
            {

                if ((countdown.Elapsed - rectangle.beginTime).Seconds > randomGenerator.Next(lowRandom, 20))
                {
                    if (rectangle.gestureTried)
                        score += 10;
                    else
                        score -= score - 20 < 0 ? score : 20;

                    UpdateRectangleThings(ref rectangle);
                    return;
                }


                canvas.Children.Add(rectangle.rect);

                //Countdown.Text = "Nope";

                foreach (Joint joint in joints)
                {
                    if (joint.Intersection(rectangle.rect))
                    {
                        switch (joint.JointType)
                        {
                            case JointType.HandLeft:
                                if (rectangle.handState != leftHandState)
                                {
                                    rectangle.gestureTried = true;
                                    continue;
                                }
                                break;
                            case JointType.HandRight:
                                if (rectangle.handState != rightHandState)
                                {
                                    rectangle.gestureTried = true;
                                    continue;
                                }
                                break;
                        }

                        // Countdown.Text = "Intersection!!!!!";

                        score += 20;

                        UpdateRectangleThings(ref rectangle);

                        break;
                    }
                }
            }
        }


        // Methods only used in the Main Screen
        void UpdateRectangleThingsMain(ref structRectangle rectangle)
        {
            UpdateUIRectangleMain(ref rectangle);
            canvas.Children.Remove(rectangle.rect);
            rectangle.beginTime = countdown.Elapsed;
        }

        void UpdateUIRectangleMain(ref structRectangle rectangle)
        {

            Rectangle rect = rectangle.rect;

            double rectWidth = rect.ActualWidth == 0 ? rect.Width : rect.ActualWidth;
            double rectHeight = rect.ActualHeight == 0 ? rect.Height : rect.ActualHeight;

            positionList.Add(rectangle.position);

            string cadena = positionList[0];
            positionList.RemoveAt(0);

            int x = (int)(rectWidth * char.GetNumericValue(cadena[0]));
            int y = (int)(rectHeight * char.GetNumericValue(cadena[2]));

            rectangle.position = cadena;

            Canvas.SetLeft(rect, x);
            Canvas.SetTop(rect, y);
        }

        void InteractionRectangle(ref structRectangle rectangle, Joint joint)
        {
            if ((countdown.Elapsed - rectangle.beginTime).Seconds > 1)
            {
                canvas.Children.Add(rectangle.rect);
               
                if (joint.Intersection(rectangle.rect))
                {
                    switch (joint.JointType)
                    {
                        case JointType.HandLeft:
                            if (rectangle.handState == leftHandState)
                                UpdateRectangleThingsMain(ref rectangle);
                            break;
                        case JointType.HandRight:
                            if (rectangle.handState == rightHandState)
                                UpdateRectangleThingsMain(ref rectangle);
                            break;
                    }
                }
            }
        }


        // The method where everything happens
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

                                Joint head = body.Joints[JointType.Head];
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

                                jointArray[0] = head;
                                jointArray[1] = handLeft;
                                jointArray[2] = handRight;

				                switch (actualState){
                                    case GameState.Initialize:
                                        for (int i = 0; i < structRectangleArray.Length; i++)
                                            UpdateUIRectangleMain(ref structRectangleArray[i]);
                                        actualState = GameState.MainScreen;

                                        countdown.Start();
                                    break;
                                    case GameState.MainScreen:
                                        foreach (Joint joint in jointArray)
                                            if (joint.Intersection(Reset))
                                            {
                                                actualState = GameState.InitialPosition;

                                                ResetText.Visibility = System.Windows.Visibility.Hidden;
                                                pink.Visibility = System.Windows.Visibility.Visible;

                                                break;
                                            }
                                            else
                                                for (int i = 0; i < structRectangleArray.Length; i++)
                                                    InteractionRectangle(ref structRectangleArray[i], joint);
                                    break;
                                    case GameState.InitialPosition:
						                CheckInitialConditions(head);
					                break;
					                case GameState.ShowStart:
						                if(countdown.Elapsed.Seconds == 2){
							                countdown.Reset();

							                actualState = GameState.Running;

                                            for (int i = 0; i < structRectangleArray.Length; i++ )
                                                UpdateUIRectangle(ref structRectangleArray[i]);

                                            ScoreBox.Visibility = System.Windows.Visibility.Visible;
                                            TimerBox.Visibility = System.Windows.Visibility.Visible;

                                            countdown.Start();
						                }
					                break;
					                case GameState.Running:
						                if (InGameConditions(head)){

                                            TimerBox.Text = (60 - countdown.Elapsed.Seconds).ToString();

                                            Point handPoint = handRight.Scale(_sensor.CoordinateMapper);

                                            rightHandState = body.HandRightState;
                                            leftHandState = body.HandLeftState;

                                            for (int i = 0; i < structRectangleArray.Length; i++)
                                                InteractionRectangle(ref structRectangleArray[i],jointArray);


                                            ScoreBox.Text = score.ToString();

                                            if (countdown.Elapsed.Seconds > 29)
                                            {
                                                lowRandom = 4;

                                                if (countdown.Elapsed.Seconds > 58)
                                                {
                                                    actualState = GameState.Ended;

                                                    countdown.Reset();


                                                    ScoreBox.Visibility = System.Windows.Visibility.Hidden;
                                                    TimerBox.Visibility = System.Windows.Visibility.Hidden;

                                                    Countdown.Text = "Your Swaggy score is " + score.ToString();

                                                    GameOver.Visibility = System.Windows.Visibility.Visible;

                                                    ResetText.Text = "Try again!";
                                                    ResetText.Visibility = System.Windows.Visibility.Visible;

                                                    for (int i = 0; i < structRectangleArray.Length; i++)
                                                        UpdateRectangleThings(ref structRectangleArray[i]);
                                                }
                                            }
                                        }
					                break;	
                                    case GameState.Ended:
                                        foreach(Joint joint in jointArray)
                                            if (joint.Intersection(Reset)) {
                                                actualState = GameState.ShowStart;

                                                Countdown.Text = "STAAAAAART!!!";
                                                ResetText.Visibility = System.Windows.Visibility.Hidden;
                                                GameOver.Visibility = System.Windows.Visibility.Hidden;

                                                TimerBox.Text = "60";

                                                countdown.Start();
                                                break;
                                            }
                                    break;
					                default:
                                    break;		
				                }


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
                                canvas.DrawPoint(thumbRight, _sensor.CoordinateMapper);
                                canvas.DrawPoint(thumbLeft, _sensor.CoordinateMapper);
                                canvas.DrawLine(WristLeftJoint, handLeft, _sensor.CoordinateMapper);
                                canvas.DrawLine(WristRightJoint, handRight, _sensor.CoordinateMapper); 

                                canvas.DrawLine(head,SpineShoulderJoint, _sensor.CoordinateMapper);
                                canvas.DrawLine(SpineShoulderJoint, ShoulderLeft, _sensor.CoordinateMapper);
                                canvas.DrawLine(SpineShoulderJoint, ShoulderRight, _sensor.CoordinateMapper);
                                canvas.DrawLine(ShoulderLeft, ElbowLeftJoint, _sensor.CoordinateMapper);
                                canvas.DrawLine(ShoulderRight, ElbowRightJoint, _sensor.CoordinateMapper);
                                canvas.DrawLine(ElbowRightJoint, WristRightJoint, _sensor.CoordinateMapper);
                                canvas.DrawLine(ElbowLeftJoint, WristLeftJoint, _sensor.CoordinateMapper);
                                                              

                                //canvas.DrawSkeleton(body, _sensor.CoordinateMapper);
                                // Find the hand states
                                
                                string RightHandState = "-";
                                string LeftHandState = "-";

                                switch (body.HandRightState)
                                {
                                    case HandState.Open:
                                        RightHandState = "Open";
                                        break;
                                    case HandState.Closed:
                                        RightHandState = "Closed";
                                        break;
                                    case HandState.Lasso:
                                        RightHandState = "Lasso";
                                        break;
                                    case HandState.Unknown:
                                        RightHandState = "Unknown...";
                                        break;
                                    case HandState.NotTracked:
                                        RightHandState = "Not tracked";
                                        break;
                                    default:
                                        break;
                                }

                                switch (body.HandLeftState)
                                {
                                    case HandState.Open:
                                        LeftHandState = "Open";
                                        break;
                                    case HandState.Closed:
                                        LeftHandState = "Closed";
                                        break;
                                    case HandState.Lasso:
                                        LeftHandState = "Lasso";
                                        break;
                                    case HandState.Unknown:
                                        LeftHandState = "Unknown...";
                                        break;
                                    case HandState.NotTracked:
                                        LeftHandState = "Not tracked";
                                        break;
                                    default:
                                        break;
                                }

                                tblRightHandState.Text = RightHandState;
                                tblLeftHandState.Text = LeftHandState;
                                




                                //To prevent Kinect for detecting other bodies
                                break;
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


        private void InteractionRect()
        {

        }

        private void TextBox_TextChanged_4(object sender, TextChangedEventArgs e)
        {

        }
    }
}
