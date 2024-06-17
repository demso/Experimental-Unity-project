using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SocialPlatforms.Impl;

public abstract class PositionalLight : CustomLight
{
    protected Vector2 tmpEnd = new();
    internal Vector2 start = new();

    protected float[] sin;
    protected float[] cos;

    protected float[] endX;
    protected float[] endY;

    private Vector3[] vertices;
    private Color[] colors;

    protected Bounds bounds = new Bounds(new Vector3(), new Vector3(1000,1000,0));

    public new void Awake()
    {
        base.Awake();

        lightMesh = new Mesh();
        softShadowMesh = new Mesh();
        softShadowMesh.bounds = new Bounds(new Vector3(), new Vector3(100, 100));
        
        SetRayNum(rayNum);
        SetMesh();
    }

    public override void Update()
    {
        UpdateBody();

        if (Cull()) return;
        if (staticLight && !dirty) return;

        dirty = false;
        UpdateMesh();
    }

    public override void Render() 
    {
        if (rayHandler.culling && culled) return;

        rayHandler.lightRenderedLastFrame++;
        
        rayHandler.commandBuffer.DrawMesh(lightMesh, gameObject.transform.localToWorldMatrix, rayHandler.lightShader);

        if (soft && !xray)
        {
            rayHandler.commandBuffer.DrawMesh(softShadowMesh, gameObject.transform.localToWorldMatrix, rayHandler.lightShader);
        }
    }

    public override Vector2 GetPosition()
    {
        return gameObject.transform.position;
    }
    public override float GetX()
    {
        return start.x;
    }

    public override float GetY()
    {
        return start.y;
    }

    public bool Contains(Vector2 pos)
    {
        return Contains(pos.x, pos.y);
    }

    // public override bool Contains(float x, float y)
    // {
    //     // fast fail
    //     var x_d = start.x - x;
    //     var y_d = start.y - y;
    //     var dst2 = x_d * x_d + y_d * y_d;
    //     if (distance * distance <= dst2) return false;
    //
    //     // actual check
    //     var oddNodes = false;
    //     var x2 = mx[rayNum] = start.x;
    //     var y2 = my[rayNum] = start.y;
    //     float x1, y1;
    //     for (var i = 0; i <= rayNum; x2 = x1, y2 = y1, ++i)
    //     {
    //         x1 = mx[i];
    //         y1 = my[i];
    //         if ((y1 < y && y2 >= y) || (y1 >= y && y2 < y))
    //             if ((y - y1) / (y2 - y1) * (x2 - x1) < x - x1)
    //                 oddNodes = !oddNodes;
    //     }
    //
    //     return oddNodes;
    // }

    protected override void SetRayNum(int rays)
    {
        base.SetRayNum(rays);

        sin = new float[rays];
        cos = new float[rays];
        endX = new float[rays];
        endY = new float[rays];
        
        vertices = new Vector3[lightVertexNum];
        colors = new Color[lightVertexNum];
        triangles = new int[lightVertexNum * 2];
        softTri = new int[lightVertexNum * 2];
        softColors = new Color[lightVertexNum];
        softVerts = new Vector3[lightVertexNum];
    }

    protected bool Cull()
    {
        culled = rayHandler.culling && !rayHandler.intersect(
            start.x, start.y, distance + softShadowLength);
        return culled;
    }

    private Transform tempBodyTransform;

    protected void UpdateBody()
    {
        if (staticLight) return;

        GameObject go = gameObject;

        var vec = go.transform.position;

        var angle = go.transform.rotation.eulerAngles.z;
        
        start.x = vec.x;
        start.y = vec.y;
        SetDirection(angle * Mathf.Rad2Deg); // rads to degrees * Mathf.Rad2Deg
    }

    protected void UpdateMesh()
    {
        for (var i = 0; i < rayNum; i++)
        {
            m_index = i;
            f[i] = 1f;
            
            mx[i] = endX[i];
            my[i] = endY[i];
            
            tmpEnd.x = endX[i];
            tmpEnd.y = endY[i];
            
            if (!xray && !rayHandler.pseudo3d)
                RayHit(Physics2D.Raycast(start, tmpEnd.normalized, distance, layerMask: Int32.MaxValue & ~Globals.IGNORE_LIGHTS_LAYER));
        }

        SetMesh();
    }
    
    public new void RayHit(RaycastHit2D rayHit)
    {
        
        //Debug.Log((rayHit.collider.includeLayers.value));
        if (rayHit.collider != null)
        {
            //Debug.Log((rayHit.collider.includeLayers.value & Globals.TRANSPARENT_CONTACT_FILTER));
            Vector2 point = rayHit.point;
            float fraction = rayHit.fraction;
            mx[m_index] = point.x - start.x;
            my[m_index] = point.y - start.y;
            f[m_index] = fraction;
        }
    }

    private int vertexIndex = 0;
    private int rayEndIndex = 0;
    // private float[] _fractions;
    // private float[] _softShadowFractions;
    private int[] triangles;

    private Vector3[] softVerts;
    private Color[] softColors;
    private int[] softTri;
    Vector2 tmpVec;

    protected void SetMesh()
    {
        vertexIndex = 0;
        rayEndIndex = 0;
        
        for (var i = 0; i < rayNum; i += 1)
        {
            vertices[vertexIndex] = Vector3.zero;
            colors[vertexIndex] = color;
            triangles[vertexIndex] = vertexIndex;
            vertexIndex++;
        
            tmpVec.Set(mx[rayEndIndex], my[rayEndIndex]);
        
            vertices[vertexIndex] = tmpVec;
            colors[vertexIndex] = color * (1 - f[rayEndIndex]);
            triangles[vertexIndex] = vertexIndex;
            vertexIndex++;
            
            rayEndIndex++;
        
            tmpVec.Set(mx[rayEndIndex], my[rayEndIndex]);
        
            vertices[vertexIndex] = tmpVec;
            colors[vertexIndex] = color * (1 - f[rayEndIndex]);
            triangles[vertexIndex] = vertexIndex;
            vertexIndex++;
        }
        
        tmpVec.Set(mx[0], my[0]);
        
        vertices[vertexIndex-1] = tmpVec;
        colors[vertexIndex-1] = color * (1 - f[0]);
        triangles[vertexIndex-1] = vertexIndex-1;

        lightMesh.SetVertices(vertices, 0, vertexIndex); 
        lightMesh.SetColors(colors, 0, vertexIndex);
        lightMesh.SetTriangles(triangles,  0, vertexIndex, 0, false);

        if (!soft || xray || rayHandler.pseudo3d) return;
        
        var triangleIndex = 0;
        
        vertexIndex = 0;
        for (var i = 0; i < rayNum; i++)
        {
            var s = 1 - f[i];
            tmpVec.Set(mx[i], my[i]);
            softVerts[vertexIndex] = tmpVec;
            softColors[vertexIndex] = color * s;
            softTri[triangleIndex++] = vertexIndex;
            vertexIndex++;
        
            tmpVec.Set(mx[i] + s * softShadowLength * cos[i], my[i] + s * softShadowLength * sin[i]);
            softVerts[vertexIndex] = tmpVec;
            softColors[vertexIndex] = new Color(0, 0, 0, 0);
            softTri[triangleIndex++] = vertexIndex;
            vertexIndex++;
            if (vertexIndex >= rayNum * 2)
            {
                softTri[triangleIndex++] = 0;
        
                softTri[triangleIndex++] = 0;
                softTri[triangleIndex++] = vertexIndex - 1;
                softTri[triangleIndex++] = 1;
            }
            else
            {
                softTri[triangleIndex++] = vertexIndex;
        
                softTri[triangleIndex++] = vertexIndex;
                softTri[triangleIndex++] = vertexIndex - 1;
                softTri[triangleIndex++] = vertexIndex + 1;
            }
        }
        softShadowMesh.SetVertices(softVerts, 0, vertexIndex);
        softShadowMesh.SetColors(softColors, 0, vertexIndex);
        softShadowMesh.SetTriangles(softTri, 0, triangleIndex, 0, false);
    }
}