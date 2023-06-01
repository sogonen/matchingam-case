using System.Collections.Generic;
using UnityEngine;

public class Bridge
{
    private GameObject bridgeObject;

    public Bridge(IslandPair islandPair)
    {
        IslandPair = islandPair;
        BridgePath = new List<Vector3>();
    }

    public IslandPair IslandPair { get; }

    public List<Vector3> BridgePath { get; }

    public void DrawBridge()
    {
        CalculateBridgePath();
        CreateBridgeObject();
    }

    private void CalculateBridgePath()
    {
        BridgePath.Clear();

        // Calculate the direction and distance between the islands
        var bridgeDirection = IslandPair.SecondIsland.transform.position - IslandPair.FirstIsland.transform.position;
        bridgeDirection.y = 0f; // Ignore vertical difference
        var bridgeDistance = bridgeDirection.magnitude;

        // Calculate the number of segments based on the distance
        var numSegments = Mathf.CeilToInt(bridgeDistance / IslandPair.FirstIsland.tileSize);

        // Calculate the step size for each segment
        var stepSize = bridgeDistance / numSegments;
        var step = bridgeDirection.normalized * stepSize;

        // Calculate the start and end points of the bridge
        var startPoint = IslandPair.FirstIsland.GetTileAt(3, 1).transform.position +
                         IslandPair.FirstIsland.GetTileAt(3, 1).transform.right * IslandPair.FirstIsland.tileSize *
                         0.5f;
        var endPoint = IslandPair.SecondIsland.GetTileAt(3, 1).transform.position +
                       IslandPair.SecondIsland.GetTileAt(3, 1).transform.right * IslandPair.SecondIsland.tileSize *
                       0.5f;

        // Add the start point to the bridge path
        BridgePath.Add(startPoint);

        // Calculate the control points for the Bezier curve, adjusting the height to add a curve to the bridge
        var heightMultiplier = 0f; // Adjust this value to make the bridge more or less curved
        var controlPoint1 = startPoint + IslandPair.FirstIsland.transform.right * stepSize * 5f +
                            Vector3.up * stepSize * heightMultiplier;
        var controlPoint2 = endPoint - IslandPair.SecondIsland.transform.right * stepSize * -5 +
                            Vector3.up * stepSize * heightMultiplier;

        // Add intermediate positions along the bridge path using a Bezier curve
        for (var i = 1; i < numSegments; i++)
        {
            var t = (float)i / numSegments;
            var position = BezierCurve(startPoint, controlPoint1, controlPoint2, endPoint, t);
            BridgePath.Add(position);
        }

        // Add the end point to the bridge path
        BridgePath.Add(endPoint);
    }

    private Vector3 BezierCurve(Vector3 startPoint, Vector3 controlPoint1, Vector3 controlPoint2, Vector3 endPoint,
        float t)
    {
        var u = 1 - t;
        var tt = t * t;
        var uu = u * u;
        var uuu = uu * u;
        var ttt = tt * t;

        var point = uuu * startPoint; // (1-t)^3 * P0
        point += 3 * uu * t * controlPoint1; // 3 * (1-t)^2 * t * P1
        point += 3 * u * tt * controlPoint2; // 3 * (1-t) * t^2 * P2
        point += ttt * endPoint; // t^3 * P3

        return point;
    }

    private void CreateBridgeObject()
    {
        if (BridgePath.Count < 2)
            return;

        bridgeObject = new GameObject("Bridge");

        for (var i = 0; i < BridgePath.Count - 1; i++)
        {
            var startPosition = BridgePath[i];
            var endPosition = BridgePath[i + 1];

            CreateBridgeSegment(startPosition, endPosition);
        }
    }

    private void CreateBridgeSegment(Vector3 startPosition, Vector3 endPosition)
    {
        var centerPosition = (startPosition + endPosition) / 2f + Vector3.up * 1f;
        var rotation = Quaternion.LookRotation(endPosition - startPosition, Vector3.up);

        var segment = GameObject.CreatePrimitive(PrimitiveType.Cube);
        segment.name = "Segment";
        segment.transform.position = centerPosition;
        segment.transform.rotation = rotation;
        segment.transform.localScale = new Vector3(Vector3.Distance(startPosition, endPosition), 0.2f, 2f);
        segment.transform.SetParent(bridgeObject.transform);
        segment.GetComponent<Renderer>().material = GetBridgeMaterial();
    }


    private Material GetBridgeMaterial()
    {
        // Replace this with your own material or load it from Resources
        return new Material(Shader.Find("Standard"));
    }

    public void DestroyBridge()
    {
        if (bridgeObject != null) Object.Destroy(bridgeObject);
    }
}