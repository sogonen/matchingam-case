using System.Collections.Generic;
using UnityEngine;

public class Bridge : MonoBehaviour
{
    public Material bridgeMaterial; // Material for the bridge
    public float bridgeWidth = 0.1f; // Width of the bridge
    public float bridgeHeight = 0.1f; // Height of the bridge

    private List<GameObject> bridgeSegments = new List<GameObject>(); // List to store individual bridge segments

    public void CreateBridge(Vector3 startPosition, Vector3 endPosition)
    {
        // Calculate the number of bridge segments needed
        int numSegments = Mathf.CeilToInt(Vector3.Distance(startPosition, endPosition));

        // Calculate the direction and length of the bridge
        Vector3 direction = (endPosition - startPosition).normalized;
        float length = Vector3.Distance(startPosition, endPosition);

        // Calculate the position increment for each bridge segment
        Vector3 positionIncrement = direction * (length / numSegments);

        // Create each bridge segment
        for (int i = 0; i < numSegments; i++)
        {
            Vector3 segmentPosition = startPosition + (positionIncrement * i);

            // Create a bridge segment as a simple cube
            GameObject segment = GameObject.CreatePrimitive(PrimitiveType.Cube);
            segment.transform.position = segmentPosition;
            segment.transform.localScale = new Vector3(bridgeWidth, bridgeHeight, positionIncrement.magnitude);
            segment.transform.LookAt(endPosition);
            segment.GetComponent<Renderer>().material = bridgeMaterial;

            // Add the segment to the list
            bridgeSegments.Add(segment);
        }
    }

    public void DestroyBridge()
    {
        // Destroy each bridge segment
        foreach (GameObject segment in bridgeSegments)
        {
            Destroy(segment);
        }

        // Clear the list
        bridgeSegments.Clear();
    }
}