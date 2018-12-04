using GeoUtility.GeoSystem;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Shapes;
using System.Xml.Linq;

public class KelosOSM
{
    public double m_Longitude = -1.984656;
    public double m_Latitude = 43.322048;
    public string m_RelativePath = "";
    private Dictionary<long, Geographic> m_NodeGeoLocs;

    private Point NodePosition(long id)
    {
        UTM GeoLoc = new Geographic(m_Longitude, m_Latitude).ConvertTo<UTM>();
        Point position = new Point();
        UTM utm = (UTM)m_NodeGeoLocs[id];
        position.X = GeoLoc.East - utm.East;
        position.Y = GeoLoc.North - utm.North;
        return position;
    }

    public void LoadFile(string file_path)
    {
        if (!System.IO.File.Exists(file_path))
            return;

        XDocument doc = XDocument.Load(file_path);

        m_NodeGeoLocs = new Dictionary<long, Geographic>();

        foreach (var node in doc.Root.Elements("node"))
        {
            if (node.Attribute("lon") != null && node.Attribute("lat") != null)
            {
                Geographic geoloc = new Geographic(double.Parse(node.Attribute("lon").Value), double.Parse(node.Attribute("lat").Value));
                m_NodeGeoLocs[long.Parse(node.Attribute("id").Value)] = geoloc;
            }
        }
    }

    public void AddPointsToCanvas(System.Windows.Controls.Canvas canvas)
    {
        var myPolygon = new Polygon();
        myPolygon.Stroke = System.Windows.Media.Brushes.Black;
        myPolygon.Fill = System.Windows.Media.Brushes.LightSeaGreen;
        myPolygon.StrokeThickness = 2;
        myPolygon.HorizontalAlignment = HorizontalAlignment.Left;
        myPolygon.VerticalAlignment = VerticalAlignment.Center;

        foreach (var node in m_NodeGeoLocs)
            myPolygon.Points.Add(NodePosition(node.Key));

        canvas.Children.Add(myPolygon);
    }

}
