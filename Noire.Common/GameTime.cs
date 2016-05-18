using System;

namespace Noire.Common {
    public class GameTime {

        public GameTime(TimeSpan elapsed, TimeSpan total) {
            ElapsedGameTime = elapsed;
            TotalGameTime = total;
        }

        public TimeSpan ElapsedGameTime { get; }

        public TimeSpan TotalGameTime { get; }

    }
}
