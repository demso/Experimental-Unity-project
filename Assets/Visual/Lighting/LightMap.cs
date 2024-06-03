using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class LightMap
{
    public RenderTexture frameBuffer;
    //private RenderTarget2D pingPongBuffer;
    private GameObject lightMapMesh;

    private RayHandler rayHandler;
    //private Effect withoutShadowShader;
    private Material diffuseShader;
    private Material shadowShader;
    private Material withoutShadowShader;

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
        //pingPongBuffer = new RenderTarget2D(graphicsDevice, fboWidth, fboHeight, false, SurfaceFormat.ColorSRgb, DepthFormat.None, 0, RenderTargetUsage.PlatformContents);

        //spriteBatch = new SpriteBatch(Core.GraphicsDevice);

        //bloomComponent = new BloomComponent();
        //bloomComponent.Settings = BloomSettings.PresetSettings[1];

        lightMapMesh = CreateLightMapMesh();

        CreateShaders();

        //Vector2 futher = new Vector2(3.2307692308f / fboWidth, 3.2307692308f / fboHeight);
        //Vector2 closer = new Vector2(1.3846153846f / fboWidth, 1.3846153846f / fboHeight);
        //Vector2 f = futher * Vector2.UnitX;
        //Vector2 c = closer * Vector2.UnitX;
        //_sampleHorOffsets = new Vector2[5] { -f, -c, Vector2.Zero, c, f };
        //f = futher * Vector2.UnitY;
        //c = closer * Vector2.UnitY;
        //_sampleVertOffsets = new Vector2[5] { -f, -c, Vector2.Zero, c, f };
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
            if (RayHandler.isDiffuse)
            {
                diffuseShader.SetTexture("_MainTex", frameBuffer);
                diffuseShader.SetColor("_Ambient", c);
                lightMapMesh.GetComponent<MeshRenderer>().material = diffuseShader;
                lightMapMesh.GetComponent<MeshRenderer>().sortingOrder = 4;
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
                shadowShader.SetTexture("_MainTex", frameBuffer);
                shadowShader.SetColor("_Ambient", c);
                lightMapMesh.GetComponent<MeshRenderer>().material = shadowShader;
                lightMapMesh.GetComponent<MeshRenderer>().sortingOrder = 4;
                //shadowShader.Parameters["Ambient"].SetValue(new Vector4((float)c.R * (float)c.A / 255f,
                //    (float)c.G * (float)c.A / 255f, (float)c.B * (float)c.A / 255f, 1f - (float)c.A / 255f));
                //shadowShader.Parameters["RenderTargetTexture"].SetValue(frameBuffer);
                //shadowShader.CurrentTechnique.Passes[0].Apply();

                //rayHandler.shadowBlendFunc.Apply();

                //graphicsDevice.SetVertexBuffer(lightMapMesh);
                //graphicsDevice.DrawPrimitives(PrimitiveType.TriangleStrip, 0, lightMapMesh.VertexCount);
            }
        }
        else if (needed)
        {

            withoutShadowShader.SetTexture("_MainTex", frameBuffer);
            lightMapMesh.GetComponent<MeshRenderer>().material = withoutShadowShader;
            lightMapMesh.GetComponent<MeshRenderer>().sortingOrder = 4;
            //withoutShadowShader.CurrentTechnique.Passes[0].Apply();
            //withoutShadowShader.Parameters["RenderTargetTexture"].SetValue(frameBuffer);

            //graphicsDevice.SetVertexBuffer(lightMapMesh);
            //graphicsDevice.DrawPrimitives(PrimitiveType.TriangleStrip, 0, lightMapMesh.VertexCount);
        }

        //graphicsDevice.BlendState = (BlendState.Opaque);
    }

    public void GaussianBlur()
    {
       // bloomComponent.Draw(frameBuffer);
    }

    public void Dispose()
    {
        DisposeShaders();

        //lightMapMesh.Dispose();

        //frameBuffer.Dispose();
        //pingPongBuffer.Dispose();

        //bloomComponent.UnloadContent();
    }

    internal void CreateShaders()
    {
        DisposeShaders();

        shadowShader = new Material(Shader.Find("ShadowShader"));

        diffuseShader = new Material(Shader.Find("DiffuseShader"));

        withoutShadowShader = new Material(Shader.Find("WithoutShadowShader"));

    }

    private void DisposeShaders()
    {
        //shadowShader?.Dispose();
        //diffuseShader?.Dispose();
        //withoutShadowShader?.Dispose();
    }

    private GameObject CreateLightMapMesh()
    {
        Vector3[] vertices = new Vector3[4];
        Vector2[] uv = new Vector2[4];
        int[] triangles = new int[6];

        //VertexPositionTexture[] vertices = new VertexPositionTexture[4];
        // vertex coord
        vertices[0] = new Vector3(0, 1, 0);
        vertices[1] = new Vector3(1, 1, 0);
        vertices[2] = new Vector3(0, 0, 0);
        vertices[3] = new Vector3(1, 0, 0);

        uv[0] = new Vector2(0, 1);
        uv[1] = new Vector2(1, 1);
        uv[2] = new Vector2(0, 0);
        uv[3] = new Vector2(1, 0);

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

        Mesh mesh = new Mesh();

        mesh.vertices = vertices;
        mesh.uv = uv;
        mesh.triangles = triangles;

        GameObject gameObject = new GameObject("LightMapMesh", typeof(MeshFilter), typeof(MeshRenderer));
        gameObject.GetComponent<MeshFilter>().mesh = mesh;
        
        return gameObject;
    }
}
