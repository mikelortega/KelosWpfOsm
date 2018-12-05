using Microsoft.Win32;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace MapTest
{

    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
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

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Multiselect = false,
                Filter = "Osm files (*.osm)|*.osm",
                InitialDirectory = Path.GetFullPath(Path.Combine(System.Environment.CurrentDirectory, @"..\..\SampleFiles"))
            };
            if (openFileDialog.ShowDialog() == true)
            {
                KelosOSM kosm = new KelosOSM();
                kosm.LoadFile(openFileDialog.FileName, MainCanvas);
            }
        }
    }

}
