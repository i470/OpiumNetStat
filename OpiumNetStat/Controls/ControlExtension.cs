using System.Windows;

namespace OpiumNetStat.Controls
{
    public  class ControlExtension
    {
        public static bool GetFadeable(DependencyObject obj)
        {
            return (bool)obj.GetValue(FadeableProperty);
        }

        public static void SetFadeable(DependencyObject obj, bool value)
        {
            obj.SetValue(FadeableProperty, value);
        }

        public static readonly DependencyProperty FadeableProperty = DependencyProperty.RegisterAttached(
            "Fadeable",
            typeof(bool),
            typeof(ControlExtension),
            new UIPropertyMetadata(false));

        public static bool GetSpin(DependencyObject obj)
        {
            return (bool)obj.GetValue(SpinProperty);
        }

        public static void SetSpin(DependencyObject obj, bool value)
        {
            obj.SetValue(SpinProperty, value);
        }

        public static readonly DependencyProperty SpinProperty = DependencyProperty.RegisterAttached(
            "Spin",
            typeof(bool),
            typeof(ControlExtension),
            new UIPropertyMetadata(false));
    }
}

