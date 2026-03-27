using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;

namespace Calculator.ViewModels;   // ← Измени на название твоего проекта!

public partial class MainWindowViewModel : ViewModelBase
{
    // 1. Отображаемое значение на экране калькулятора
    [ObservableProperty]
    private string _shownValue = "0";

    // Внутренние переменные для логики расчётов
    private string _previousValue = "";
    private string _currentOperation = "";
    private bool _isNewInput = true;

    // ==================== КОМАНДЫ ====================

    /// <summary>
    /// Добавляет цифру на экран
    /// </summary>
    [RelayCommand]
    private void AddNumber(string number)
    {
        if (_isNewInput)
        {
            ShownValue = number;
            _isNewInput = false;
        }
        else
        {
            ShownValue = ShownValue == "0" && number != "0" ? number : ShownValue + number;
        }
    }

    /// <summary>
    /// Добавляет десятичную точку
    /// </summary>
    [RelayCommand]
    private void AddDecimal()
    {
        if (_isNewInput)
        {
            ShownValue = "0.";
            _isNewInput = false;
        }
        else if (!ShownValue.Contains('.'))
        {
            ShownValue += ".";
        }
    }

    /// <summary>
    /// Выбирает математическую операцию (+, -, *, /)
    /// </summary>
    [RelayCommand]
    private void ExecuteOperation(string operation)
    {
        if (!string.IsNullOrEmpty(_currentOperation) && !_isNewInput)
            CalculateResult();

        _previousValue = ShownValue;
        _currentOperation = operation;
        _isNewInput = true;
    }

    /// <summary>
    /// Выполняет расчёт при нажатии "="
    /// </summary>
    [RelayCommand]
    private void Calculate()
    {
        if (string.IsNullOrEmpty(_currentOperation)) return;

        if (!double.TryParse(_previousValue, out double num1) ||
            !double.TryParse(ShownValue, out double num2))
            return;

        double result = _currentOperation switch
        {
            "+" => num1 + num2,
            "-" => num1 - num2,
            "*" => num1 * num2,
            "/" => num2 != 0 ? num1 / num2 : double.NaN,
            _   => 0
        };

        ShownValue = double.IsNaN(result) ? "Ошибка" : result.ToString("G15");
        _currentOperation = "";
        _isNewInput = true;
    }

    /// <summary>
    /// Очищает весь калькулятор
    /// </summary>
    [RelayCommand]
    private void Clear()
    {
        ShownValue = "0";
        _previousValue = "";
        _currentOperation = "";
        _isNewInput = true;
    }

    /// <summary>
    /// Меняет знак числа (+/-)
    /// </summary>
    [RelayCommand]
    private void ToggleSign()
    {
        if (ShownValue == "0" || ShownValue == "Ошибка") return;
        ShownValue = ShownValue.StartsWith("-") ? ShownValue[1..] : "-" + ShownValue;
    }

    /// <summary>
    /// Вычисляет процент
    /// </summary>
    [RelayCommand]
    private void Percent()
    {
        if (double.TryParse(ShownValue, out double value))
            ShownValue = (value / 100).ToString("G");
    }

    /// <summary>
    /// Удаляет последнюю введённую цифру (Backspace)
    /// </summary>
    [RelayCommand]
    private void RemoveLast()
    {
        if (_isNewInput || ShownValue.Length <= 1)
        {
            ShownValue = "0";
            _isNewInput = true;
            return;
        }

        ShownValue = ShownValue[..^1];
    }

    // Вспомогательный метод
    private void CalculateResult() => Calculate();
}