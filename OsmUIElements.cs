using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Xml.Linq;

public class OsmUIElements
{

    static public UIElement CreateRoad(List<Point> points, XElement way)
    {
        Polyline line = new Polyline();
        PointCollection collection = new PointCollection();
        foreach (Point p in points)
        {
            collection.Add(p);
        }
        line.Points = collection;
        line.Stroke = new SolidColorBrush(Colors.Black);
        line.StrokeThickness = 1;

        return line;
    }

    static public UIElement CreateBuilding(List<Point> points, XElement way)
    {
        if (points.Count < 2)
            return null;

        var myPolygon = new Polygon
        {
            Stroke = Brushes.Black,
            Fill = Brushes.LightSeaGreen,
            StrokeThickness = 1,
            HorizontalAlignment = HorizontalAlignment.Left,
            VerticalAlignment = VerticalAlignment.Center
        };

        foreach (var tag in way.Elements("tag"))
        {
            if (tag.Attribute("k").Value == "name")
                myPolygon.ToolTip = tag.Attribute("v").Value;
        }

        foreach (var point in points)
            myPolygon.Points.Add(point);

        return myPolygon;
    }

}
