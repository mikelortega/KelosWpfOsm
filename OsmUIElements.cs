using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Xml.Linq;

public class OsmUIElements
{

    static public UIElement CreateRoad(List<Point> points, XElement way)
    {
        double width = 3.0;
        Color color = Colors.Black;
        string[] pedestrianStrings = { "footway", "steps", "bridleway" };

        foreach (var tag in way.Elements("tag"))
        {
            if (tag.Attribute("k").Value == "lanes")
                width = double.Parse(tag.Attribute("v").Value) * 3.0;
            if (tag.Attribute("k").Value == "highway")
            {
                if (tag.Attribute("v").Value == "cycleway")
                {
                    color = Colors.Blue;
                    width = 1.0;
                }
                if (tag.Attribute("v").Value == "pedestrian")
                {
                    color = Colors.Gray;
                    width = 1.0;
                }
                if (pedestrianStrings.Contains(tag.Attribute("v").Value))
                {
                    color = Colors.Red;
                    width = 1.0;
                }
            }
        }

        Polyline line = new Polyline();
        PointCollection collection = new PointCollection();
        foreach (Point p in points)
        {
            collection.Add(p);
        }
        line.Points = collection;
        line.Stroke = new SolidColorBrush(color);
        line.StrokeThickness = width;

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


    static public UIElement CreateArea(List<Point> points, XElement way)
    {
        if (points.Count < 2)
            return null;

        var myPolygon = new Polygon
        {
            Stroke = Brushes.Black,
            Fill = Brushes.Gray,
            StrokeThickness = 0,
            HorizontalAlignment = HorizontalAlignment.Left,
            VerticalAlignment = VerticalAlignment.Center
        };

        Color color = Colors.Gray;

        foreach (var tag in way.Elements("tag"))
        {
            if (tag.Attribute("k").Value == "name")
                myPolygon.ToolTip = tag.Attribute("v").Value;
            if (tag.Attribute("k").Value == "landuse")
            {
                if (tag.Attribute("v").Value == "grass")
                    color = Colors.Green;
            }
            if (tag.Attribute("k").Value == "leisure")
            {
                if (tag.Attribute("v").Value == "garden")
                    color = Colors.Green;
            }
        }

        myPolygon.Fill = new SolidColorBrush(color);

        foreach (var point in points)
            myPolygon.Points.Add(point);

        return myPolygon;
    }
}
