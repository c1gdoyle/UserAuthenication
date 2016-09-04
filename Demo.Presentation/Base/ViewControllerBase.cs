using System;
using Demo.Presentation.Events;

namespace Demo.Presentation.Base
{
    /// <summary>
    /// Base class for view controllers.
    /// </summary>
    /// <remarks>
    /// Based on the Cinch ViewModelBase.cs by Sacha Barber.
    /// See :
    /// https://cinch.codeplex.com/
    /// http://www.codeproject.com/Articles/87541/CinchV-Version-of-my-Cinch-MVVM-framework-part
    /// </remarks>
    public abstract class ViewControllerBase : NotifierBase
    {
        public event EventHandler<WindowShouldCloseEventArgs> WindowShouldClose;

        /// <summary>
        /// Closes the window controller by this view controller, with the supplied
        /// value of the Window's DialogResult property, if appropriate.
        /// </summary>
        /// <param name="dialogResult">The value that the Window's Dialogresult property
        /// should be set to, if the Window was shown as a dialog.</param>
        protected void NotifiyWindowShouldClose(bool? dialogResult)
        {
            EventHandler<WindowShouldCloseEventArgs> handler = WindowShouldClose;
            if (handler != null)
            {
                handler(this, new WindowShouldCloseEventArgs(dialogResult));
            }
        }

        /// <summary>
        /// Method that is called when the <see cref="System.Windows.Window"/> associated
        /// with this view controller is closed.
        /// </summary>
        protected internal virtual void OnWindowClosed()
        {
        }

        /// <summary>
        /// Method that is called when the <see cref="System.Windows.Window"/> associated
        /// with this view controller gets the logical focus.
        /// </summary>
        protected internal virtual void OnWindowActivated()
        {
        }

        /// <summary>
        /// Method that is called when the <see cref="System.Windows.Window"/> associated
        /// with this view controller loses the logical focus.
        /// </summary>
        protected internal virtual void OnWindowDeactivated()
        {
        }
    }
}
