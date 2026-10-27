using System.Globalization;

namespace Calificaciones;

public partial class LoginPage : ContentPage
{
    private readonly string[] users = { "Carlos", "Ana", "Jose", "Boris" };
    private readonly string[] passwords = { "carlos123", "ana123", "jose123", "boris123" };

    public LoginPage()
    {
        InitializeComponent();
    }

    private async void OnLoginClicked(object sender, EventArgs e)
    {
        var username = userEntry.Text?.Trim();
        var password = passwordEntry.Text?.Trim();

        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
        {
            await DisplayAlert("Validación", "Ingrese usuario y contraseña antes de continuar.", "Aceptar");
            return;
        }

        await AnimateButtonPress(loginButton);

        for (var i = 0; i < users.Length; i++)
        {
            if (string.Equals(username, users[i], StringComparison.OrdinalIgnoreCase)
                && string.Equals(password, passwords[i], StringComparison.OrdinalIgnoreCase))
            {
                await DisplayAlert("Bienvenido", $"¡Bienvenido, {users[i]}!", "Continuar");
                await Navigation.PushAsync(new MainPage(users[i]));
                return;
            }
        }

        await DisplayAlert("Acceso denegado", "Usuario o contraseña incorrectos.", "Aceptar");
    }

    private async Task AnimateButtonPress(Button button)
    {
        await button.ScaleTo(0.95, 80, Easing.CubicInOut);
        await button.ScaleTo(1.0, 100, Easing.CubicOut);
    }

    private async void OnPageLoaded(object sender, EventArgs e)
    {
        loginCard.Opacity = 0;
        loginCard.TranslationY = 24;

        await loginCard.FadeTo(1, 450, Easing.CubicOut);
        await loginCard.TranslateTo(0, 0, 500, Easing.CubicOut);
    }
}
