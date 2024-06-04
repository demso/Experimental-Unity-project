using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointLight : PositionalLight
{
    /// <summary>
    /// Creates light shaped as a circle with given radius
    /// 
    /// @param rayHandler
    ///            not {@code null} instance of RayHandler
    /// @param rays
    ///            number of rays - more rays make light to look more realistic
    ///            but will decrease performance, can't be less than MIN_RAYS
    /// @param color
    ///            color, set to {@code null} to use the default color
    /// @param distance
    ///            distance of light, soft shadow length is set to distance * 0.1f
    /// @param x
    ///            horizontal position in world coordinates
    /// @param y
    ///            vertical position in world coordinates
    /// </summary>
    public PointLight()
    {
        direction = 0;
    }

    public new void Awake()
    {
        base.Awake();
    }

    public override void Update()
    {
        //Debug.Log(gameObject.transform.position);
        SetPosition(gameObject.transform.position);
        
        //UpdateBody();
        if (dirty) SetEndPoints();

        if (Cull()) return;
        if (staticLight && !dirty) return;

        dirty = false;
        UpdateMesh();
        
        rayHandler.lightRenderedLastFrame++;
        gameObject.GetComponent<MeshFilter>().mesh = lightMesh;
        //gameObject.GetComponent<MeshRenderer>().material = rayHandler.lightShader;
        lightMesh.bounds = new Bounds(new Vector3(0,0), new Vector3(100, 100));
    }

    /// <summary>
    /// Sets light distance
    /// 
    /// <p>MIN value capped to 0.1f meter
    /// <p>Actual recalculations will be done only on {@link #update()} call
    /// </summary>
    public override void SetDistance(float dist)
    {
        dist *= RayHandler.gammaCorrectionParameter;
        this.distance = dist < 0.01f ? 0.01f : dist;
        dirty = true;
    }

    /// <summary>
    /// Updates light basing on it's distance and rayNum
    /// </summary>
    void SetEndPoints()
    {
        float angleNum = 360f / (rayNum);
        for (int i = 0; i < rayNum; i++)
        {
            float angle = angleNum * i;
            sin[i] = (float)Math.Sin( Mathf.Deg2Rad * angle);
            if (Math.Abs(sin[i]) < 0.001)
                sin[i] = 0; 
            cos[i] = (float)Math.Cos( Mathf.Deg2Rad * angle);
            if (Math.Abs(cos[i]) < 0.001)
                cos[i] = 0;
            endY[i] = distance * sin[i];
            endX[i] = distance * cos[i];
        }
    }

    /// <summary>
    /// Not applicable for this light type
    /// </summary>
    [Obsolete]
    public override void SetDirection(float directionDegree)
    {
    }
}
