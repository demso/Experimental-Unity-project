using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class LightMap
{
    public RenderTexture frameBuffer;
    public RenderTexture pingPongBuffer;
    private GameObject lightMapMesh;

    private RayHandler rayHandler;
    //private Effect withoutShadowShader;
    private Material diffuseShader;
    private Material shadowShader;
    private Material withoutShadowShader;
    private Material blurShader;
    

    private Mesh baseMesh, baseMesh2;

    //CommandBuffer commandBuffer = new CommandBuffer();

    //private BloomComponent bloomComponent;

    //public SpriteBatch spriteBatch;

    internal bool lightMapDrawingDisabled;

    private int fboWidth, fboHeight;

    public LightMap(RayHandler rayHandler, int fboWidth, int fboHeight)
    {
        this.rayHandler = rayHandler;
        //graphicsDevice = Core.GraphicsDevice;

        if (fboWidth <= 0)
            fboWidth = 1;
        if (fboHeight <= 0)
            fboHeight = 1;

        this.fboWidth = fboWidth;
        this.fboHeight = fboHeight;

        frameBuffer = new RenderTexture(fboWidth, fboHeight, 0);
        pingPongBuffer = new RenderTexture(fboWidth, fboHeight, 0);

        lightMapMesh = CreateLightMapMesh();

        CreateShaders();
    }

    public void Render()
    {
        bool needed = rayHandler.lightRenderedLastFrame > 0;

        Color c = rayHandler.ambientLight;
        //Material shader = shadowShader;
        //BlendFunc blFn = rayHandler.shadowBlendFunc;

        //Core.GraphicsDevice.SetRenderTarget(rayHandler.RenderHere);
        
        if (rayHandler.shadows)
        {
            //rayHandler.commandBuffer.Clear();
            if (RayHandler.isDiffuse)
            {
                diffuseShader.SetTexture("_MainTex", frameBuffer);
                diffuseShader.SetColor("_Ambient", c);
                MeshRenderer renderer  = lightMapMesh.GetComponent<MeshRenderer>();
               
                renderer.material = diffuseShader;
                renderer.sortingOrder = 4;
                
                //rayHandler.commandBuffer.DrawRenderer(renderer, shadowShader);
                //Graphics.DrawTexture(new Rect(0,0, Screen.width, Screen.height), frameBuffer, diffuseShader);
                //diffuseShader.SetInteger("SrcMode", BlendMode.DstColor);
                //diffuseShader.Parameters["Ambient"].SetValue(c.ToVector4());
                //diffuseShader.Parameters["RenderTargetTexture"].SetValue(frameBuffer);
                //diffuseShader.CurrentTechnique.Passes[0].Apply();

                //rayHandler.diffuseBlendFunc.Apply();

                //graphicsDevice.SetVertexBuffer(lightMapMesh);
                //graphicsDevice.DrawPrimitives(PrimitiveType.TriangleStrip, 0, lightMapMesh.VertexCount);
            }
            else
            {
                // shadowShader.SetTexture("_MainTex", frameBuffer);
                // shadowShader.SetColor("_Ambient", c);
                // MeshRenderer renderer  = lightMapMesh.GetComponent<MeshRenderer>();
                // renderer.material = shadowShader;
                // renderer.sortingOrder = 4;
                // rayHandler.commandBuffer.DrawRenderer(renderer, shadowShader);
                
                
                
                //shadowShader.Parameters["Ambient"].SetValue(new Vector4((float)c.R * (float)c.A / 255f,
                //    (float)c.G * (float)c.A / 255f, (float)c.B * (float)c.A / 255f, 1f - (float)c.A / 255f));
                //shadowShader.Parameters["RenderTargetTexture"].SetValue(frameBuffer);
                //shadowShader.CurrentTechnique.Passes[0].Apply();

                //rayHandler.shadowBlendFunc.Apply();

                //graphicsDevice.SetVertexBuffer(lightMapMesh);
                //graphicsDevice.DrawPrimitives(PrimitiveType.TriangleStrip, 0, lightMapMesh.VertexCount);
            }
            //Graphics.ExecuteCommandBuffer(rayHandler.commandBuffer);
        }
        else if (needed)
        {

            // withoutShadowShader.SetTexture("_MainTex", frameBuffer);
            // lightMapMesh.GetComponent<MeshRenderer>().material = withoutShadowShader;
            // lightMapMesh.GetComponent<MeshRenderer>().sortingOrder = 4;
            //withoutShadowShader.CurrentTechnique.Passes[0].Apply();
            //withoutShadowShader.Parameters["RenderTargetTexture"].SetValue(frameBuffer);

            //graphicsDevice.SetVertexBuffer(lightMapMesh);
            //graphicsDevice.DrawPrimitives(PrimitiveType.TriangleStrip, 0, lightMapMesh.VertexCount);
        }

        //graphicsDevice.BlendState = (BlendState.Opaque);
    }

    public void GaussianBlur()
    { 
        var cb = rayHandler.commandBuffer;
        pingPongBuffer.Release();
        blurShader.SetFloat("_FBO_W", frameBuffer.width);
        blurShader.SetFloat("_FBO_H", frameBuffer.height);
        
        for (var i = 0; i < rayHandler.blurNum; i++)
        {
            cb.SetRenderTarget(pingPongBuffer);
            cb.SetViewProjectionMatrices(Matrix4x4.identity, Matrix4x4.identity);
            blurShader.SetTexture("_MainTex" , frameBuffer);
            blurShader.SetVector("_Dir", new Vector4(1, 0));
            cb.DrawMesh(baseMesh, Matrix4x4.identity, blurShader);
            
            Graphics.ExecuteCommandBuffer(cb);
            cb.Clear();
        
            cb.SetRenderTarget(frameBuffer);
            blurShader.SetTexture("_MainTex", pingPongBuffer);
            blurShader.SetVector("_Dir", new Vector4(0, 1));
            cb.DrawMesh(baseMesh, Matrix4x4.identity, blurShader);
        
            Graphics.ExecuteCommandBuffer(cb);
            cb.Clear();
        }
    }

    internal void CreateShaders()
    {
        shadowShader = new Material(Shader.Find("z/ShadowShader"));
        diffuseShader = new Material(Shader.Find("z/DiffuseShader"));
        withoutShadowShader = new Material(Shader.Find("z/WithoutShadowSHader"));
        blurShader = new Material(Shader.Find("z/BlurShader"));
    }

    private GameObject CreateLightMapMesh()
    {
        Vector3[] vertices = new Vector3[4];
        Vector2[] uv = new Vector2[4];
        int[] triangles = new int[6];

        //VertexPositionTexture[] vertices = new VertexPositionTexture[4];
        // vertex coord
        vertices[0] = new Vector3(-1, 1, 0);
        vertices[1] = new Vector3(1, 1, 0);
        vertices[2] = new Vector3(-1, -1, 0);
        vertices[3] = new Vector3(1, -1, 0);

        uv[0] = new Vector2(0, 0);
        uv[1] = new Vector2(1, 0);
        uv[2] = new Vector2(0, 1);
        uv[3] = new Vector2(1, 1);

        triangles[0] = 0;
        triangles[1] = 1;
        triangles[2] = 2;
        triangles[3] = 2;
        triangles[4] = 1;
        triangles[5] = 3;

        // tex coords
        //vertices[0].TextureCoordinate = new Vector2(0f, 1f);
        //vertices[1].TextureCoordinate = new Vector2(0f, 0f);
        //vertices[2].TextureCoordinate = new Vector2(1f, 1f);
        //vertices[3].TextureCoordinate = new Vector2(1f, 0f);

        baseMesh = new Mesh();
        baseMesh2 = new Mesh();

        baseMesh.vertices = vertices;
        baseMesh.uv = uv;
        baseMesh.triangles = triangles;
        baseMesh.RecalculateBounds(MeshUpdateFlags.DontRecalculateBounds);
        baseMesh.bounds = new Bounds(new Vector3(0,0,0), new Vector3(10000,10000,0));
        
        uv[0] = new Vector2(0, 1);
        uv[1] = new Vector2(1, 1);
        uv[2] = new Vector2(0, 0);
        uv[3] = new Vector2(1, 0);
        
        baseMesh2.vertices = vertices;
        baseMesh2.uv = uv;
        baseMesh2.triangles = triangles;
        baseMesh2.RecalculateBounds(MeshUpdateFlags.DontRecalculateBounds);
        baseMesh2.bounds = new Bounds(new Vector3(0,0,0), new Vector3(10000,10000,0));

        GameObject gameObject = new GameObject("LightMapMesh", typeof(MeshFilter), typeof(MeshRenderer));
        //gameObject.SetActive(false);
        gameObject.GetComponent<MeshFilter>().mesh = baseMesh;
        
        
        return gameObject;
    }
}
