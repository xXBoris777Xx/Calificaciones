using System.Globalization;

namespace Calificaciones;

public partial class MainPage : ContentPage
{
    private const double MinGrade = 0.1;
    private const double MaxGrade = 10.0;
    private readonly string[] students = { "Mateo", "Camila", "Luis", "Valentina", "Diego" };
    private readonly string connectedUser;

    public MainPage(string? userName = null)
    {
        InitializeComponent();
        connectedUser = string.IsNullOrWhiteSpace(userName) ? "Estudiante" : userName;
        welcomeLabel.Text = $"Bienvenido, {connectedUser}";
        LoadStudents();
    }

    private void LoadStudents()
    {
        studentPicker.ItemsSource = students;
        studentPicker.SelectedIndex = 0;
    }

    private async void OnPageLoaded(object sender, EventArgs e)
    {
        studentCard.Opacity = 0;
        partialOneCard.Opacity = 0;
        partialTwoCard.Opacity = 0;
        summaryCard.Opacity = 0;
        studentCard.TranslationY = 18;
        partialOneCard.TranslationY = 18;
        partialTwoCard.TranslationY = 18;
        summaryCard.TranslationY = 18;

        await Task.WhenAll(
            studentCard.FadeTo(1, 350, Easing.CubicOut),
            partialOneCard.FadeTo(1, 400, Easing.CubicOut),
            partialTwoCard.FadeTo(1, 450, Easing.CubicOut),
            summaryCard.FadeTo(1, 500, Easing.CubicOut));

        await Task.WhenAll(
            studentCard.TranslateTo(0, 0, 350, Easing.CubicOut),
            partialOneCard.TranslateTo(0, 0, 400, Easing.CubicOut),
            partialTwoCard.TranslateTo(0, 0, 450, Easing.CubicOut),
            summaryCard.TranslateTo(0, 0, 500, Easing.CubicOut));
    }

    private async void OnCalculateClicked(object sender, EventArgs e)
    {
        if (studentPicker.SelectedItem is null)
        {
            await DisplayAlert("Validación", "Seleccione un estudiante antes de continuar.", "Aceptar");
            return;
        }

        if (!TryReadGrade(seguimiento1Entry.Text, "Nota Seguimiento 1", out var seguimiento1))
            return;

        if (!TryReadGrade(examen1Entry.Text, "Examen 1", out var examen1))
            return;

        if (!TryReadGrade(seguimiento2Entry.Text, "Nota Seguimiento 2", out var seguimiento2))
            return;

        if (!TryReadGrade(examen2Entry.Text, "Examen 2", out var examen2))
            return;

        await AnimateButtonPress(calculateButton);

        var parcial1 = (seguimiento1 * 0.3) + (examen1 * 0.2);
        var parcial2 = (seguimiento2 * 0.3) + (examen2 * 0.2);
        var notaFinal = parcial1 + parcial2;
        var estado = GetEstado(notaFinal);

        partialOneResultLabel.Text = parcial1.ToString("0.00", CultureInfo.InvariantCulture);
        partialTwoResultLabel.Text = parcial2.ToString("0.00", CultureInfo.InvariantCulture);
        finalResultLabel.Text = $"Nota Final: {notaFinal.ToString("0.00", CultureInfo.InvariantCulture)}";
        statusLabel.Text = $"Estado: {estado}";
        statusBorder.BackgroundColor = estado switch
        {
            "APROBADO" => Color.FromArgb("#DDF7E8"),
            "COMPLEMENTARIO" => Color.FromArgb("#FDE9D6"),
            _ => Color.FromArgb("#FAD7D7")
        };
        statusLabel.TextColor = estado switch
        {
            "APROBADO" => Color.FromArgb("#1B6B38"),
            "COMPLEMENTARIO" => Color.FromArgb("#A95D00"),
            _ => Color.FromArgb("#922B2B")
        };

        var summary = $"Nombre: {studentPicker.SelectedItem}\nFecha: {datePicker.Date:dd/MM/yyyy}\nNota Parcial 1: {parcial1.ToString("0.00", CultureInfo.InvariantCulture)}\nNota Parcial 2: {parcial2.ToString("0.00", CultureInfo.InvariantCulture)}\nNota Final: {notaFinal.ToString("0.00", CultureInfo.InvariantCulture)}\nEstado: {estado}";

        await DisplayAlert("Resumen de calificaciones", summary, "Aceptar");
    }

    private bool TryReadGrade(string? value, string fieldName, out double grade)
    {
        grade = 0;

        if (!double.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out grade))
        {
            _ = DisplayAlert("Validación", $"El campo {fieldName} solo acepta valores numéricos.", "Aceptar");
            return false;
        }

        if (grade < MinGrade || grade > MaxGrade)
        {
            _ = DisplayAlert("Validación", $"El campo {fieldName} debe estar entre {MinGrade.ToString("0.0", CultureInfo.InvariantCulture)} y {MaxGrade.ToString("0.0", CultureInfo.InvariantCulture)}.", "Aceptar");
            return false;
        }

        return true;
    }

    private static string GetEstado(double notaFinal)
    {
        if (notaFinal >= 7)
            return "APROBADO";

        if (notaFinal >= 5 && notaFinal <= 6.9)
            return "COMPLEMENTARIO";

        return "REPROBADO";
    }

    private async Task AnimateButtonPress(Button button)
    {
        await button.ScaleTo(0.95, 80, Easing.CubicInOut);
        await button.ScaleTo(1.0, 100, Easing.CubicOut);
    }
}
