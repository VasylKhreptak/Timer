using System;

namespace Plugins.Timer
{
    public interface ITimer : IReadonlyTimer
    {
        public void Start(TimeSpan timeSpan);

        public void Complete();

        public void Stop();

        public void Pause();

        public void Resume();

        public void TogglePause();

        public void Reset();

        public void SetTimeScale(float timeScale);

        public void SetTime(TimeSpan timeSpan);

        public void ResetTime() => SetTime(TimeSpan.Zero);

        public void SetTargetTime(TimeSpan targetTime);
    }
}