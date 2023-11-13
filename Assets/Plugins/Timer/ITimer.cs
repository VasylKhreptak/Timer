namespace Plugins.Timer
{
    public interface ITimer : IReadonlyTimer
    {
        public void Start(float duration);

        public void Stop();

        public void Pause();

        public void Resume();

        public void Reset();
    }
}