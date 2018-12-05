using GeoUtility.GeoSystem;
using System.Collections.Generic;
using System.Windows;
using System.Xml.Linq;
using System.Windows.Controls;
using System.Linq;

public class KelosOSM
{
    public double m_Longitude = -1.984656;
    public double m_Latitude = 43.322048;
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

    private Point NodePosition(double lon, double lat)
    {
        UTM GeoLoc = new Geographic(m_Longitude, m_Latitude).ConvertTo<UTM>();
        UTM utm = new Geographic(lon, lat).ConvertTo<UTM>();
        Point position = new Point();
        position.X = GeoLoc.East - utm.East;
        position.Y = GeoLoc.North - utm.North;
        return position;
    }

    public void LoadFile(string file_path, Canvas canvas)
    {
        if (!System.IO.File.Exists(file_path))
            return;

        canvas.Children.Clear();

        XDocument doc = XDocument.Load(file_path);

        m_NodeGeoLocs = new Dictionary<long, Geographic>();

        double minlat = double.Parse(doc.Root.Element("bounds").Attribute("minlat").Value);
        double maxlat = double.Parse(doc.Root.Element("bounds").Attribute("maxlat").Value);
        double minlon = double.Parse(doc.Root.Element("bounds").Attribute("minlon").Value);
        double maxlon = double.Parse(doc.Root.Element("bounds").Attribute("maxlon").Value);

        m_Longitude = (minlon + maxlon) * 0.5;
        m_Latitude = (minlat + maxlat) * 0.5;

        canvas.Width = NodePosition(minlon, m_Latitude).X;
        canvas.Height = NodePosition(m_Longitude, minlat).Y;

        foreach (var node in doc.Root.Elements("node"))
        {
            if (node.Attribute("lon") != null && node.Attribute("lat") != null)
            {
                double lon = double.Parse(node.Attribute("lon").Value);
                double lat = double.Parse(node.Attribute("lat").Value);
                Geographic geoloc = new Geographic(lon, lat);
                m_NodeGeoLocs[long.Parse(node.Attribute("id").Value)] = geoloc;
            }
        }

        CreateRoads(doc, canvas);
        CreateBuildings(doc, canvas);
    }

    private void CreateRoads(XDocument doc, Canvas canvas)
    {
        foreach (var way in doc.Root.Elements("way"))
        {
            bool isRoad = false;
            foreach (var tag in way.Elements("tag"))
            {
                if (tag.Attribute("k").Value == "highway")
                    isRoad = true;
            }

            if (isRoad)
            {
                List<Point> points = new List<Point>();
                foreach (var nd in way.Elements("nd"))
                {
                    Point point = NodePosition(long.Parse(nd.Attribute("ref").Value));
                    point = new Point(canvas.Width * 0.5 - point.X * 0.5, canvas.Height * 0.5 + point.Y * 0.5);
                    points.Add(point);
                }

                var road = OsmUIElements.CreateRoad(points, way);

                canvas.Children.Add(road);
            }
        }
    }

    private void CreateBuildings(XDocument doc, Canvas canvas)
    {
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
            foreach (var tag in way.Elements("tag"))
            {
                if (tag.Attribute("k").Value == "building")
                    isBuilding = true;
                if (tag.Attribute("k").Value == "building:part")
                    isBuilding = true;
            }

            if (isBuilding || outerOuterBuildingRelations.Contains(long.Parse(way.Attribute("id").Value)))
            {
                List<Point> points = new List<Point>();
                foreach (var nd in way.Elements("nd"))
                {
                    Point point = NodePosition(long.Parse(nd.Attribute("ref").Value));
                    point = new Point(canvas.Width * 0.5 - point.X * 0.5, canvas.Height * 0.5 + point.Y * 0.5);
                    points.Add(point);
                }

                var building = OsmUIElements.CreateBuilding(points, way);

                canvas.Children.Add(building);
            }
        }
    }

}
