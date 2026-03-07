using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace AutoService.Views.Behaviors;

public static class PhoneMaskBehavior
{
    public static readonly DependencyProperty UseMaskProperty =
        DependencyProperty.RegisterAttached(
            "UseMask",
            typeof(bool),
            typeof(PhoneMaskBehavior),
            new PropertyMetadata(false, OnUseMaskChanged));

    public static bool GetUseMask(DependencyObject obj) => (bool)obj.GetValue(UseMaskProperty);
    public static void SetUseMask(DependencyObject obj, bool value) => obj.SetValue(UseMaskProperty, value);

    // Проверка что телефон полностью заполнен (9 цифр)
    public static bool IsPhoneComplete(string phoneText)
    {
        var digits = GetDigits(phoneText);
        return digits.Length == 9;
    }

    private static void OnUseMaskChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is TextBox textBox && (bool)e.NewValue)
        {
            if (string.IsNullOrEmpty(textBox.Text) || textBox.Text == "+375")
            {
                textBox.Text = "+375 ";
            }

            textBox.PreviewTextInput += OnPreviewTextInput;
            textBox.PreviewKeyDown += OnPreviewKeyDown;
            textBox.GotFocus += OnGotFocus;
        }
    }

    private static void OnGotFocus(object sender, RoutedEventArgs e)
    {
        if (sender is TextBox textBox && string.IsNullOrWhiteSpace(textBox.Text))
        {
            textBox.Text = "+375 ";
            textBox.CaretIndex = textBox.Text.Length;
        }
    }

    private static void OnPreviewTextInput(object sender, TextCompositionEventArgs e)
    {
        if (sender is not TextBox textBox) return;

        if (!char.IsDigit(e.Text[0]))
        {
            e.Handled = true;
            return;
        }

        var digits = GetDigits(textBox.Text);

        if (digits.Length >= 9)
        {
            e.Handled = true;
            return;
        }

        digits += e.Text[0];

        textBox.Text = FormatPhone(digits);
        textBox.CaretIndex = textBox.Text.Length;

        e.Handled = true;
    }

    private static void OnPreviewKeyDown(object sender, KeyEventArgs e)
    {
        if (sender is not TextBox textBox) return;

        if (e.Key == Key.Back)
        {
            var digits = GetDigits(textBox.Text);
            if (digits.Length > 0)
            {
                digits = digits[..^1];
                textBox.Text = FormatPhone(digits);
                textBox.CaretIndex = textBox.Text.Length;
            }
            e.Handled = true;
        }
        else if (e.Key == Key.Delete)
        {
            e.Handled = true;
        }
    }

    private static string GetDigits(string text)
    {
        var allDigits = new string(text.Where(char.IsDigit).ToArray());
        if (allDigits.StartsWith("375"))
        {
            return allDigits.Length > 3 ? allDigits[3..] : "";
        }
        return allDigits;
    }

    private static string FormatPhone(string digits)
    {
        if (string.IsNullOrEmpty(digits)) return "+375 ";

        var result = "+375";

        if (digits.Length >= 1)
        {
            result += $" ({digits[..Math.Min(2, digits.Length)]}";
            if (digits.Length == 1) result += "_";
            result += ")";
        }

        if (digits.Length > 2)
        {
            result += $" {digits[2..Math.Min(5, digits.Length)]}";
        }

        if (digits.Length > 5)
        {
            result += $"-{digits[5..Math.Min(7, digits.Length)]}";
        }

        if (digits.Length > 7)
        {
            result += $"-{digits[7..Math.Min(9, digits.Length)]}";
        }

        return result;
    }
}