using Serilog;

namespace GOES_X.Services
{
    public enum AnimationMode
    {
        Loop, 
        Rock,
        OnePass
    }

    public class AnimationService
    {
        public delegate void OnAnimationTickDelegate(DateTime currentDate);
        public event OnAnimationTickDelegate? OnAnimationTick;

        public TimeSpan Interval { get; private set; }
        public bool AnimationRunning { get; private set; }
        public AnimationMode AnimationMode { get; set; }

        public DateTime BeginDate { get; private set; }
        public DateTime EndDate { get; private set; }
        public DateTime CurrentDate { get; private set; }
        
        private Timer? AnimationTimer = null;

        /// <summary>
        /// Used during rock animation mode, true being forward
        /// </summary>
        private bool RockSide = true;

        private void AnimationThreadLogic(object? state)
        {
            CurrentDate = CurrentDate.AddMinutes(AnimationMode == AnimationMode.Rock ? (RockSide ? 10 : -10) : 10);

            if (CurrentDate > EndDate)
            {
                switch (this.AnimationMode)
                {
                    case AnimationMode.Loop:
                        CurrentDate = BeginDate;
                        break;
                    case AnimationMode.Rock:
                        CurrentDate = EndDate;
                        RockSide = !RockSide;
                        break;
                    case AnimationMode.OnePass:
                        this.AnimationRunning = false;
                        this.AnimationTimer?.Dispose();
                        break;
                }
            } 
            else if (CurrentDate < BeginDate)
            {
                if (AnimationMode == AnimationMode.Rock) 
                    RockSide = !RockSide;
                CurrentDate = BeginDate;
            }
            this.OnAnimationTick?.Invoke(this.CurrentDate);
        }

        public void Start(TimeSpan interval, DateTime beginDate, DateTime endDate)
        {
            if (this.AnimationRunning)
            {
                this.AnimationRunning = false;
                this.AnimationTimer?.Dispose();
            }
            this.Interval = interval;
            this.BeginDate = this.CurrentDate = beginDate;
            this.EndDate = endDate;
            this.AnimationRunning = true;
            this.AnimationTimer = new Timer(AnimationThreadLogic, null, TimeSpan.Zero, interval);
        }

        public void Stop()
        {
            this.AnimationRunning = false;
            this.AnimationTimer?.Dispose();
        }
    }
}
