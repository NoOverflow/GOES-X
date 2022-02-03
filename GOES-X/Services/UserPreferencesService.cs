namespace GOES_X.Services
{
    public class UserPreferencesService
    {
        private int _bandSelected = 1;
        public int BandSelected
        {
            get
            {
                return _bandSelected;
            }

            set
            {
                _bandSelected = value;
                OnUserPreferencesUpdate?.Invoke();
            }
        }

        public delegate void OnUserPreferencesUpdateDelegate();
        public event OnUserPreferencesUpdateDelegate? OnUserPreferencesUpdate;
    }
}
