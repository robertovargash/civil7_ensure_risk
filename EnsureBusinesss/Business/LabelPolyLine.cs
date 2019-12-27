using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace EnsureBusinesss.Business
{
    public class LabelPolyLine : TextBlock
    {
        public RiskPolyLine Line { get; set; }

        public LabelPolyLine() : base()
        {

        }

        #region Double click

        /// <summary>
        /// Holds the millis for the first click.
        /// </summary>
        private long lastClickedTime = 0;

        /// <summary>
        /// Delegate definition for the call back function.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public delegate void DoubleClickHandler(object sender, MouseButtonEventArgs e);

        /// <summary>
        /// Holder for the call back function.
        /// </summary>
        private DoubleClickHandler doubleClickCallBackFunction;
        /// <summary>
        /// This will attach a double click to an object.
        /// </summary>
        /// <remarks>
        /// Due to Silverlight lack of double click event a custom implementation is needed.
        /// this function and its adjacent handlers will attach a double click event to any object
        /// deriving from UIElement.
        /// </remarks>
        /// <param name="doubleClickSender">the UIElement to attach the event to.</param>
        /// <param name="function">the function to call upon the double click event.</param>
        public void AttachDoubleClick(object doubleClickTarget, DoubleClickHandler function)
        {
            UIElement target = (UIElement)doubleClickTarget;
            target.MouseLeftButtonUp += new MouseButtonEventHandler(target_MouseLeftButtonUp);
            target.MouseLeftButtonDown += new MouseButtonEventHandler(target_MouseLeftButtonDown);
            doubleClickCallBackFunction = function;
        }

        private void target_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            lastClickedTime = DateTime.Now.Ticks / 10000;
        }

        private void target_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            long currentMillis = DateTime.Now.Ticks / 10000;

            if (currentMillis - lastClickedTime < 100 && currentMillis - lastClickedTime > 0)
            {
                doubleClickCallBackFunction(sender, e);
            }

        }

        #endregion

    }
}
