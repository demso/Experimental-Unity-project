
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using Visual.Lighting;

// [CustomEditor(typeof(PointLight))]
class DecalMeshHelperEditor : Editor {
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        if (GUILayout.Button("Print Render Texture"))
        {
            //SaveTextureToFileUtility.SaveRenderTextureToFile(GameObject.Find("Light").GetComponent<PointLight>().renderTexture, "C:\\Users\\Дмитрий\\Desktop\\RenderTExture\\LightTexture.png" );
        }
            
    }
}
