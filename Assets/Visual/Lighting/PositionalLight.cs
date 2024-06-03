using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public abstract class PositionalLight : Light
{
    private Color tmpColor = new();

    protected Vector2 tmpEnd = new();
    protected Vector2 start = new();

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

    public new void Awake()
    {
        base.Awake();
        start = gameObject.transform.position;

        lightMesh = new Mesh();
        softShadowMesh = new Mesh();
        vertices = new Vector3[lightVertexNum];
        colors = new Color[lightVertexNum];
        SetRayNum(rayNum);
        SetMesh();
    }

    //public struct CustomVertex(Vector2 position, Color color, float fog) : IVertexType
    //{
    //    public Vector2 Position = position;
    //    public Color Color = color;
    //    public float Fog = fog;
    //    VertexDeclaration IVertexType.VertexDeclaration
    //    {
    //        get => VertexDeclaration;
    //    }

    //    public static readonly VertexDeclaration VertexDeclaration = new(
    //        new VertexElement(0, VertexElementFormat.Vector2, VertexElementUsage.Position, 0),
    //        new VertexElement(8, VertexElementFormat.Color, VertexElementUsage.Color, 0),
    //        new VertexElement(12, VertexElementFormat.Single, VertexElementUsage.Fog, 0)
    //    );
    //}

    public override void Update()
    {
        SetPosition(gameObject.transform.position);
        
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
        gameObject.GetComponent<MeshFilter>().mesh = lightMesh;
        rayHandler.commandBuffer.Clear();
        rayHandler.commandBuffer.DrawRenderer(gameObject.GetComponent<MeshRenderer>(), rayHandler.lightShader);
        Graphics.ExecuteCommandBuffer(rayHandler.commandBuffer);
        // Core.GraphicsDevice.SetVertexBuffer(lightMesh);
        // Core.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleList, 0, lightVertexNum);

        if (soft && !xray && !rayHandler.pseudo3d)
        {
            gameObject.GetComponent<MeshFilter>().mesh  = softShadowMesh;
            rayHandler.commandBuffer.Clear();
            rayHandler.commandBuffer.DrawRenderer(gameObject.GetComponent<MeshRenderer>(), rayHandler.lightShader);
            Graphics.ExecuteCommandBuffer(rayHandler.commandBuffer);
            // Core.GraphicsDevice.SetVertexBuffer(softShadowMesh);
            // Core.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleStrip, 0, softShadowVertexNum);
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

    public override bool Contains(float x, float y)
    {
        // fast fail
        var x_d = start.x - x;
        var y_d = start.y - y;
        var dst2 = x_d * x_d + y_d * y_d;
        if (distance * distance <= dst2) return false;

        // actual check
        var oddNodes = false;
        var x2 = mx[rayNum] = start.x;
        var y2 = my[rayNum] = start.y;
        float x1, y1;
        for (var i = 0; i <= rayNum; x2 = x1, y2 = y1, ++i)
        {
            x1 = mx[i];
            y1 = my[i];
            if ((y1 < y && y2 >= y) || (y1 >= y && y2 < y))
                if ((y - y1) / (y2 - y1) * (x2 - x1) < x - x1)
                    oddNodes = !oddNodes;
        }

        return oddNodes;
    }

    protected override void SetRayNum(int rays)
    {
        base.SetRayNum(rays);

        sin = new float[rays];
        cos = new float[rays];
        endX = new float[rays];
        endY = new float[rays];
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
            tmpEnd.x = endX[i] + start.x;
            mx[i] = tmpEnd.x;
            tmpEnd.y = endY[i] + start.y;
            my[i] = tmpEnd.y;
            if (rayHandler.world != null && !xray && !rayHandler.pseudo3d)
                rayHit(Physics2D.Raycast(start, tmpEnd.normalized, distance = (tmpEnd - start).magnitude));
        }

        SetMesh();
    }

    private int vertexIndex = 0;
    private int rayEndIndex = 0;
    // private float[] _fractions;
    // private float[] _softShadowFractions;
    private int[] triangles;
    Vector2 tmpVec;

    protected void SetMesh()
    {
        // ray starting point

        vertexIndex = 0;
        rayEndIndex = 0;
        // _fractions = new float[lightVertexNum];
        // _softShadowFractions = new float[rayNum * 2];
        triangles = new int[lightVertexNum * 2];
        try
        {

        
        for (var i = 0; i < rayNum; i += 1)
        {
            vertices[vertexIndex] = start;
            colors[vertexIndex] = color;
            //_fractions[vertexIndex] = 1;
            triangles[vertexIndex] = vertexIndex;
            //vertices[size] = new CustomVertex(start, color, 1);//* rayHandler.SimToDisplay
            vertexIndex++;

            tmpVec.Set(mx[rayEndIndex], my[rayEndIndex]);

            vertices[vertexIndex] = tmpVec;
            //_fractions[vertexIndex] = 1 - f[rayEndIndex];
            colors[vertexIndex] = color * (1 - f[rayEndIndex]);
            triangles[vertexIndex] = vertexIndex;
            //vertices[size] = new CustomVertex(tmpVec, color, 1 - f[tempIndex]);//* rayHandler.SimToDisplay
            vertexIndex++;
            rayEndIndex++;

            tmpVec.Set(mx[rayEndIndex], my[rayEndIndex]);

            vertices[vertexIndex] = tmpVec;
            //_fractions[vertexIndex] = 1 - f[rayEndIndex];
            colors[vertexIndex] = color * (1 - f[rayEndIndex]);
            triangles[vertexIndex] = vertexIndex;
            //vertices[size] = new CustomVertex(tmpVec, color, 1 - f[tempIndex]);//* rayHandler.SimToDisplay
            vertexIndex++;
        }

        lightMesh.SetVertices(vertices, 0, vertexIndex);
        lightMesh.SetColors(colors, 0, vertexIndex);
        lightMesh.triangles = new int[vertexIndex];
        Array.Copy(triangles, lightMesh.triangles, vertexIndex);

        if (!soft || xray || rayHandler.pseudo3d) return;

        var triangleIndex = 0;

        vertexIndex = 0;
        for (var i = 0; i < rayNum; i++)
        {
            var s = 1 - f[i];
            tmpVec.Set(mx[i], my[i]);
            vertices[vertexIndex] = tmpVec;
            colors[vertexIndex] = color * s;
            //_softShadowFractions[vertexIndex] = s;
            triangles[triangleIndex++] = vertexIndex;
            vertexIndex++;
            //vertices[vertexIndex++] = new CustomVertex(tmpVec * rayHandler.SimToDisplay, color, s);

            tmpVec.Set(mx[i] + s * softShadowLength * cos[i], my[i] + s * softShadowLength * sin[i]);
            vertices[vertexIndex] = tmpVec;
            colors[vertexIndex] = color * s;
            //_softShadowFractions[vertexIndex] = s;
            triangles[triangleIndex++] = vertexIndex;
            vertexIndex++;
            //vertices[vertexIndex++] = new CustomVertex(tmpVec * rayHandler.SimToDisplay, Color.Transparent, 0);
            if (vertexIndex >= lightVertexNum)
            {
                triangles[triangleIndex++] = 0;

                triangles[triangleIndex++] = 0;
                triangles[triangleIndex++] = vertexIndex - 1;
                triangles[triangleIndex++] = 1;
            }
            else
            {
                triangles[triangleIndex++] = vertexIndex;

                triangles[triangleIndex++] = vertexIndex;
                triangles[triangleIndex++] = vertexIndex - 1;
                triangles[triangleIndex++] = vertexIndex + 1;
            }
        }

        softShadowMesh.SetVertices(vertices, 0, vertexIndex);
        softShadowMesh.SetColors(colors, 0, vertexIndex);
        softShadowMesh.triangles = new int[triangleIndex];
        Array.Copy(triangles, softShadowMesh.triangles, triangleIndex);
        
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
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