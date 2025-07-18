using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SocialPlatforms.Impl;

public class RayHandler : MonoBehaviour
{
    // Start is called before the first frame update
    private new Camera camera;

    static float GAMMA_COR = 0.625f;

    static bool gammaCorrection = false;
    internal static float gammaCorrectionParameter = 1f;

    static float dynamicShadowColorReduction = 1;

    internal static bool isDiffuse = true;
    internal Color ambientLight = new Color();

    internal List<CustomLight> lightList = new List<CustomLight>();
    internal List<CustomLight> disabledLights = new List<CustomLight>();

    public LightMap lightMap;
    internal Material lightShader;

    public bool culling = false;
    public bool shadows = true;
    bool blur = true;

    internal bool pseudo3d = false;

    public int blurNum = 2;

    internal int lightRenderedLastFrame = 0;

    float x1, x2, y1, y2;

    public int SimToDisplay = 32;

    internal Physics2D world;

    internal RenderTexture renTar;
    private int[] data;

    internal CommandBuffer commandBuffer;
    
    void Start()
    {
        int w = Screen.width;
        int h = Screen.height;
        renTar = new RenderTexture(w, h, 0);
        data = new int[w * h];
        resizeFBO(w / 4, h / 4);
        lightShader = new Material(Shader.Find("z/LightShader"));
        commandBuffer = new CommandBuffer();
        setAmbientLight(0,0,0,1);
        setShadows(true);
        useDiffuseLight(true);
        camera = GameObject.Find("Main Camera").GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        blur = blurNum > 0;
        Vector2 camPos = camera.transform.position;
        setCullingInfo(camPos.x, camPos.y, camera.pixelWidth, camera.pixelHeight);
        renderLights();
    }
    
    ~RayHandler()
    {
        Dispose();
    }

    public void resizeFBO(int fboWidth, int fboHeight)
    {
        lightMap = new LightMap(this, fboWidth, fboHeight);
    }

    public void setCullingInfo(float x, float y, float viewPortWidth, float viewPortHeight)
    {
        float halfViewPortWidth = viewPortWidth * 0.5f / SimToDisplay;
        x1 = (x - halfViewPortWidth);
        x2 = (x + halfViewPortWidth);

        float halfViewPortHeight = viewPortHeight * 0.5f / SimToDisplay;
        y1 = (y - halfViewPortHeight);
        y2 = (y + halfViewPortHeight);
    }

    public bool intersect(float x, float y, float radius)
    {
        Vector3 pos = camera.WorldToViewportPoint(new Vector3(x, y));
        return  pos is {x: >= 0 and <= 1, y: >= 0 and <= 1 };
    }

    public void prepareRender()
    {
        lightRenderedLastFrame = 0;
        
        bool useLightMap = shadows || blur;
        if (useLightMap)
        {
            lightMap.frameBuffer.Release();
            commandBuffer.Clear();
            
            commandBuffer.SetViewProjectionMatrices(camera.worldToCameraMatrix, camera.projectionMatrix);
            commandBuffer.SetRenderTarget(lightMap.frameBuffer);
        }
        
        foreach (CustomLight light in lightList)
        {
            light.Render();
        }
        
        if (useLightMap)
            Graphics.ExecuteCommandBuffer(commandBuffer);
        
        commandBuffer.Clear();

        bool needed = lightRenderedLastFrame > 0;
        if (needed && blur)
            lightMap.GaussianBlur();
    }

    public void renderLights()
    {
        prepareRender();
        lightMap.Render();
    }

    public void renderOnly()
    {
        lightMap.Render();
    }

    // public bool pointAtLight(float x, float y)
    // {
    //     foreach (CustomLight light in lightList)
    //     {
    //         if (light.Contains(x, y)) return true;
    //     }
    //     return false;
    // }
    //
    // public bool pointAtShadow(float x, float y)
    // {
    //     foreach (CustomLight light in lightList)
    //     {
    //         if (light.Contains(x, y)) return false;
    //     }
    //     return true;
    // }

    public void Dispose()
    {
        //removeAll();
        //renTar.Dispose();
        //if (lightMap != null) lightMap.Dispose();
        //if (lightShader != null) lightShader.Dispose();
    }

    // public void removeAll()
    // {
    //     foreach (Light light in lightList)
    //     {
    //         light.Dispose();
    //     }
    //     lightList.Clear();
    //
    //     foreach (Light light in disabledLights)
    //     {
    //         light.Dispose();
    //     }
    //     disabledLights.Clear();
    // }

    public void setCulling(bool culling)
    {
        this.culling = culling;
    }

    public void setBlur(bool blur)
    {
        this.blur = blur;
    }

    public void setBlurNum(int blurNum)
    {
        this.blurNum = blurNum;
    }

    public void setShadows(bool shadows)
    {
        this.shadows = shadows;
    }

    public void setAmbientLight(float ambientLight)
    {
        this.ambientLight.a = ambientLight;
    }

    public void setAmbientLight(float r, float g, float b, float a)
    {
        ambientLight = new Color(r, g, b, a);
    }

    public void setAmbientLight(Color ambientLightColor)
    {
        ambientLight = ambientLightColor;
    }

    public void setWorld(Physics2D world)
    {
        this.world = world;
    }

    public static bool getGammaCorrection()
    {
        return gammaCorrection;
    }

    public void applyGammaCorrection(bool gammaCorrectionWanted)
    {
        gammaCorrection = gammaCorrectionWanted;
        gammaCorrectionParameter = gammaCorrection ? GAMMA_COR : 1f;
        lightMap.CreateShaders();
    }

    public void setDiffuseLight(bool useDiffuse)
    {
        isDiffuse = useDiffuse;
        lightMap.CreateShaders();
    }

    public static bool isDiffuseLight()
    {
        return isDiffuse;
    }

    public static float getDynamicShadowColorReduction()
    {
        return dynamicShadowColorReduction;
    }

    public static void useDiffuseLight(bool useDiffuse)
    {
        isDiffuse = useDiffuse;
    }

    public static void setGammaCorrection(bool gammaCorrectionWanted)
    {
        gammaCorrection = gammaCorrectionWanted;
        gammaCorrectionParameter = gammaCorrection ? GAMMA_COR : 1f;
    }

    public void setLightMapRendering(bool isAutomatic)
    {
        lightMap.lightMapDrawingDisabled = !isAutomatic;
    }

    public RenderTexture getLightMapTexture()
    {
        return lightMap.frameBuffer;
    }

    public RenderTexture getLightMapBuffer()
    {
        return lightMap.frameBuffer;
    }
    
}
