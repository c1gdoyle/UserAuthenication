using System.Reflection;
using System.Security;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Interactivity;

namespace Demo.Presentation.Behaviours
{
    /// <summary>
    /// A behaviour that listens for PasswordChanged events from a <see cref="PasswordBox"/> and
    /// exposes it as a DependencyProperty to allow binding to a property in our ViewModel. 
    /// </summary>
    /// <remarks>
    /// See Brian Noyes' post at
    /// http://briannoyesblog.azurewebsites.net/2015/03/04/wpf-using-passwordbox-in-mvvm/
    /// </remarks>
    public class PasswordBoxBindingBehaviour : Behavior<PasswordBox>
    {
        /// <summary>
        /// Identifies the EventCommand Event dependency property.
        /// </summary>
        public static readonly DependencyProperty PasswordProperty =
            DependencyProperty.Register("Password", typeof(SecureString),
                typeof(PasswordBoxBindingBehaviour), new PropertyMetadata(null));
        /// <summary>
        /// Gets or sets the Password property.
        /// </summary>
        public SecureString Password
        {
            get { return (SecureString)GetValue(PasswordProperty); }
            set { SetValue(PasswordProperty, value); }
        }

        protected override void OnAttached()
        {
            AssociatedObject.PasswordChanged += OnPasswordBoxValueChanged;
        }

        private void OnPasswordBoxValueChanged(object sender, RoutedEventArgs e)
        {
            var binding = BindingOperations.GetBindingExpression(this, PasswordProperty);
            if (binding != null)
            {
                PropertyInfo property = binding.DataItem.GetType().GetProperty(binding.ParentBinding.Path.Path);
                if (property != null)
                    property.SetValue(binding.DataItem, AssociatedObject.SecurePassword, null);
            }
        }
    }
}
