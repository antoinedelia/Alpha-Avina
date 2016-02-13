using Holo.Kinect;

namespace Holo
{
    public interface SpeakObservable
    {
        void addObserver(SpeakObserver observer);
        void Notify(string text);
    }
}
