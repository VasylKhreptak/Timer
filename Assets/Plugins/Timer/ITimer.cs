namespace Plugins.Timer
{
    public interface ITimer : IReadonlyTimer
    {
        public void Start(float duration);

        public void Complete();

        public void Stop();

        public void Pause();

        public void Resume();

        public void TogglePause();

        public void Reset();

        public void SetTimeScale(float timeScale);

        public void SetTime(float time);

        public void SetTargetTime(float targetTime);
    }
}