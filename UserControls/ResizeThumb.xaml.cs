namespace UserControls
{
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;

    /// <summary>
    /// Interaction logic for ResizeThumb.xaml
    /// </summary>
    public partial class ResizeThumb : UserControl
    {
        /// <summary>
        /// Dependency property to set the ResizeTarget of the ResizeThumb
        /// </summary>
        public static readonly DependencyProperty ResizeTargetProperty =
            DependencyProperty.RegisterAttached("ResizeTarget",
            typeof(FrameworkElement), typeof(ResizeThumb),
            new FrameworkPropertyMetadata(null,
                FrameworkPropertyMetadataOptions.Inherits));

        /// <summary>
        /// Sets the ResizeTarget.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="value">The value.</param>
        public static void SetResizeTarget(UIElement element, FrameworkElement value)
        {
            element.SetValue(ResizeTargetProperty, value);
        }

        /// <summary>
        /// Gets the ResizeTarget.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns></returns>
        public static FrameworkElement GetResizeTarget(UIElement element)
        {
            return (FrameworkElement)element.GetValue(ResizeTargetProperty);
        }

        private bool resizing;

        public ResizeThumb()
        {
            InitializeComponent();
        }

        private void ResizeThumbDragStarted(object sender, DragStartedEventArgs e)
        {
            var resizeTarget = GetResizeTarget(this);

            this.resizing = resizeTarget != null
                && (resizeTarget.Width == resizeTarget.ActualWidth
                && resizeTarget.Height == resizeTarget.ActualHeight);
        }

        private void ResizeThumbDragDelta(object sender, DragDeltaEventArgs e)
        {
            var resizeTarget = GetResizeTarget(this);

            if (this.resizing)
            {
                if (e.HorizontalChange >= 1.0 || e.HorizontalChange <= 11.0)
                {
                    resizeTarget.Width += e.HorizontalChange;
                }

                if (e.VerticalChange >= 1.0 || e.VerticalChange <= 11.0)
                {
                    resizeTarget.Height += e.VerticalChange;
                }
            }
        }

        private void ResizeThumbDragCompleted(object sender, DragCompletedEventArgs e)
        {
            this.resizing = false;
        }
    }
}
