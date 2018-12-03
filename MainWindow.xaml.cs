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

            PathFigure pathFigure = new PathFigure();
            pathFigure.StartPoint = new Point(0, 0);
            BezierSegment bezier = new BezierSegment();
            bezier.Point1 = new Point(100, 300);
            bezier.Point2 = new Point(500, 500);
            bezier.Point3 = new Point(300, 200);

            PathSegmentCollection psc = new PathSegmentCollection();
            psc.Add(bezier);
            pathFigure.Segments = psc;

            PathGeometry pathgeom = new PathGeometry();
            pathgeom.Figures.Add(pathFigure);

            Path p = new Path();
            p.Stroke = Brushes.Red;
            p.Data = pathgeom;

            MainCanvas.Children.Add(p);
        }

        double ScaleRate = 1.1;
        void MainCanvas_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (e.Delta > 0)
                rt.ScaleX = rt.ScaleY *= ScaleRate;
            else if (rt.ScaleX > 1.0)
                rt.ScaleX = rt.ScaleY /= ScaleRate;

            sv.ScrollToHorizontalOffset(e.GetPosition(sv).X);
            sv.ScrollToVerticalOffset(e.GetPosition(sv).Y);
        }

        Point MiddleMousePressPoint;

        private void Sv_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.MiddleButton == MouseButtonState.Pressed)
            {
                sv.ScrollToHorizontalOffset(sv.HorizontalOffset + MiddleMousePressPoint.X - e.GetPosition(sv).X);
                sv.ScrollToVerticalOffset(sv.VerticalOffset + MiddleMousePressPoint.Y - e.GetPosition(sv).Y);
                MiddleMousePressPoint = e.GetPosition(sv);
            }
        }

        private void Sv_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.MiddleButton == MouseButtonState.Pressed)
                MiddleMousePressPoint = e.GetPosition(sv);
        }
    }

}
