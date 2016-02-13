using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using Microsoft.Kinect;

//TODO : ONCLOSED

namespace Holo.Kinect
{
    public class Squelette : SpeakObservable
    {
        private bool isRightHandUp = false;
        private bool isLeftHandUp = false;
        private const float RenderWidth = 640.0f;
        private const float RenderHeight = 480.0f;
        private const double JointThickness = 3;
        private const double BodyCenterThickness = 10;
        private const double ClipBoundsThickness = 10;
        private readonly Brush centerPointBrush = Brushes.Blue;
        private readonly Brush trackedJointBrush = new SolidColorBrush(Color.FromArgb(255, 68, 192, 68));
        private readonly Brush inferredJointBrush = Brushes.Yellow;
        private readonly Pen trackedBonePen = new Pen(Brushes.Green, 6);
        private readonly Pen inferredBonePen = new Pen(Brushes.Gray, 1);
        private KinectSensor sensor;
        private DrawingGroup drawingGroup;
        private DrawingImage imageSource;
        private static Squelette instance = null;
        private List<SpeakObserver> listeObserver = new List<SpeakObserver>();
        private Connexion connexion_kinect;

        
        public static Squelette getInstance()
        {
            if (instance != null)
                return instance;
            else
                return new Squelette();
        }


        private Squelette(){}

        public void Load()
        {
            connexion_kinect = Connexion.getInstance();
            sensor = connexion_kinect.Sensor;

            if (sensor == null)
            {
                MessageBox.Show("Kinect non connecté");
                return;
            }

            this.drawingGroup = new DrawingGroup();

            // Create an image source that we can use in our image control
            this.imageSource = new DrawingImage(this.drawingGroup);
            this.sensor.SkeletonFrameReady += this.SensorSkeletonFrameReady;
        }

        private void SensorSkeletonFrameReady(object sender, SkeletonFrameReadyEventArgs e)
        {
            Skeleton[] skeletons = new Skeleton[0];

            using (SkeletonFrame skeletonFrame = e.OpenSkeletonFrame())
            {
                if (skeletonFrame != null)
                {
                    skeletons = new Skeleton[skeletonFrame.SkeletonArrayLength];
                    skeletonFrame.CopySkeletonDataTo(skeletons);
                }
            }

                if (skeletons.Length != 0)
                {
                    foreach (Skeleton skel in skeletons)
                    {
                        if (skel.TrackingState == SkeletonTrackingState.Tracked)
                            this.DrawBonesAndJoints(skel);
                        else if (skel.TrackingState == SkeletonTrackingState.PositionOnly){}
                    }
                }
        }

        private void DrawBonesAndJoints(Skeleton skeleton)
        {
            // Left Arm
            this.DrawBone(skeleton, JointType.ShoulderLeft, JointType.ElbowLeft);
            this.DrawBone(skeleton, JointType.ElbowLeft, JointType.WristLeft);
            this.DrawBone(skeleton, JointType.WristLeft, JointType.HandLeft);

            // Right Arm
            this.DrawBone(skeleton, JointType.ShoulderRight, JointType.ElbowRight);
            this.DrawBone(skeleton, JointType.ElbowRight, JointType.WristRight);
            this.DrawBone(skeleton, JointType.WristRight, JointType.HandRight);
            
            // Render Joints
            foreach (Joint joint in skeleton.Joints)
            {
                Brush drawBrush = null;

                if (joint.TrackingState == JointTrackingState.Tracked)
                    drawBrush = this.trackedJointBrush;
                else if (joint.TrackingState == JointTrackingState.Inferred)
                    drawBrush = this.inferredJointBrush;

                if (drawBrush != null){ }
            }
        }

        private Point SkeletonPointToScreen(SkeletonPoint skelpoint)
        {
            DepthImagePoint depthPoint = this.sensor.CoordinateMapper.MapSkeletonPointToDepthPoint(skelpoint, DepthImageFormat.Resolution640x480Fps30);
            return new Point(depthPoint.X, depthPoint.Y);
        }

        private void DrawBone(Skeleton skeleton, JointType jointType0, JointType jointType1)
        {
            Joint joint0 = skeleton.Joints[jointType0];
            Joint joint1 = skeleton.Joints[jointType1];

            //Bonjour
            if (skeleton.Joints[JointType.HandRight].Position.Y >
                skeleton.Joints[JointType.ElbowRight].Position.Y)
            {
                // Hand right of elbow
                if (skeleton.Joints[JointType.HandRight].Position.X >
                    skeleton.Joints[JointType.ElbowRight].Position.X)
                {
                    if (!isRightHandUp)
                    {
                        Notify("Bonjour Comment ça va ?");
                        isRightHandUp = true;
                    }
                }
            }
            else
                isRightHandUp = false;

            //Tant mieux
            if (skeleton.Joints[JointType.HandLeft].Position.Y >
                skeleton.Joints[JointType.ElbowLeft].Position.Y)
            {
                // Hand right of elbow
                if (skeleton.Joints[JointType.HandLeft].Position.X >
                    skeleton.Joints[JointType.ElbowLeft].Position.X)
                {
                    if (!isLeftHandUp)
                    {
                        Notify("Tant mieux !");
                        isLeftHandUp = true;
                    }
                }
            }
            else
                isLeftHandUp = false;

            //Agrandir
            if (skeleton.Joints[JointType.HandRight].Position.X -
                skeleton.Joints[JointType.HandLeft].Position.X < 0.2)
                Notify("-zoom-");

            //Rétrécir
            if (skeleton.Joints[JointType.HandRight].Position.X -
                skeleton.Joints[JointType.HandLeft].Position.X > 0.7)
                Notify("-zoom+");




            // If we can't find either of these joints, exit
            if (joint0.TrackingState == JointTrackingState.NotTracked ||
                joint1.TrackingState == JointTrackingState.NotTracked)
                return;

            // Don't draw if both points are inferred
            if (joint0.TrackingState == JointTrackingState.Inferred &&
                joint1.TrackingState == JointTrackingState.Inferred)
                return;

            // We assume all drawn bones are inferred unless BOTH joints are tracked
            Pen drawPen = this.inferredBonePen;
            if (joint0.TrackingState == JointTrackingState.Tracked && joint1.TrackingState == JointTrackingState.Tracked)
                drawPen = this.trackedBonePen;
        }

        public void addObserver(SpeakObserver observer)
        {
            listeObserver.Add(observer);
        }

        public void Notify(string text)
        {
            foreach (var o in listeObserver)
                o.onSpeak(text);
        }
    }
}
