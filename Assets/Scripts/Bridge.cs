using System.Collections.Generic;
using UnityEngine;

public class Bridge
{
    private Island island1;
    private Island island2;
    private GameObject bridgeObject;
    private List<Vector3> bridgePath;
    
    public Island Island1
    {
        get { return island1; }
    }
    
    public Island Island2
    {
        get { return island2; }
    }
    
    public List<Vector3> BridgePath
    {
        get { return bridgePath; }
    }
    
    public Bridge(Island island1, Island island2)
    {
        this.island1 = island1;
        this.island2 = island2;
        bridgePath = new List<Vector3>();
    }

    public void DrawBridge()
    {
        CalculateBridgePath();
        CreateBridgeObject();
    }

    private void CalculateBridgePath()
    {
        bridgePath.Clear();

        // Calculate the direction and distance between the islands
        Vector3 bridgeDirection = island2.transform.position - island1.transform.position;
        bridgeDirection.y = 0f; // Ignore vertical difference
        float bridgeDistance = bridgeDirection.magnitude;

        // Calculate the number of segments based on the distance
        int numSegments = Mathf.CeilToInt(bridgeDistance / island1.tileSize);

        // Calculate the step size for each segment
        float stepSize = bridgeDistance / numSegments;
        Vector3 step = bridgeDirection.normalized * stepSize;

        // Calculate the start and end points of the bridge
        Vector3 startPoint = island1.GetTileAt(3, 1).transform.position + island1.GetTileAt(3, 1).transform.right * island1.tileSize * 0.5f;
        Vector3 endPoint = island2.GetTileAt(3, 1).transform.position + island2.GetTileAt(3, 1).transform.right * island2.tileSize * 0.5f;

        // Add the start point to the bridge path
        bridgePath.Add(startPoint);

        // Calculate the control points for the Bezier curve, adjusting the height to add a curve to the bridge
        float heightMultiplier = 0f; // Adjust this value to make the bridge more or less curved
        Vector3 controlPoint1 = startPoint + island1.transform.right * stepSize * 5f + Vector3.up * stepSize * heightMultiplier;
        Vector3 controlPoint2 = endPoint - island2.transform.right * stepSize * -5 + Vector3.up * stepSize * heightMultiplier;

        // Add intermediate positions along the bridge path using a Bezier curve
        for (int i = 1; i < numSegments; i++)
        {
            float t = (float)i / numSegments;
            Vector3 position = BezierCurve(startPoint, controlPoint1, controlPoint2, endPoint, t);
            bridgePath.Add(position);
        }

        // Add the end point to the bridge path
        bridgePath.Add(endPoint);
    }
    
    private Vector3 BezierCurve(Vector3 startPoint, Vector3 controlPoint1, Vector3 controlPoint2, Vector3 endPoint, float t)
{
    float u = 1 - t;
    float tt = t * t;
    float uu = u * u;
    float uuu = uu * u;
    float ttt = tt * t;

    Vector3 point = uuu * startPoint; // (1-t)^3 * P0
    point += 3 * uu * t * controlPoint1; // 3 * (1-t)^2 * t * P1
    point += 3 * u * tt * controlPoint2; // 3 * (1-t) * t^2 * P2
    point += ttt * endPoint; // t^3 * P3

    return point;
}

    private void CreateBridgeObject()
    {
        if (bridgePath.Count < 2)
            return;

        bridgeObject = new GameObject("Bridge");

        for (int i = 0; i < bridgePath.Count - 1; i++)
        {
            Vector3 startPosition = bridgePath[i];
            Vector3 endPosition = bridgePath[i + 1];

            CreateBridgeSegment(startPosition, endPosition);
        }
    }

    private void CreateBridgeSegment(Vector3 startPosition, Vector3 endPosition)
    {
        Vector3 centerPosition = (startPosition + endPosition) / 2f + Vector3.up * 1f;
        Quaternion rotation = Quaternion.LookRotation(endPosition - startPosition, Vector3.up);

        GameObject segment = GameObject.CreatePrimitive(PrimitiveType.Cube);
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
        if (bridgeObject != null)
        {
            Object.Destroy(bridgeObject);
        }
    }
}