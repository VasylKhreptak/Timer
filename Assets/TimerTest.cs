using System;
using Plugins.Timer;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UniRx;
using UnityEngine;

public class TimerTest : SerializedMonoBehaviour
{
    [Header("Preferences")]
    [OdinSerialize] private float _startTargetTime = 10f;
    [SerializeField] private float _targetTimeToSet = 20f;
    [SerializeField] private float _targetTimeScale = 1f;

    [Space]
    [SerializeField] private float _timeToSet = 20f;

    [Header("Debug")]
    [SerializeField] private float _progress;
    [SerializeField] private float _remainingProgress;
    [SerializeField] private string _time;
    [SerializeField] private string _remainingTime;
    [SerializeField] private string _targetTime;
    [SerializeField] private float _timeScale;

    private ITimer _timer;

    private void Awake()
    {
        _timer = new Timer();

        _timer.Progress.Subscribe(x => _progress = x).AddTo(this);
        _timer.RemainingProgress.Subscribe(x => _remainingProgress = x).AddTo(this);
        _timer.Time.Milliseconds.Subscribe(x => _time = Format(_timer.Time.TimeSpan)).AddTo(this);
        _timer.RemainingTime.Milliseconds.Subscribe(x => _remainingTime = Format(_timer.RemainingTime.TimeSpan)).AddTo(this);
        _timer.TargetTime.Milliseconds.Subscribe(x => _targetTime = Format(_timer.TargetTime.TimeSpan)).AddTo(this);
        _timer.TimeScale.Subscribe(x => _timeScale = x).AddTo(this);
        _timer.IsPaused.Subscribe(x => Debug.Log($"IsPaused: {x}")).AddTo(this);

        _timer.OnStarted.Subscribe(_ => Debug.Log("Started")).AddTo(this);
        _timer.OnCompleted.Subscribe(_ => Debug.Log("Completed")).AddTo(this);
        _timer.OnStopped.Subscribe(_ => Debug.Log("Stopped")).AddTo(this);
    }

    private string Format(TimeSpan timeSpan) =>
        $"{timeSpan.Days} : {timeSpan.Hours} : {timeSpan.Minutes} : {timeSpan.Seconds} : {timeSpan.Milliseconds}";

    [Button]
    private void StartTimer() => _timer.Start(TimeSpan.FromSeconds(_startTargetTime));

    [Button]
    private void Complete() => _timer.Complete();

    [Button]
    private void StopTimer() => _timer.Stop();

    [Button]
    private void PauseTimer() => _timer.Pause();

    [Button]
    private void ResumeTimer() => _timer.Resume();

    [Button]
    private void ResetTimer() => _timer.Reset();

    [Button]
    private void SetTimeScale() => _timer.SetTimeScale(_targetTimeScale);

    [Button]
    private void SetTime() => _timer.SetTime(TimeSpan.FromSeconds(_timeToSet));

    [Button]
    private void SetTargetTime() => _timer.SetTargetTime(TimeSpan.FromSeconds(_targetTimeToSet));

    [Button]
    private void TogglePause() => _timer.TogglePause();

    [Button]
    private void SetUpdateMethod(UpdateMethod updateMethod) => _timer.UpdateMethod = updateMethod;

    [Button]
    private void SetEngineTimeScale(float timeScale) => Time.timeScale = timeScale;
}