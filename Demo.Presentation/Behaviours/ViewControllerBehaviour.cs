using System;
using System.Windows;
using System.Windows.Interop;
using Demo.Presentation.Base;
using Demo.Presentation.Events;

namespace Demo.Presentation.Behaviours
{
    public class ViewControllerBehaviour
    {
        public static readonly DependencyProperty ControllerProperty =
            DependencyProperty.RegisterAttached("Controller", typeof(object), typeof(ViewControllerBehaviour),
                new UIPropertyMetadata(null, OnControllerChanged));

        public static object GetController(DependencyObject obj)
        {
            return (object)obj.GetValue(ControllerProperty);
        }

        public static void SetController(DependencyObject obj, object value)
        {
            obj.SetValue(ControllerProperty, value);
        }

        private static void OnControllerChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            Window w = sender as Window;
            if (w == null)
            {
                //sender is a child element
                w = Window.GetWindow(sender);
            }

            if (w != null)
            {
                ViewControllerBase controller = e.NewValue as ViewControllerBase;
                if (controller != null)
                {
                    EventHandler<WindowShouldCloseEventArgs> windowShouldClose = null;
                    windowShouldClose = (x, y) =>
                    {
                        //check if the call is on the same thread as the window
                        //need to marshal onto UI thread
                        if (!w.Dispatcher.CheckAccess())
                        {
                            //different thread so invoke async on the dispatcher's thread
                            w.Dispatcher.Invoke(windowShouldClose, x, y);
                        }
                        else
                        {
                            //ok thread is being called on the same thread as the window

                            //now check if the current thread is modal, this should mean that the window was shown using ShowDialog
                            if (ComponentDispatcher.IsThreadModal)
                            {
                                //window should be Modal, i.e. a child window that is owned by a parent window
                                try
                                {
                                    //set the DialogResult value, close this window and return to the parent window
                                    //this will throw if the Window was not shown using ShowDialog
                                    w.DialogResult = y.DialogResult;
                                }
                                catch (InvalidOperationException)
                                {
                                }
                            }                           
                            w.Close();
                        }
                    };
                    controller.WindowShouldClose += windowShouldClose;

                    EventHandler closed = null;
                    closed = (x, y) =>
                    {
                        w.ClearValue(ViewControllerBehaviour.ControllerProperty);
                        controller.WindowShouldClose -= windowShouldClose;
                        w.Closed -= closed;
                        controller.OnWindowClosed();
                    };
                    w.Closed += closed;

                    EventHandler activated = (x, y) => controller.OnWindowActivated();
                    w.Activated += activated;

                    EventHandler deactivated = (x, y) => controller.OnWindowDeactivated();
                    w.Deactivated += deactivated;
                }
            }
        }
    }
}
