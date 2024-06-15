using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SocialPlatforms.Impl;

public abstract class PositionalLight : CustomLight
{
    private Color tmpColor = new();

    protected Vector2 tmpEnd = new();
    public Vector2 start = new();

    protected Rigidbody2D body;
    protected float bodyOffsetX;
    protected float bodyOffsetY;
    protected float bodyAngleOffset;

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
        //start = gameObject.transform.position;

        lightMesh = new Mesh();
        //lightMesh.bounds = new Bounds(new Vector3(-100, -100), new Vector3(100, 100));
        softShadowMesh = new Mesh();
        softShadowMesh.bounds = new Bounds(new Vector3(), new Vector3(100, 100));
        
        SetRayNum(rayNum);
        SetMesh();
    }

    public override void Update()
    {
        //SetPosition(gameObject.transform.position);
        
        UpdateBody();

        if (Cull()) return;
        if (staticLight && !dirty) return;

        dirty = false;
        UpdateMesh();
        
        //gameObject.GetComponent<MeshFilter>().mesh = lightMesh;
        //gameObject.GetComponent<MeshRenderer>().material = rayHandler.lightShader;
        //gameObject.GetComponent<MeshRenderer>().material = rayHandler.lightShader;//
        // gameObject.GetComponent<MeshRenderer>().sortingOrder = 0;
        // gameObject.GetComponent<MeshRenderer>().renderingLayerMask = 0;
        //gameObject.GetComponent<MeshRenderer>().rendererPriority = -1;
    }

    public override void Render()
    {
        if (rayHandler.culling && culled) return;

        rayHandler.lightRenderedLastFrame++;
        
        rayHandler.commandBuffer.DrawMesh(lightMesh, gameObject.transform.localToWorldMatrix, rayHandler.lightShader);
        //Graphics.ExecuteCommandBuffer(rayHandler.commandBuffer);

        if (soft && !xray)
        {
            rayHandler.commandBuffer.DrawMesh(softShadowMesh, gameObject.transform.localToWorldMatrix, rayHandler.lightShader);
            //     gameObject.GetComponent<MeshFilter>().mesh  = softShadowMesh;
            //     rayHandler.commandBuffer.Clear();
            //     rayHandler.commandBuffer.DrawRenderer(gameObject.GetComponent<MeshRenderer>(), rayHandler.lightShader);
            //     Graphics.ExecuteCommandBuffer(rayHandler.commandBuffer);
            //     // Core.GraphicsDevice.SetVertexBuffer(softShadowMesh);
            //     // Core.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleStrip, 0, softShadowVertexNum);
        }
    }

    public override void AttachToBody(Rigidbody2D body)
    {
        AttachToBody(body, 0f, 0f, 0f);
    }

    public void AttachToBody(Rigidbody2D body, float offsetX, float offsetY)
    {
        AttachToBody(body, offsetX, offsetY, 0f);
    }

    public void AttachToBody(Rigidbody2D body, float offsetX, float offsetY, float degrees)
    {
        this.body = body;
        bodyOffsetX = offsetX;
        bodyOffsetY = offsetY;
        bodyAngleOffset = degrees;
        if (staticLight) dirty = true;
    }

    public override Vector2 GetPosition()
    {
        tmpPosition.x = start.x;
        tmpPosition.y = start.y;
        return tmpPosition;
    }

    public override Rigidbody2D GetBody()
    {
        return body;
    }

    public override float GetX()
    {
        return start.x;
    }

    public override float GetY()
    {
        return start.y;
    }

    public override void SetPosition(float x, float y)
    {
        start.x = x;
        start.y = y;
        if (staticLight) dirty = true;
    }

    public override void SetPosition(Vector2 position)
    {
        start.x = position.x;
        start.y = position.y;
        if (staticLight) dirty = true;
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
        if (body == null || staticLight) return;

        var vec = body.position;

        var angle = body.rotation;
        var cos = (float)Math.Cos(angle);
        var sin = (float)Math.Sin(angle);
        var dX = bodyOffsetX * cos - bodyOffsetY * sin;
        var dY = bodyOffsetX * sin + bodyOffsetY * cos;
        start.x = vec.x + dX;
        start.y = vec.y + dY;
        SetDirection(bodyAngleOffset + angle); // rads to degrees * Mathf.Rad2Deg
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
                RayHit(Physics2D.Raycast(start, tmpEnd.normalized, distance));
        }

        SetMesh();
    }
    
    public new void RayHit(RaycastHit2D rayHit)
    {
        if (rayHit.collider != null)
        {
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

    public float GetBodyOffsetX()
    {
        return bodyOffsetX;
    }

    public float GetBodyOffsetY()
    {
        return bodyOffsetY;
    }

    public float GetBodyAngleOffset()
    {
        return bodyAngleOffset;
    }

    public void SetBodyOffsetX(float bodyOffsetX)
    {
        this.bodyOffsetX = bodyOffsetX;
    }

    public void SetBodyOffsetY(float bodyOffsetY)
    {
        this.bodyOffsetY = bodyOffsetY;
    }

    public void SetBodyAngleOffset(float bodyAngleOffset)
    {
        this.bodyAngleOffset = bodyAngleOffset;
    }
}