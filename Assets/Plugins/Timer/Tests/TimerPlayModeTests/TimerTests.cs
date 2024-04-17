using System.Collections;
using NUnit.Framework;
using UniRx;
using UnityEngine;
using UnityEngine.TestTools;

namespace Plugins.Timer.Tests.TimerPlayModeTests
{
    public class TimerTests
    {
        private const float DefaultDuration = 2f;

        [Test]
        public void InitialValues()
        {
            ITimer timer = new Timer();

            Assert.AreEqual(0f, timer.Progress.Value);
            Assert.AreEqual(0f, timer.RemainingProgress.Value);
            Assert.AreEqual(0f, timer.Time.Value);
            Assert.AreEqual(0f, timer.RemainingTime.Value);
            Assert.AreEqual(0f, timer.TargetTime.Value);
            Assert.AreEqual(1f, timer.TimeScale.Value);
            Assert.IsFalse(timer.IsPaused.Value);
        }

        [UnityTest]
        public IEnumerator Events()
        {
            bool started = false;
            bool completed = false;
            bool stopped = false;

            ITimer timer = new Timer();

            CompositeDisposable disposables = new CompositeDisposable();

            timer.OnStarted.Subscribe(_ => started = true).AddTo(disposables);
            timer.OnCompleted.Subscribe(_ => completed = true).AddTo(disposables);
            timer.OnStopped.Subscribe(_ => stopped = true).AddTo(disposables);

            timer.Start(DefaultDuration);

            yield return new WaitForSeconds(DefaultDuration);

            disposables.Clear();

            Assert.IsTrue(started);
            Assert.IsTrue(completed);
            Assert.IsTrue(stopped);
        }

        [UnityTest]
        public IEnumerator CompletionBeforeStop()
        {
            bool completed = false;

            ITimer timer = new Timer();

            CompositeDisposable disposables = new CompositeDisposable();

            timer.OnCompleted.Subscribe(_ => completed = true).AddTo(disposables);
            timer.OnStopped.Subscribe(_ =>
                {
                    if (completed == false)
                        Assert.Fail();
                })
                .AddTo(disposables);

            timer.Start(DefaultDuration);

            yield return new WaitForSeconds(DefaultDuration);

            disposables.Dispose();

            Assert.IsTrue(completed);
        }

        [UnityTest]
        public IEnumerator AfterCompletedValues()
        {
            ITimer timer = new Timer();

            timer.Start(DefaultDuration);

            yield return new WaitForSeconds(DefaultDuration);

            Assert.AreEqual(1f, timer.Progress.Value);
            Assert.AreEqual(0f, timer.RemainingProgress.Value);
            Assert.AreEqual(DefaultDuration, timer.Time.Value, Time.smoothDeltaTime);
            Assert.AreEqual(0f, timer.RemainingTime.Value);
            Assert.AreEqual(DefaultDuration, timer.TargetTime.Value);
            Assert.AreEqual(1f, timer.TimeScale.Value);
            Assert.IsFalse(timer.IsPaused.Value);
        }

        [UnityTest]
        public IEnumerator TimeScale()
        {
            ITimer timer = new Timer();

            timer.Start(DefaultDuration);

            timer.SetTimeScale(0.5f);

            yield return new WaitForSeconds(DefaultDuration);

            Assert.AreEqual(0.5f, timer.Progress.Value, Time.maximumDeltaTime);
            Assert.AreEqual(0.5f, timer.RemainingProgress.Value, Time.maximumDeltaTime);
            Assert.AreEqual(DefaultDuration / 2, timer.Time.Value, Time.maximumDeltaTime);
            Assert.AreEqual(DefaultDuration / 2, timer.RemainingTime.Value, Time.maximumDeltaTime);
            Assert.AreEqual(DefaultDuration, timer.TargetTime.Value);
            Assert.AreEqual(0.5f, timer.TimeScale.Value);
            Assert.IsFalse(timer.IsPaused.Value);
        }

        [UnityTest]
        public IEnumerator PauseResume()
        {
            ITimer timer = new Timer();

            timer.Start(3);

            yield return new WaitForSeconds(1f);

            timer.Pause();

            Assert.IsTrue(timer.IsPaused.Value);
            Assert.AreEqual(1f, timer.Time.Value, Time.maximumDeltaTime);
            Assert.AreEqual(2f, timer.RemainingTime.Value, Time.maximumDeltaTime);
            Assert.AreEqual(1f / 3f, timer.Progress.Value, Time.maximumDeltaTime);

            yield return new WaitForSeconds(0.5f);

            Assert.IsTrue(timer.IsPaused.Value);
            Assert.AreEqual(1f, timer.Time.Value, Time.maximumDeltaTime);
            Assert.AreEqual(2f, timer.RemainingTime.Value, Time.maximumDeltaTime);
            Assert.AreEqual(1f / 3f, timer.Progress.Value, Time.maximumDeltaTime);

            timer.Resume();

            yield return new WaitForSeconds(1f);

            timer.Pause();

            Assert.IsTrue(timer.IsPaused.Value);
            Assert.AreEqual(2f, timer.Time.Value, Time.maximumDeltaTime);
            Assert.AreEqual(1f, timer.RemainingTime.Value, Time.maximumDeltaTime);
            Assert.AreEqual(2f / 3f, timer.Progress.Value, Time.maximumDeltaTime);

            timer.Resume();

            yield return new WaitForSeconds(1f);

            Assert.AreEqual(3f, timer.Time.Value, Time.maximumDeltaTime);
            Assert.AreEqual(0f, timer.RemainingTime.Value, Time.maximumDeltaTime);
            Assert.AreEqual(1f, timer.Progress.Value, Time.maximumDeltaTime);
            Assert.IsFalse(timer.IsPaused.Value);
        }

        [UnityTest]
        public IEnumerator TogglePause()
        {
            ITimer timer = new Timer();

            timer.Start(3);

            Assert.IsFalse(timer.IsPaused.Value);

            yield return new WaitForSeconds(1f);

            timer.TogglePause();

            Assert.IsTrue(timer.IsPaused.Value);
            Assert.AreEqual(1f, timer.Time.Value, Time.maximumDeltaTime);
            Assert.AreEqual(2f, timer.RemainingTime.Value, Time.maximumDeltaTime);
            Assert.AreEqual(1f / 3f, timer.Progress.Value, Time.maximumDeltaTime);

            yield return new WaitForSeconds(0.5f);

            Assert.IsTrue(timer.IsPaused.Value);
            Assert.AreEqual(1f, timer.Time.Value, Time.maximumDeltaTime);
            Assert.AreEqual(2f, timer.RemainingTime.Value, Time.maximumDeltaTime);
            Assert.AreEqual(1f / 3f, timer.Progress.Value, Time.maximumDeltaTime);

            timer.TogglePause();

            yield return new WaitForSeconds(1f);

            timer.TogglePause();

            Assert.IsTrue(timer.IsPaused.Value);
            Assert.AreEqual(2f, timer.Time.Value, Time.maximumDeltaTime);
            Assert.AreEqual(1f, timer.RemainingTime.Value, Time.maximumDeltaTime);
            Assert.AreEqual(2f / 3f, timer.Progress.Value, Time.maximumDeltaTime);

            timer.TogglePause();

            yield return new WaitForSeconds(1f);

            Assert.AreEqual(3f, timer.Time.Value, Time.maximumDeltaTime);
            Assert.AreEqual(0f, timer.RemainingTime.Value, Time.maximumDeltaTime);
            Assert.AreEqual(1f, timer.Progress.Value, Time.maximumDeltaTime);
            Assert.IsFalse(timer.IsPaused.Value);
        }

        [UnityTest]
        public IEnumerator Stop()
        {
            ITimer timer = new Timer();

            timer.Start(3);

            yield return new WaitForSeconds(1f);

            timer.Stop();

            Assert.IsFalse(timer.IsPaused.Value);
            Assert.AreEqual(1f, timer.Time.Value, Time.maximumDeltaTime);
            Assert.AreEqual(2f, timer.RemainingTime.Value, Time.maximumDeltaTime);
            Assert.AreEqual(1f / 3f, timer.Progress.Value, Time.maximumDeltaTime);

            yield return new WaitForSeconds(0.5f);

            Assert.IsFalse(timer.IsPaused.Value);
            Assert.AreEqual(1f, timer.Time.Value, Time.maximumDeltaTime);
            Assert.AreEqual(2f, timer.RemainingTime.Value, Time.maximumDeltaTime);
            Assert.AreEqual(1f / 3f, timer.Progress.Value, Time.maximumDeltaTime);
        }

        [UnityTest]
        public IEnumerator Reset()
        {
            ITimer timer = new Timer();

            timer.Start(3);

            yield return new WaitForSeconds(1f);

            timer.Reset();

            Assert.IsFalse(timer.IsPaused.Value);
            Assert.AreEqual(0f, timer.Time.Value, Time.maximumDeltaTime);
            Assert.AreEqual(0f, timer.RemainingTime.Value, Time.maximumDeltaTime);
            Assert.AreEqual(0f, timer.Progress.Value, Time.maximumDeltaTime);
        }

        [UnityTest]
        public IEnumerator SetTime()
        {
            ITimer timer = new Timer();

            timer.Start(3);

            yield return new WaitForSeconds(1f);

            timer.SetTime(2f);

            yield return null;

            Assert.IsFalse(timer.IsPaused.Value);
            Assert.AreEqual(2f, timer.Time.Value, Time.maximumDeltaTime);
            Assert.AreEqual(1f, timer.RemainingTime.Value, Time.maximumDeltaTime);
            Assert.AreEqual(2f / 3f, timer.Progress.Value, Time.maximumDeltaTime);

            yield return new WaitForSeconds(1f);

            Assert.AreEqual(3f, timer.Time.Value, Time.maximumDeltaTime);
            Assert.AreEqual(0f, timer.RemainingTime.Value, Time.maximumDeltaTime);
            Assert.AreEqual(1f, timer.Progress.Value, Time.maximumDeltaTime);
        }

        [UnityTest]
        public IEnumerator SetTargetTime()
        {
            ITimer timer = new Timer();

            timer.Start(3);

            yield return new WaitForSeconds(1f);

            timer.SetTargetTime(2f);

            yield return null;

            Assert.IsFalse(timer.IsPaused.Value);
            Assert.AreEqual(1f, timer.Time.Value, Time.maximumDeltaTime);
            Assert.AreEqual(1f, timer.RemainingTime.Value, Time.maximumDeltaTime);
            Assert.AreEqual(1f / 2f, timer.Progress.Value, Time.maximumDeltaTime);

            yield return new WaitForSeconds(1f);

            Assert.AreEqual(2f, timer.Time.Value, Time.maximumDeltaTime);
            Assert.AreEqual(0f, timer.RemainingTime.Value, Time.maximumDeltaTime);
            Assert.AreEqual(1f, timer.Progress.Value, Time.maximumDeltaTime);
        }

        [UnityTest]
        public IEnumerator ResetTime()
        {
            ITimer timer = new Timer();

            timer.Start(3);

            yield return new WaitForSeconds(1f);

            timer.ResetTime();

            yield return null;

            Assert.IsFalse(timer.IsPaused.Value);
            Assert.AreEqual(0f, timer.Time.Value, Time.maximumDeltaTime);
            Assert.AreEqual(3f, timer.RemainingTime.Value, Time.maximumDeltaTime);
            Assert.AreEqual(0f, timer.Progress.Value, Time.maximumDeltaTime);
        }

        [UnityTest]
        public IEnumerator StartEvent()
        {
            bool started = false;

            ITimer timer = new Timer();

            CompositeDisposable disposables = new CompositeDisposable();

            timer.OnStarted.Subscribe(_ => started = true).AddTo(disposables);

            timer.Start(DefaultDuration);

            yield return new WaitForSeconds(Time.maximumDeltaTime);

            disposables.Dispose();

            Assert.IsTrue(started);
        }

        [UnityTest]
        public IEnumerator CompleteEvent()
        {
            bool completed = false;

            ITimer timer = new Timer();

            CompositeDisposable disposables = new CompositeDisposable();

            timer.OnCompleted.Subscribe(_ => completed = true).AddTo(disposables);

            timer.Start(DefaultDuration);

            yield return new WaitForSeconds(DefaultDuration + Time.maximumDeltaTime);

            disposables.Dispose();

            Assert.IsTrue(completed);
        }

        [UnityTest]
        public IEnumerator StopEvent()
        {
            bool stopped = false;

            ITimer timer = new Timer();

            CompositeDisposable disposables = new CompositeDisposable();

            timer.OnStopped.Subscribe(_ => stopped = true).AddTo(disposables);

            timer.Start(2f);

            yield return new WaitForSeconds(1f + Time.maximumDeltaTime);

            timer.Stop();

            disposables.Dispose();

            Assert.IsTrue(stopped);
        }
    }
}