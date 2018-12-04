using GeoUtility.GeoSystem;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Shapes;
using System.Xml.Linq;
using System.Windows.Controls;

public class KelosOSM
{
    public double m_Longitude = -1.984656;
    public double m_Latitude = 43.322048;
    public string m_RelativePath = "../../Donostia.osm";
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

    public void AddPointsToCanvas(Canvas canvas)
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

    public void CreateBuildings(Canvas canvas)
    {
        XDocument doc = XDocument.Load(m_RelativePath);

        List<long> outerOuterBuildingRelations = new List<long>();
        foreach (var relation in doc.Root.Elements("relation"))
        {
            foreach (var tag in relation.Elements("tag"))
            {
                if (tag.Attribute("k").Value == "building" && tag.Attribute("v").Value == "yes")
                {
                    foreach (var member in relation.Elements("member"))
                    {
                        if (member.Attribute("type").Value == "way" && member.Attribute("role").Value == "outer")
                            outerOuterBuildingRelations.Add(long.Parse(member.Attribute("ref").Value));
                    }
                }
            }
        }

        foreach (var way in doc.Root.Elements("way"))
        {
            bool isBuilding = false;
            float height = 10.0f;
            foreach (var tag in way.Elements("tag"))
            {
                if (tag.Attribute("k").Value == "building")
                    isBuilding = true;
                if (tag.Attribute("k").Value == "building:part")
                    isBuilding = true;
                if (tag.Attribute("k").Value == "levels" || tag.Attribute("k").Value == "building:levels")
                    height = float.Parse(tag.Attribute("v").Value) * 3.0f;
                if (tag.Attribute("k").Value == "height")
                    height = float.Parse(tag.Attribute("v").Value);
            }

            if (isBuilding || outerOuterBuildingRelations.Contains(long.Parse(way.Attribute("id").Value)))
            {
                List<Point> positions = new List<Point>();
                foreach (var nd in way.Elements("nd"))
                    positions.Add(NodePosition(long.Parse(nd.Attribute("ref").Value)));

                CreateBuilding(positions, height, canvas);
            }
        }
    }

    private void CreateBuilding(List<Point> positions, float height, Canvas canvas)
    {
        if (positions.Count < 2)
            return;

        var myPolygon = new Polygon();
        myPolygon.Stroke = System.Windows.Media.Brushes.Black;
        myPolygon.Fill = System.Windows.Media.Brushes.LightSeaGreen;
        myPolygon.StrokeThickness = 2;
        myPolygon.HorizontalAlignment = HorizontalAlignment.Left;
        myPolygon.VerticalAlignment = VerticalAlignment.Center;

        foreach (var point in positions)
            myPolygon.Points.Add(new Point(canvas.Width * 0.5 - point.X * 0.5, canvas.Height * 0.5 + point.Y * 0.5));

        canvas.Children.Add(myPolygon);
    }

}
