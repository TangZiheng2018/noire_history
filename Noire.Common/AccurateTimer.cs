using System.Diagnostics;

namespace Noire.Common {

    public class AccurateTimer {

        static AccurateTimer() {
            var countsPerSec = Stopwatch.Frequency;
            SecondsPerCount = 1.0 / countsPerSec;
        }

        public AccurateTimer() {
            FrameTime = 1f / 60f;
            _deltaTime = -1.0;
            _baseTime = 0;
            _pausedTime = 0;
            _prevTime = 0;
            _currTime = 0;
            _stopped = false;
        }

        public double TotalTime {
            get {
                if (_stopped) {
                    return ((_stopTime - _pausedTime) - _baseTime) * SecondsPerCount;
                } else {
                    return ((_currTime - _pausedTime) - _baseTime) * SecondsPerCount;
                }
            }
        }
        public double DeltaTime => _deltaTime;

        public void Reset() {
            var curTime = Stopwatch.GetTimestamp();
            _baseTime = curTime;
            _prevTime = curTime;
            _stopTime = 0;
            _stopped = false;
        }

        public void Start() {
            var startTime = Stopwatch.GetTimestamp();
            if (_stopped) {
                _pausedTime += (startTime - _stopTime);
                _prevTime = startTime;
                _stopTime = 0;
                _stopped = false;
            }
        }

        public void Stop() {
            if (!_stopped) {
                var curTime = Stopwatch.GetTimestamp();
                _stopTime = curTime;
                _stopped = true;
            }
        }

        public void Tick() {
            if (_stopped) {
                _deltaTime = 0.0;
                return;
            }
            var curTime = Stopwatch.GetTimestamp();
            _currTime = curTime;
            _deltaTime = (_currTime - _prevTime) * SecondsPerCount;
            _prevTime = _currTime;
            if (_deltaTime < 0.0) {
                _deltaTime = 0.0;
            }
        }

        public float FrameTime { get; set; }

        private static readonly double SecondsPerCount;

        private double _deltaTime;

        private long _baseTime;
        private long _pausedTime;
        private long _stopTime;
        private long _prevTime;
        private long _currTime;

        private bool _stopped;

    }
}