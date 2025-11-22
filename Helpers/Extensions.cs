using System.Windows;

namespace SnakeGame.Helpers
{
    public static class Helpers
    {
        public static string GetBindingProperty(this FrameworkElement el, DependencyProperty dp)
        {
            var binding = el.GetBindingExpression(dp);
            
            string propName = binding?.ParentBinding.Path.Path 
                ?? throw new Exception("No binding path found, make sure you provided one in xaml.");

            return propName.Split('.').Last();
        } 
    }
}