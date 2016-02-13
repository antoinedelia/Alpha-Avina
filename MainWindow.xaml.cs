using System;
using System.Windows;
using System.Windows.Controls;
using Holo.Kinect;
using System.Threading;
using System.Speech.Synthesis;
using Microsoft.Kinect;
using System.ComponentModel;
using Microsoft.Kinect.Toolkit.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.IO;
using Microsoft.Kinect.Toolkit;
using System.Linq;

namespace Holo
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, SpeakObserver
    {


        private static Son micro = null;
        private static Squelette squelette = null;
        private SpeechSynthesizer synth = new SpeechSynthesizer();
        private Connexion connexion_kinect;
        private KinectSensorChooser sensorChooser;
        private bool isRotate = false;
        private static double tailleMax = 2;
        private static double tailleMin = 0.5;
        private double taille = 1;



        public MainWindow()
        {


            if ()
            {
                if ( < 80 && > 800)
                {
                    InitializeComponent();
                    MediaPlayer1.LoadedBehavior = MediaState.Manual;
                    MediaPlayer1.UnloadedBehavior = MediaState.Manual;

                    MediaPlayer2.LoadedBehavior = MediaState.Manual;
                    MediaPlayer2.UnloadedBehavior = MediaState.Manual;

                    MediaPlayer3.LoadedBehavior = MediaState.Manual;
                    MediaPlayer3.UnloadedBehavior = MediaState.Manual;

                    MediaPlayer4.LoadedBehavior = MediaState.Manual;
                    MediaPlayer4.UnloadedBehavior = MediaState.Manual;
                    afficher("C:\\noAudio.mp4");

                    this.sensorChooser = new KinectSensorChooser();
                    this.sensorChooser.KinectChanged += SensorChooserOnKinectChanged;
                    this.sensorChooser.Start();

                    connexion_kinect = Connexion.getInstance();
                    connexion_kinect.Initconnexion();


                    micro = Son.getInstance();
                    squelette = Squelette.getInstance();

                    //Thread microThread = new Thread(micro.WindowLoaded);
                    //Thread squeletteThread = new Thread(squelette.Load);

                    // Start the worker thread.

                    //TODO : Mettre en commun sensor
                    //microThread.Start(sensor);
                    //squeletteThread.Start(sensor);

                    micro.addObserver(this);
                    squelette.addObserver(this);

                    micro.Load();
                    squelette.Load();
                }
            }
            else
            {
                Thread newWindowThread = new Thread(new ThreadStart(ThreadStartingPoint));
                newWindowThread.SetApartmentState(ApartmentState.STA);
                newWindowThread.IsBackground = true;
                newWindowThread.Start();
            }

        }




        private void ThreadStartingPoint()
        {
            WpfApplication1.MainWindow mainWindow = new WpfApplication1.MainWindow();
            mainWindow.Show();
            System.Windows.Threading.Dispatcher.Run();
        }





        public void onSpeak(string audiotext)
        {
            if (audiotext.Substring(0, 1).Equals("-"))
            {
                if (audiotext.Substring(1, audiotext.Length - 1).Equals("video"))
                {
                    fillScrollContent();
                    synth.SpeakAsync("Voici la liste des vidéos disponibles");
                    GridMain.Visibility = Visibility.Hidden;
                    GridKinect.Visibility = Visibility.Visible;
                    GridVideo.Visibility = Visibility.Hidden;

                    //TODO CHANGE ROTATION

                    ScaleTransform scaleTransform = new ScaleTransform(1, 1);
                    MediaPlayer1.RenderTransform = scaleTransform;
                    MediaPlayer2.RenderTransform = scaleTransform;
                    MediaPlayer3.RenderTransform = scaleTransform;
                    MediaPlayer4.RenderTransform = scaleTransform;
                    taille = 1;
                }
                else if (audiotext.Substring(1, audiotext.Length - 1).Equals("retour"))
                {
                    if (GridVideo.Visibility == Visibility.Visible)
                    {
                        GridMain.Visibility = Visibility.Hidden;
                        GridKinect.Visibility = Visibility.Visible;
                        GridVideo.Visibility = Visibility.Hidden;
                        ScaleTransform scaleTransform = new ScaleTransform(1, 1);
                        MediaPlayer5.RenderTransform = scaleTransform;
                        taille = 1;
                    }
                    else if (GridKinect.Visibility == Visibility.Visible)
                    {
                        GridMain.Visibility = Visibility.Visible;
                        GridKinect.Visibility = Visibility.Hidden;
                        GridVideo.Visibility = Visibility.Hidden;
                    }
                }
                else if (audiotext.Substring(1, audiotext.Length - 1).Equals("rotate"))
                {
                    rotation();
                }

                //TODO CHANGE ROTATION
                else if (audiotext.Substring(1, audiotext.Length - 1).Equals("zoom+"))
                {
                    if(taille < tailleMax)
                    {
                        taille += 0.001;
                        ScaleTransform scaleTransform;
                        if (GridVideo.Visibility == Visibility.Visible)
                        {
                            scaleTransform = new ScaleTransform(taille, taille, 960, 540);
                            MediaPlayer5.RenderTransform = scaleTransform;
                        }
                    }
                }

                //TODO CHANGE ROTATION
                else if (audiotext.Substring(1, audiotext.Length - 1).Equals("zoom-"))
                {
                    if(taille > tailleMin)
                    {
                        taille -= 0.001;
                        ScaleTransform scaleTransform;
                        if (GridVideo.Visibility == Visibility.Visible)
                        {
                            scaleTransform = new ScaleTransform(taille, taille, 960, 540);
                            MediaPlayer5.RenderTransform = scaleTransform;
                        }
                    }
                }
            }
            else
            {
                if (GridMain.Visibility == Visibility.Visible)
                {
                    afficher("C:\\audio.mp4");
                    Thread.Sleep(1000);

                    synth.SpeakAsync(audiotext);
                    synth.SpeakCompleted += new EventHandler<SpeakCompletedEventArgs>(synth_SpeakCompleted);
                    //afficher("D:\\noAudio.mp4");
                }
            }
        }

        private void rotation()
        {
            if (!isRotate)
            {
                RotateTransform rotateTransform1 = new RotateTransform(0);
                RotateTransform rotateTransform2 = new RotateTransform(90);
                RotateTransform rotateTransform3 = new RotateTransform(180);
                RotateTransform rotateTransform4 = new RotateTransform(270);
                MediaPlayer1.RenderTransform = rotateTransform3;
                MediaPlayer2.RenderTransform = rotateTransform4;
                MediaPlayer3.RenderTransform = rotateTransform2;
                MediaPlayer4.RenderTransform = rotateTransform1;
                isRotate = true;
            }
            else
            {
                RotateTransform rotateTransform1 = new RotateTransform(0);
                RotateTransform rotateTransform2 = new RotateTransform(90);
                RotateTransform rotateTransform3 = new RotateTransform(180);
                RotateTransform rotateTransform4 = new RotateTransform(270);
                MediaPlayer1.RenderTransform = rotateTransform1;
                MediaPlayer2.RenderTransform = rotateTransform2;
                MediaPlayer3.RenderTransform = rotateTransform4;
                MediaPlayer4.RenderTransform = rotateTransform3;
                isRotate = false;
            }
        }

        private void afficher(string video)
        {
            MediaPlayer1.Source = new Uri(video);
            MediaPlayer2.Source = new Uri(video);
            MediaPlayer3.Source = new Uri(video);
            MediaPlayer4.Source = new Uri(video);

            MediaPlayer1.MediaEnded += new RoutedEventHandler(MediaPlayer_MediaEnded);
            MediaPlayer2.MediaEnded += new RoutedEventHandler(MediaPlayer_MediaEnded);
            MediaPlayer3.MediaEnded += new RoutedEventHandler(MediaPlayer_MediaEnded);
            MediaPlayer4.MediaEnded += new RoutedEventHandler(MediaPlayer_MediaEnded);

            MediaPlayer1.Play();
            MediaPlayer2.Play();
            MediaPlayer3.Play();
            MediaPlayer4.Play();
        }



        // methode loop video
        void MediaPlayer_MediaEnded(object sender, RoutedEventArgs e)
        {
            ((MediaElement)sender).LoadedBehavior = MediaState.Stop;
            ((MediaElement)sender).Position = new TimeSpan(0);
            ((MediaElement)sender).LoadedBehavior = MediaState.Play;
        }

        private void WindowClosing(object sender, CancelEventArgs e)
        {
            if (null != connexion_kinect.Sensor)
            {
                connexion_kinect.Sensor.AudioSource.Stop();
                connexion_kinect.Sensor.Stop();
                connexion_kinect.Sensor.Dispose();
            }
        }


        void synth_SpeakCompleted(object sender, SpeakCompletedEventArgs e)
        {
            afficher("C:\\noAudio.mp4");
        }

        private void fillScrollContent()
        {
            int j = Directory.GetFiles("C:\\Videos\\").Length;
            string[] nomVideos = Directory.EnumerateFiles("C:\\Videos\\", "*.*", SearchOption.AllDirectories).Select(Path.GetFileNameWithoutExtension).ToArray();
            for (int i = 0; i < j; i++)
            {
                var button = new KinectTileButton
                {
                    Content = nomVideos[i],
                    Height = 200
                };

                var brush = new ImageBrush();
                brush.ImageSource = new BitmapImage(new Uri("C:\\white.png", UriKind.Relative));
                button.Background = brush;
                button.Height = 150;
                button.Width = 150;

                int i1 = i;
                button.Click +=
                    (o, args) => loadVideo(nomVideos[i1]);

                scrollContent.Children.Add(button);
            }

            for (int i = 0; i < j; i++)
            {
                var button = new KinectTileButton
                {
                    Content = nomVideos[i],
                    Height = 200
                };

                var brush = new ImageBrush();
                brush.ImageSource = new BitmapImage(new Uri("C:\\white.png", UriKind.Relative));
                button.Background = brush;
                button.Height = 150;
                button.Width = 150;

                int i1 = i;
                button.Click +=
                    (o, args) => loadVideo(nomVideos[i1]);

                scrollContent2.Children.Add(button);
            }

            for (int i = 0; i < j; i++)
            {
                var button = new KinectTileButton
                {
                    Content = nomVideos[i],
                    Height = 200
                };

                var brush = new ImageBrush();
                brush.ImageSource = new BitmapImage(new Uri("C:\\white.png", UriKind.Relative));
                button.Background = brush;
                button.Height = 150;
                button.Width = 150;

                int i1 = i;
                button.Click +=
                    (o, args) => loadVideo(nomVideos[i1]);

                scrollContent3.Children.Add(button);
            }

            for (int i = 0; i < j; i++)
            {
                var button = new KinectTileButton
                {
                    Content = nomVideos[i],
                    Height = 200
                };

                var brush = new ImageBrush();
                brush.ImageSource = new BitmapImage(new Uri("C:\\white.png", UriKind.Relative));
                button.Background = brush;
                button.Height = 150;
                button.Width = 150;

                int i1 = i;
                button.Click +=
                    (o, args) => loadVideo(nomVideos[i1]);

                scrollContent4.Children.Add(button);
            }
        }

        private void loadVideo(string name)
        {
            GridMain.Visibility = Visibility.Hidden;
            GridKinect.Visibility = Visibility.Hidden;
            GridVideo.Visibility = Visibility.Visible;

            MediaPlayer5.LoadedBehavior = MediaState.Manual;
            MediaPlayer5.UnloadedBehavior = MediaState.Manual;

            afficherUnique("C:\\Videos\\" + name + ".mp4");
        }

        private void afficherUnique(string video)
        {
            try
            {
                MediaPlayer5.Source = new Uri(video);
                MediaPlayer5.MediaEnded += new RoutedEventHandler(MediaPlayer_MediaEnded);
                MediaPlayer5.Play();
            }
            catch (IOException e)
            {
                Console.WriteLine(e.Message + " : La vidéo n'a pas été trouvée");
                throw;
            }
        }

        private void scrollChanged(object sender, ScrollChangedEventArgs e)
        {
            scroll2.ScrollToHorizontalOffset(e.HorizontalOffset);
            scroll3.ScrollToHorizontalOffset(e.HorizontalOffset);
            scroll4.ScrollToHorizontalOffset(e.HorizontalOffset);
        }

        private void SensorChooserOnKinectChanged(object sender, KinectChangedEventArgs args)
        {
            bool error = false;
            if (args.OldSensor != null)
            {
                try
                {
                    args.OldSensor.DepthStream.Range = DepthRange.Default;
                    args.OldSensor.SkeletonStream.EnableTrackingInNearRange = false;
                    args.OldSensor.DepthStream.Disable();
                    args.OldSensor.SkeletonStream.Disable();
                }
                catch (InvalidOperationException)
                {
                    // KinectSensor might enter an invalid state while enabling/disabling streams or stream features.
                    // E.g.: sensor might be abruptly unplugged.
                    error = true;
                }
            }

            if (args.NewSensor != null)
            {
                try
                {
                    args.NewSensor.DepthStream.Enable(DepthImageFormat.Resolution640x480Fps30);
                    args.NewSensor.SkeletonStream.Enable();

                    try
                    {
                        args.NewSensor.DepthStream.Range = DepthRange.Default;
                        args.NewSensor.SkeletonStream.EnableTrackingInNearRange = true;
                        args.NewSensor.SkeletonStream.TrackingMode = SkeletonTrackingMode.Seated;
                    }
                    catch (InvalidOperationException)
                    {
                        // Non Kinect for Windows devices do not support Near mode, so reset back to default mode.
                        args.NewSensor.DepthStream.Range = DepthRange.Default;
                        args.NewSensor.SkeletonStream.EnableTrackingInNearRange = false;
                        error = true;
                    }
                }
                catch (InvalidOperationException)
                {
                    error = true;
                    // KinectSensor might enter an invalid state while enabling/disabling streams or stream features.
                    // E.g.: sensor might be abruptly unplugged.
                }
            }
            if (!error)
                kinectRegion.KinectSensor = args.NewSensor;
        }
    }

}
