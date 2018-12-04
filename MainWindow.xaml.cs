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

            //Path p = BezierPath(new Point(0,0), new Point(1600, 0), new Point(-800, 450), new Point(800, 450), Brushes.Red);
            //MainCanvas.Children.Add(p);

            //p = BezierPath(new Point(0, 0), new Point(0, 450), new Point(800, 0), new Point(800, 450), Brushes.Blue);
            //MainCanvas.Children.Add(p);

            KelosOSM kosm = new KelosOSM();
            kosm.LoadFile("../../Donostia.osm");
            //kosm.AddPointsToCanvas(MainCanvas);
            kosm.CreateBuildings(MainCanvas);
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

        void MainCanvas_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            const double ScaleRate = 1.1;

            Point mouseAtMainCanvas = e.GetPosition(MainCanvas);
            Point mouseAtScrollViewer = e.GetPosition(MainScrollViewer);

            ScaleTransform st = MainCanvas.LayoutTransform as ScaleTransform;

            if (e.Delta > 0 && st.ScaleX < 64)
                st.ScaleX = st.ScaleY = st.ScaleX * ScaleRate;
            else if (st.ScaleX > 1)
                st.ScaleX = st.ScaleY = st.ScaleX / ScaleRate;

            MainScrollViewer.ScrollToHorizontalOffset(0);
            MainScrollViewer.ScrollToVerticalOffset(0);
            UpdateLayout();

            Vector offset = MainCanvas.TranslatePoint(mouseAtMainCanvas, MainScrollViewer) - mouseAtScrollViewer;
            MainScrollViewer.ScrollToHorizontalOffset(offset.X);
            MainScrollViewer.ScrollToVerticalOffset(offset.Y);
            UpdateLayout();

            e.Handled = true;
        }

        Point MiddleMousePressPoint;

        private void MainScrollViewer_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.MiddleButton == MouseButtonState.Pressed)
                MiddleMousePressPoint = e.GetPosition(MainScrollViewer);
        }

        private void MainScrollViewer_MouseMove(object sender, MouseEventArgs e)
        {
            Cursor = Cursors.Arrow;
            if (e.MiddleButton == MouseButtonState.Pressed)
            {
                MainScrollViewer.ScrollToHorizontalOffset(MainScrollViewer.HorizontalOffset + MiddleMousePressPoint.X - e.GetPosition(MainScrollViewer).X);
                MainScrollViewer.ScrollToVerticalOffset(MainScrollViewer.VerticalOffset + MiddleMousePressPoint.Y - e.GetPosition(MainScrollViewer).Y);
                MiddleMousePressPoint = e.GetPosition(MainScrollViewer);
                Cursor = Cursors.ScrollAll;
            }
        }

        private void MainScrollViewer_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Cursor = Cursors.Arrow;
        }

    }

}
