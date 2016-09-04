using System;
using System.ComponentModel;

namespace Demo.Presentation.Base
{
    /// <summary>
    /// Base class for objects that want to do PropertyChanged notifications.
    /// </summary>
    [Serializable]
    public abstract class NotifierBase : INotifyPropertyChanged, INotifyPropertyChanging
    {
        #region INotifyPropertyChanged Members
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion INotifyPropertyChanged Members

        #region INotifyPropertyChanging Members
        public event PropertyChangingEventHandler PropertyChanging;
        #endregion INotifyPropertyChanging Members

        /// <summary>
        /// Raise the PropertyChanged event for the given property.
        /// </summary>
        /// <param name="propertyName">The name of the property.</param>
        protected void NotifyPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
            OnPropertyChanged(propertyName);
        }

        /// <summary>
        /// Raise the PropertyChanging event for the given property.
        /// </summary>
        /// <param name="propertyName">The name of the property.</param>
        protected void NotifyPropertyChanging(string propertyName)
        {
            PropertyChangingEventHandler handler = PropertyChanging;
            if (handler != null)
            {
                handler(this, new PropertyChangingEventArgs(propertyName));
            }
            OnPropertyChanging(propertyName);

        }

        /// <summary>
        /// Method that is called when a PropertyChanged event is fired.
        /// </summary>
        /// <param name="propertyName">The name of the property that has changed.</param>
        protected virtual void OnPropertyChanged(string propertyName)
        {

        }

        /// <summary>
        /// Method that is called when a PropertyChanging event is fired.
        /// </summary>
        /// <param name="propertyName">The name of the property that has changing.</param>
        protected virtual void OnPropertyChanging(string propertyName)
        {

        }
    }
}
