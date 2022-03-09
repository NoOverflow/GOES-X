using GOES_X.Model;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

namespace GOES_X.Services
{
    public class UserPreferencesService
    {
        public delegate void OnUserPreferencesUpdateDelegate();
        public event OnUserPreferencesUpdateDelegate? OnUserPreferencesUpdate;
        private readonly ProtectedSessionStorage _sessionStorage;

        public UserPreferencesService(ProtectedSessionStorage sessionStorage)
        {
            _sessionStorage = sessionStorage;
        }

        public void TriggerUpdateEvent()
        {
            OnUserPreferencesUpdate?.Invoke();
        }

        public async Task<UserPreferences> AddEndUserProduct(EndUserProduct eup)
        {
            UserPreferences userPreferences = await GetPreferencesAsync();

            userPreferences.SelectedProducts.Add(eup);
            await SavePreferencesAsync(userPreferences);
            TriggerUpdateEvent();
            return userPreferences;
        }

        public async Task<UserPreferences> SetEndUserProductOpacity(EndUserProduct eup, double opacity)
        {
            UserPreferences userPreferences = await GetPreferencesAsync();
            EndUserProduct? eupPref = userPreferences.SelectedProducts.Find(x => x.Name == eup.Name);

            if (eupPref is null)
                return userPreferences;
            eupPref.Opacity = opacity;
            await SavePreferencesAsync(userPreferences);
            TriggerUpdateEvent();
            return userPreferences;
        }


        public async Task SavePreferencesAsync(UserPreferences preferences)
        {
            await _sessionStorage.SetAsync("preferences", preferences);
        }

        public async Task<UserPreferences> GetPreferencesAsync()
        {
            var result = await _sessionStorage.GetAsync<UserPreferences>("preferences");

            return result.Success ? result.Value! : new UserPreferences(); 
        }
    }
}
