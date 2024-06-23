using System;
using UnityEngine;
using UnityEngine.Rendering;

public class PointLight : PositionalLight
{
    public PointLight()
    {
        direction = 0;
    }

    public new void Awake()
    {
        base.Awake();
        pars.renderingLayerMask = 5;
    }

    internal RenderTexture renderTexture;
    
    private RenderParams pars;

    public override void Update()
    {
        UpdateBody();
        if (dirty) SetEndPoints();

        if (Cull()) return;
        if (staticLight && !dirty) return;

        dirty = false;
        UpdateMesh();
        
        rayHandler.lightRenderedLastFrame++;
        
        lightMesh.bounds = bounds;
    }
    
    public override void SetDistance(float dist)
    {
        dist *= RayHandler.gammaCorrectionParameter;
        this.distance = dist < 0.01f ? 0.01f : dist;
        dirty = true;
    }

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
    
    public override void SetDirection(float directionDegree)
    {
    }
}
