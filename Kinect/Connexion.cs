using Microsoft.Kinect;
using System.IO;    

namespace Holo.Kinect
{
    public class Connexion
    {

        private KinectSensor sensor;
        private static Connexion instance = null;

        public KinectSensor Sensor
        {
            get {

                if(sensor == null)
                {
                    //throw new System.ArgumentException("Kinect non connecté");
                }
                return sensor;
            }

        }

        


        private Connexion() { }


        public void Initconnexion() { sensor = connexion_kinect(); }

        public static Connexion getInstance()
        {
            if (instance != null)
            {
                return instance;
            }
            else
            {
                instance = new Connexion();
                return instance;
            }
        }

   

        public KinectSensor connexion_kinect()
        {

            foreach (var potentialSensor in KinectSensor.KinectSensors)
            {
                if (potentialSensor.Status == KinectStatus.Connected)
                {
                    sensor = potentialSensor;
                }
            }

            if (null != sensor)
            {
                // Turn on the skeleton stream to receive skeleton frames
                sensor.SkeletonStream.Enable();

                // Start the sensor!
                try
                {
                    sensor.Start();
                }
                catch (IOException)
                {
                    return sensor = null;
                }  
            }

            return sensor;
        }
    }
}
