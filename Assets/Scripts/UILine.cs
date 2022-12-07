using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UILine : Graphic
{
    public Vector3 pointA;
    public Vector3 pointB;
    public Vector3 pointC;
    public Vector3 pointD;
    public float thickness;
    public bool absolutePosition;

    public Vector3 getPoint(float t)
    {
        return pointA * (1 - t) * (1 - t) * (1 - t) + pointB * 3 * t * (1 - t) * (1 - t) + pointC * 3 * t * t * (1 - t) + pointD * t * t * t;
    }
    public Vector3 getDerivative(float t)
    {
        return pointA * (-3 * t * t + 6 * t - 3) + pointB * 3 * (3 * t * t - 4 * t + 1) + pointC * 3 * (-3 * t * t + 2 * t) + pointD * 3 * t * t;
    }
    public Vector3 getSecDerivative(float t)
    {
        return pointA * (-6 * t + 6) + pointB * 3 * (6 * t - 4) + pointC * 3 * (-6 * t + 2) + pointD * 6 * t;
    }
    public Vector3 GetCurvature(float t, bool normal)
    {
        Vector3 deriv = getDerivative(t);
        Vector3 secDeriv = getSecDerivative(t) / deriv.sqrMagnitude;
        deriv /= deriv.magnitude;
        if (!normal)
            return Vector3.Cross(deriv, secDeriv);
        return Vector3.Cross(Vector3.Cross(deriv, secDeriv), deriv);
    }

    private void OnDrawGizmos()
    {
        Color col = Color.white - color;
        col.a = 1f;
        Gizmos.color = col;
        Gizmos.DrawWireSphere(pointA + (absolutePosition ? Vector3.zero : transform.position), thickness / 2f);    
        Gizmos.DrawWireSphere(pointB + (absolutePosition ? Vector3.zero : transform.position), thickness / 4f);    
        Gizmos.DrawWireSphere(pointC + (absolutePosition ? Vector3.zero : transform.position), thickness / 4f);    
        Gizmos.DrawWireSphere(pointD + (absolutePosition ? Vector3.zero : transform.position), thickness / 2f);    
    }

    void AddSquare(VertexHelper vh, Vector3 p1, Vector3 p2, Vector3 p3, Vector3 p4)
    {
        UIVertex vertex = UIVertex.simpleVert;
        vertex.color = color;
        vertex.position = p1 - (absolutePosition ? transform.position : Vector3.zero);
        vh.AddVert(vertex);
        vertex.position = p2 - (absolutePosition ? transform.position : Vector3.zero);
        vh.AddVert(vertex);
        vertex.position = p3 - (absolutePosition ? transform.position : Vector3.zero);
        vh.AddVert(vertex);
        vertex.position = p4 - (absolutePosition ? transform.position : Vector3.zero);
        vh.AddVert(vertex);

        vh.AddTriangle(vertCounts, 1 + vertCounts, 2 + vertCounts);
        vh.AddTriangle(vertCounts + 1, 3 + vertCounts, 2 + vertCounts);
        vertCounts += 4;
    }
    
    void AddTriangle(VertexHelper vh, Vector3 p1, Vector3 p2, Vector3 p3)
    {
        UIVertex vertex = UIVertex.simpleVert;
        vertex.color = color;
        vertex.position = p1 - (absolutePosition ? transform.position : Vector3.zero);
        vh.AddVert(vertex);
        vertex.position = p2 - (absolutePosition ? transform.position : Vector3.zero);
        vh.AddVert(vertex);
        vertex.position = p3 - (absolutePosition ? transform.position : Vector3.zero);
        vh.AddVert(vertex);

        vh.AddTriangle(vertCounts, 1 + vertCounts, 2 + vertCounts);
        vertCounts += 3;
    }

    int vertCounts;
    protected override void OnPopulateMesh(VertexHelper vh)
    {
        transform.SetAsFirstSibling();

        vh.Clear();
        vertCounts = 0;

        AddTriangle(vh, Vector3.down * 25f, Vector3.right * 12f, Vector3.up * 25f);

        Vector3 topleft = getPoint(0);
        Vector3 bottomleft = getPoint(0);
        Vector3 normal = getDerivative(0);
        normal = new Vector3(-normal.y, normal.x).normalized * .5f;
        topleft += normal * thickness;
        bottomleft -= normal * thickness;

        for (float i = 0.03125f; i <= 1f; i += 0.03125f)
        {
            Vector3 topright = getPoint(i);
            Vector3 bottomright = getPoint(i);
            normal = getDerivative(i);
            normal = new Vector3(-normal.y, normal.x).normalized * .5f;
            topright += normal * thickness;
            bottomright -= normal * thickness;

            AddSquare(vh, bottomleft, bottomright, topleft, topright);

            topleft = topright;
            bottomleft = bottomright;
        }
    }
}
