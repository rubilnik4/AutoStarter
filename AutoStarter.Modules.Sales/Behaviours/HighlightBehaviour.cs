using System.Globalization;
using System.Windows;
using System.Windows.Controls;

namespace AutoStarter.Modules.Sales.Behaviours;

public static  class HighlightBehaviour
{
    public static readonly DependencyProperty ThresholdProperty =
        DependencyProperty.RegisterAttached("Threshold", typeof(decimal), typeof(HighlightBehaviour),
            new PropertyMetadata(0m));
    public static void SetThreshold(DependencyObject d, decimal v) => 
        d.SetValue(ThresholdProperty, v); 
    public static decimal GetThreshold(DependencyObject d) => 
        (decimal)d.GetValue(ThresholdProperty);
    
    private static readonly DependencyProperty ValueProperty =
        DependencyProperty.RegisterAttached("Value", typeof(decimal?), typeof(HighlightBehaviour),
            new PropertyMetadata(null, OnValueChanged));
    public static void SetValue(DependencyObject d, decimal? v) => 
        d.SetValue(ValueProperty, v);
    public static decimal? GetValue(DependencyObject d) => 
        (decimal?)d.GetValue(ValueProperty);
   
    private static readonly DependencyPropertyKey IsOverThresholdPropertyKey =
        DependencyProperty.RegisterAttachedReadOnly("IsOverThreshold", typeof(bool), typeof(HighlightBehaviour), 
            new PropertyMetadata(false));
    public static readonly DependencyProperty IsOverThresholdProperty =
        IsOverThresholdPropertyKey.DependencyProperty;
    public static bool GetIsOverThreshold(DependencyObject d) =>
        (bool)d.GetValue(IsOverThresholdProperty);

    private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not DataGridCell cell) return;

        var threshold = GetThreshold(cell);
        var over = false;

        try
        {
            if (e.NewValue is decimal dec)
                over = dec > threshold;
            else if (e.NewValue is IConvertible conv)
                over = Convert.ToDecimal(conv, CultureInfo.CurrentCulture) > threshold;
        }
        catch
        {
            // ignored
        }

        cell.SetValue(IsOverThresholdPropertyKey, over);
    }
}