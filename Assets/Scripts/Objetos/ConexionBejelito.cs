using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Rigidbody))]
public class ConexionBejelito : MonoBehaviour
{
    public float maxDistance = 15f;
    public float lineWidth = 0.1f;
    public Material lineMaterial;

    private Dictionary<Transform, LineRenderer> connections = new Dictionary<Transform, LineRenderer>();

    void Update()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, maxDistance);
        HashSet<Transform> nearbyBalloons = new HashSet<Transform>();

        foreach (Collider col in colliders)
        {
            if (col.gameObject == this.gameObject)
                continue;

            ConexionBejelito otherBalloon = col.GetComponent<ConexionBejelito>();
            if (otherBalloon != null)
            {

                if (otherBalloon.GetInstanceID() > this.GetInstanceID())
                {
                    nearbyBalloons.Add(col.transform);
                    if (!connections.ContainsKey(col.transform))
                    {
                        CreateConnection(col.transform);
                    }
                }
            }
        }

        List<Transform> toRemove = new List<Transform>();
        foreach (var kvp in connections)
        {
            Transform other = kvp.Key;
            if (!nearbyBalloons.Contains(other) || Vector3.Distance(transform.position, other.position) > maxDistance)
            {
                if (kvp.Value != null)
                {
                    Destroy(kvp.Value.gameObject);
                }
                toRemove.Add(other);
            }
        }
        foreach (Transform tr in toRemove)
        {
            connections.Remove(tr);
        }

        foreach (var kvp in connections)
        {
            if (kvp.Value != null)
            {
                kvp.Value.SetPosition(0, transform.position);
                kvp.Value.SetPosition(1, kvp.Key.position);
            }
        }
    }

    private void CreateConnection(Transform other)
    {
        GameObject lineObj = new GameObject("ConnectionLine");

        lineObj.transform.parent = this.transform;

        LineRenderer lr = lineObj.AddComponent<LineRenderer>();
        lr.positionCount = 2;
        lr.startWidth = lineWidth;
        lr.endWidth = lineWidth;
        lr.material = lineMaterial != null ? lineMaterial : new Material(Shader.Find("Sprites/Default"));
        lr.startColor = Color.white;
        lr.endColor = Color.white;


        lr.SetPosition(0, transform.position);
        lr.SetPosition(1, other.position);

        connections.Add(other, lr);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, maxDistance);
    }
}