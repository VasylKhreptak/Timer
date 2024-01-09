using Plugins.Timer;
using Sirenix.OdinInspector;
using UniRx;
using UnityEngine;

public class TimerTest : MonoBehaviour
{
    [Header("Preferences")]
    [SerializeField] private float _startTargetTime = 10f;
    [SerializeField] private float _targetTimeToSet = 20f;
    [SerializeField] private float _timeScale = 1f;
    [SerializeField] private float _timeToSet = 20f;

    [SerializeField] private float _progress;
    [SerializeField] private float _remainingProgress;
    [SerializeField] private float _time;
    [SerializeField] private float _remainingTime;
    [SerializeField] private float _targetTime;

    private ITimer _timer;

    private void Awake()
    {
        _timer = new Timer();

        _timer.Progress.Subscribe(x => _progress = x).AddTo(this);
        _timer.RemainingProgress.Subscribe(x => _remainingProgress = x).AddTo(this);
        _timer.Time.Subscribe(x => _time = x).AddTo(this);
        _timer.RemainingTime.Subscribe(x => _remainingTime = x).AddTo(this);
        _timer.TargetTime.Subscribe(x => _targetTime = x).AddTo(this);
        _timer.TimeScale.Subscribe(x => _timeScale = x).AddTo(this);
        _timer.IsPaused.Subscribe(x => Debug.Log($"IsPaused: {x}")).AddTo(this);

        _timer.OnStarted.Subscribe(_ => Debug.Log("Started")).AddTo(this);
        _timer.OnCompleted.Subscribe(_ => Debug.Log("Completed")).AddTo(this);
        _timer.OnStopped.Subscribe(_ => Debug.Log("Stopped")).AddTo(this);
        _timer.OnPaused.Subscribe(_ => Debug.Log("Paused")).AddTo(this);
        _timer.OnResumed.Subscribe(_ => Debug.Log("Resumed")).AddTo(this);
        _timer.OnReset.Subscribe(_ => Debug.Log("Reset")).AddTo(this);
    }

    [Button]
    private void StartTimer() => _timer.Start(_startTargetTime);

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
    private void SetTimeScale() => _timer.SetTimeScale(_timeScale);

    [Button]
    private void SetTime() => _timer.SetTime(_timeToSet);

    [Button]
    private void SetTargetTime() => _timer.SetTargetTime(_targetTimeToSet);

    [Button]
    private void TogglePause() => _timer.TogglePause();
}