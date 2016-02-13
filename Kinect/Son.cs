using System;
using System.ComponentModel;
using System.IO;
using Microsoft.Kinect;
using Microsoft.Speech.AudioFormat;
using Microsoft.Speech.Recognition;
using System.Collections.Generic;

namespace Holo.Kinect
{
    public class Son : SpeakObservable
    {

        private static Son instance = null;
        private List<SpeakObserver> listeObserver = new List<SpeakObserver>();


        private KinectSensor sensor;

        private SpeechRecognitionEngine speechEngine;

        private Connexion connexion_kinect;

        public static Son getInstance()
        {
            if (instance != null)
            {
                return instance;
            }
            else
            {
                return new Son();
            }
        }


        private Son(){ }


        
        private static RecognizerInfo GetKinectRecognizer()
        {
            foreach (RecognizerInfo recognizer in SpeechRecognitionEngine.InstalledRecognizers())
            {
                string value;
                recognizer.AdditionalInfo.TryGetValue("Kinect", out value);
                if ("True".Equals(value, StringComparison.OrdinalIgnoreCase) && "fr-FR".Equals(recognizer.Culture.Name, StringComparison.OrdinalIgnoreCase))
                {
                    return recognizer;
                }
            }

            return null;
        }

        /// <summary>
        /// Initialisation de la capture de la video.
        /// </summary>
        public void Load()
        {
            connexion_kinect = Connexion.getInstance();

            sensor = connexion_kinect.Sensor;

            if( sensor == null)
            {
                return;
            }



            RecognizerInfo ri = GetKinectRecognizer();

            if (null != ri)
            {

                this.speechEngine = new SpeechRecognitionEngine(ri.Id);

                try
                {
                    // Create a grammar from grammar definition XML file.

                    //TODO : Changer chemin
                    //var g = new Grammar("C:\\Users\\florian\\Desktop\\t.xml");
                    var g = new Grammar("C:\\t.xml");
                    speechEngine.LoadGrammar(g);
                    speechEngine.SpeechRecognized += SpeechRecognized;


                    speechEngine.SetInputToAudioStream(
                        sensor.AudioSource.Start(), new SpeechAudioFormatInfo(EncodingFormat.Pcm, 16000, 16, 1, 32000, 2, null));
                    speechEngine.RecognizeAsync(RecognizeMode.Multiple);
                }
                catch (IOException e)
                {
                    Console.WriteLine(e.Data.ToString() + " - Le fichier de grammaire n'est pas trouvable");
                }
            }

        }

        /// <summary>
        /// Execute uninitialization tasks.
        /// </summary>
        /// <param name="sender">Objet de l'envoyeur</param>
        /// <param name="e">Argument de l'evenement</param>
        public void WindowClosing(object sender, CancelEventArgs e)
        {
            if (null != this.sensor)
            {
               this.sensor.AudioSource.Stop();

                this.sensor.Stop();

                sensor.Dispose();
                this.sensor = null;
            }

            if (null != this.speechEngine)
            {
                this.speechEngine.SpeechRecognized -= SpeechRecognized;
                this.speechEngine.RecognizeAsyncStop();
            }
        }

        /// <summary>
        /// Handler for recognized speech events.
        /// </summary>
        /// <param name="sender">Objet de l'envoyeur</param>
        /// <param name="e">Argument de l'evenement</param>
        private void SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            // Speech utterance confidence below which we treat speech as if it hadn't been heard
            const double ConfidenceThreshold = 0.6;

            if (e.Result.Confidence >= ConfidenceThreshold)
            {
                switch (e.Result.Semantics.Value.ToString())
                {
                    case "BONJOUR":
                        Notify("Bonjour!");
                        break;
                    case "CA_VA":
                        Notify("Je vais bien et vous ?");
                        break;
                    case "PRESENTATION":
                        Notify("Bonjour, je suis l'intelligence artificielle Avina, je vous écoute");
                        break;
                    case "BLAGUE":
                        Notify("Toto si je te donne 50 gâteaux et tu en manges 48 tu as donc ? Mal au ventre");
                        break;
                    case "VIDEO":
                        Notify("-video");
                        break;
                    case "RETOUR":
                        Notify("-retour");
                        break;
                    case "ROTATE":
                        Notify("-rotate");
                        break;
                    case "ZOOM":
                        Notify("-zoom");
                        break;
                    default:
                        Notify("Désolé, je n'ai pas compris");
                        break;
                }
            }
        }



        public void addObserver(SpeakObserver observer)
        {
            listeObserver.Add(observer);
        }



        public void Notify(string text)
        {
            foreach (var o in listeObserver)
            {
                o.onSpeak(text);
            }
        }
    }
}
