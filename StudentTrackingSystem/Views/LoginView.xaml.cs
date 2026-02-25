using StudentTrackingSystem.Services;
using StudentTrackingSystem.Models;
using System;
using System.Threading.Tasks;
using Microsoft.Maui.Storage;

namespace StudentTrackingSystem.Views
{
    public partial class LoginView : ContentPage
    {
        private readonly LoginService _loginService;

        public LoginView()
        {
            try
            {
                InitializeComponent();
                _loginService = new LoginService();
                LoadSavedCredentials();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"LoginView Init Hatası: {ex.Message}");
            }
        }

        private void OnPasswordToggleClicked(object sender, EventArgs e)
        {
            try
            {
                // iOS, IsPassword true'ya dönerken UITextField içeriğini güvenlik
                // nedeniyle temizler. Metni kaydet ve geri yükle.
                var savedText = TxtPassword.Text;
                TxtPassword.IsPassword = !TxtPassword.IsPassword;
                if (TxtPassword.Text != savedText)
                    TxtPassword.Text = savedText;

                BtnPasswordToggle.Source = TxtPassword.IsPassword ? "eye_off.png" : "eye_on.png";
            }
            catch { /**/ }
        }

        private async void BtnLogin_Clicked(object sender, EventArgs e)
        {
            try
            {
                string username = TxtUsername.Text?.Trim();
                string password = TxtPassword.Text?.Trim();

                if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                {
                    await DisplayAlert("Uyarı", "Lütfen kullanıcı adı ve şifre giriniz.", "Tamam");
                    return;
                }

                BtnLogin.IsEnabled = false;
                BtnLogin.Text = "Giriş Yapılıyor...";

                bool isSuccess = await _loginService.LoginAsUserAsync(username, password);

                if (isSuccess)
                {
                    ManageRememberMe(username, password);
                    await Shell.Current.GoToAsync("///ClassListView");
                }
                else
                {
                    await DisplayAlert("Hata", "Kullanıcı adı veya şifre hatalı. Lütfen tekrar deneyin.", "Tamam");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Bağlantı Hatası",
                    "Sunucuya erişilemedi. Lütfen internet bağlantınızı kontrol edin veya daha sonra tekrar deneyin.", "Tamam");
                System.Diagnostics.Debug.WriteLine($"Login Hatası: {ex.Message}");
            }
            finally
            {
                BtnLogin.IsEnabled = true;
                BtnLogin.Text = "Giriş Yap";
            }
        }

        private void ManageRememberMe(string user, string pass)
        {
            try
            {
                if (ChkRememberMe.IsChecked)
                {
                    Preferences.Default.Set("SavedUsername", user);
                    Preferences.Default.Set("SavedPassword", pass);
                    Preferences.Default.Set("IsRemembered", true);
                }
                else
                {
                    Preferences.Default.Remove("SavedUsername");
                    Preferences.Default.Remove("SavedPassword");
                    Preferences.Default.Set("IsRemembered", false);
                }
            }
            catch { /**/ }
        }

        private void LoadSavedCredentials()
        {
            try
            {
                if (Preferences.Default.Get("IsRemembered", false))
                {
                    TxtUsername.Text = Preferences.Default.Get("SavedUsername", "");
                    TxtPassword.Text = Preferences.Default.Get("SavedPassword", "");
                    ChkRememberMe.IsChecked = true;
                }
            }
            catch { /**/ }
        }

        private void OnRememberMeLabelTapped(object sender, EventArgs e)
        {
            try { ChkRememberMe.IsChecked = !ChkRememberMe.IsChecked; }
            catch { /**/ }
        }
    }
}
