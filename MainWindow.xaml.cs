using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace MapTest
{

    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            Path p = BezierPath(new Point(0,0), new Point(1600, 0), new Point(-800, 450), new Point(800, 450), Brushes.Red);
            MainCanvas.Children.Add(p);

            p = BezierPath(new Point(0, 0), new Point(0, 450), new Point(800, 0), new Point(800, 450), Brushes.Blue);
            MainCanvas.Children.Add(p);
        }

        private Path BezierPath(Point p0, Point p1, Point p2, Point p3, Brush brush)
        {
            PathFigure pathFigure = new PathFigure();
            pathFigure.StartPoint = p0;
            BezierSegment bezier = new BezierSegment();
            bezier.Point1 = p1;
            bezier.Point2 = p2;
            bezier.Point3 = p3;

            PathSegmentCollection psc = new PathSegmentCollection();
            psc.Add(bezier);
            pathFigure.Segments = psc;

            PathGeometry pathgeom = new PathGeometry();
            pathgeom.Figures.Add(pathFigure);

            Path p = new Path();
            p.Stroke = brush;
            p.Data = pathgeom;

            return p;
        }

        double ScaleRate = 1.1;
        void MainCanvas_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (e.Delta > 0)
                rt.ScaleX = rt.ScaleY *= ScaleRate;
            else if (rt.ScaleX > 1.0)
                rt.ScaleX = rt.ScaleY /= ScaleRate;

            e.GetPosition(MainCanvas);

            double MiddleWidth = e.GetPosition(MainCanvas).X / MainCanvas.RenderSize.Width;
            double MiddleHeight = e.GetPosition(MainCanvas).Y / MainCanvas.RenderSize.Height;

            scroller.ScrollToHorizontalOffset(scroller.ScrollableWidth * 0.5);// * MiddleWidth);
            scroller.ScrollToVerticalOffset(scroller.ScrollableHeight * 0.5);// * MiddleHeight);

            kk0.Content = e.GetPosition(MainCanvas);
            kk1.Content = MainCanvas.RenderSize;
        }

        Point MiddleMousePressPoint;

        private void Scroller_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.MiddleButton == MouseButtonState.Pressed)
                MiddleMousePressPoint = e.GetPosition(scroller);
        }

        private void Scroller_MouseMove(object sender, MouseEventArgs e)
        {
            Cursor = Cursors.Arrow;
            if (e.MiddleButton == MouseButtonState.Pressed)
            {
                scroller.ScrollToHorizontalOffset(scroller.HorizontalOffset + MiddleMousePressPoint.X - e.GetPosition(scroller).X);
                scroller.ScrollToVerticalOffset(scroller.VerticalOffset + MiddleMousePressPoint.Y - e.GetPosition(scroller).Y);
                MiddleMousePressPoint = e.GetPosition(scroller);
                Cursor = Cursors.ScrollAll;
            }
        }

        private void Scroller_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Cursor = Cursors.Arrow;
        }
    }

}
