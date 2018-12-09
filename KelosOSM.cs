using GeoUtility.GeoSystem;
using System.Collections.Generic;
using System.Windows;
using System.Xml.Linq;
using System.Windows.Controls;
using System;
using System.Linq;

public class KelosOSM
{
    public double m_Longitude = -1.984656;
    public double m_Latitude = 43.322048;
    private Dictionary<long, Geographic> m_NodeGeoLocs;

    private Point NodePosition(long id)
    {
        return NodePosition(m_NodeGeoLocs[id].Longitude, m_NodeGeoLocs[id].Latitude);
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

        XDocument doc = XDocument.Load(file_path);

        double minlat, maxlat, minlon, maxlon;
        minlat = minlon = double.MaxValue;
        maxlat = maxlon = double.MinValue;

        foreach (var bound in doc.Root.Elements("bounds"))
        {
            minlat = Math.Min(minlat, double.Parse(bound.Attribute("minlat").Value));
            maxlat = Math.Max(maxlat, double.Parse(bound.Attribute("maxlat").Value));
            minlon = Math.Min(minlon, double.Parse(bound.Attribute("minlon").Value));
            maxlon = Math.Max(maxlon, double.Parse(bound.Attribute("maxlon").Value));
        }

        m_Longitude = (minlon + maxlon) * 0.5;
        m_Latitude = (minlat + maxlat) * 0.5;

        canvas.Width = NodePosition(minlon, m_Latitude).X - NodePosition(maxlon, m_Latitude).X;
        canvas.Height = NodePosition(m_Longitude, minlat).Y - NodePosition(m_Longitude, maxlat).Y;

        m_NodeGeoLocs = new Dictionary<long, Geographic>();

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

        CreateAreas(doc, canvas);
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
                    point = new Point(canvas.Width * 0.5 - point.X, canvas.Height * 0.5 + point.Y);
                    points.Add(point);
                }

                var road = OsmUIElements.CreateRoad(points, way);

                canvas.Children.Add(road);
            }
        }
    }

    private void CreateAreas(XDocument doc, Canvas canvas)
    {
        string[] areaStrings = { "area", "leisure" };

        foreach (var way in doc.Root.Elements("way"))
        {
            bool isArea = false;
            foreach (var tag in way.Elements("tag"))
            {
                if (areaStrings.Contains(tag.Attribute("k").Value))
                    isArea = true;
            }

            if (isArea)
            {
                List<Point> points = new List<Point>();
                foreach (var nd in way.Elements("nd"))
                {
                    Point point = NodePosition(long.Parse(nd.Attribute("ref").Value));
                    point = new Point(canvas.Width * 0.5 - point.X, canvas.Height * 0.5 + point.Y);
                    points.Add(point);
                }

                var area = OsmUIElements.CreateArea(points, way);

                canvas.Children.Add(area);
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
                    point = new Point(canvas.Width * 0.5 - point.X, canvas.Height * 0.5 + point.Y);
                    points.Add(point);
                }

                var building = OsmUIElements.CreateBuilding(points, way);

                canvas.Children.Add(building);
            }
        }
    }

}
