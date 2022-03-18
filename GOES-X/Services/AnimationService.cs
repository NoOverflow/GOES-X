namespace GOES_X.Services
{
    public class AnimationService
    {
        public delegate void OnAnimationTickDelegate(DateTime currentDate);
        public event OnAnimationTickDelegate? OnAnimationTick;

        public TimeSpan Interval { get; private set; }
        public bool AnimationRunning { get; private set; }

        public DateTime BeginDate { get; private set; }
        public DateTime EndDate { get; private set; }
        public DateTime CurrentDate { get; private set; }
        
        private Timer? AnimationTimer = null;

        private void AnimationThreadLogic(object? state)
        {
            CurrentDate = CurrentDate.AddMinutes(10);
            if (CurrentDate > EndDate)
                CurrentDate = BeginDate;
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
