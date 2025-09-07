using UnityEngine;
using UnityEditor;
using System.Linq;

public class VRMMaterialConverter : EditorWindow
{
    [MenuItem("Tools/VRM Material Converter")]
    public static void ShowWindow()
    {
        GetWindow<VRMMaterialConverter>("VRM Material Converter");
    }

    void OnGUI()
    {
        GUILayout.Label("VRM Material to HDRP Converter", EditorStyles.boldLabel);
        
        if (GUILayout.Button("Convert All VRM Materials to HDRP/Lit"))
        {
            ConvertVRMMaterialsToHDRP();
        }
        
        if (GUILayout.Button("Convert Selected GameObject Materials"))
        {
            ConvertSelectedObjectMaterials();
        }
        
        if (GUILayout.Button("Update VRM Prefabs in Project"))
        {
            UpdateVRMPrefabsInProject();
        }
        
        if (GUILayout.Button("Revert All Materials to MToon"))
        {
            RevertMaterialsToMToon();
        }
        
        EditorGUILayout.Space();
        EditorGUILayout.HelpBox("This tool will convert VRM10/MToon materials to HDRP/Lit materials automatically.", MessageType.Info);
    }

    void ConvertVRMMaterialsToHDRP()
    {
        // プロジェクト内の全マテリアルを検索
        string[] materialGuids = AssetDatabase.FindAssets("t:Material");
        int convertedCount = 0;
        
        foreach (string guid in materialGuids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            Material material = AssetDatabase.LoadAssetAtPath<Material>(path);
            
            if (material != null && IsVRMMaterial(material))
            {
                ConvertMaterialToHDRP(material);
                convertedCount++;
            }
        }
        
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        
        Debug.Log($"Converted {convertedCount} VRM materials to HDRP/Lit");
        EditorUtility.DisplayDialog("Conversion Complete", $"Successfully converted {convertedCount} materials to HDRP/Lit", "OK");
    }
    
    void ConvertSelectedObjectMaterials()
    {
        GameObject selectedObject = Selection.activeGameObject;
        if (selectedObject == null)
        {
            EditorUtility.DisplayDialog("No Selection", "Please select a GameObject with materials to convert.", "OK");
            return;
        }
        
        Renderer[] renderers = selectedObject.GetComponentsInChildren<Renderer>();
        int convertedCount = 0;
        
        foreach (Renderer renderer in renderers)
        {
            foreach (Material material in renderer.sharedMaterials)
            {
                if (material != null && IsVRMMaterial(material))
                {
                    ConvertMaterialToHDRP(material);
                    convertedCount++;
                }
            }
        }
        
        // プレハブの場合は変更を適用 - Fixed method name
        GameObject prefabRoot = PrefabUtility.GetOutermostPrefabInstanceRoot(selectedObject);
        if (prefabRoot != null)
        {
            // Unity 2018.3以降の新しいプレハブシステムに対応
            string prefabPath = PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(selectedObject);
            if (!string.IsNullOrEmpty(prefabPath))
            {
                PrefabUtility.ApplyPrefabInstance(prefabRoot, InteractionMode.AutomatedAction);
            }
        }
        
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        
        Debug.Log($"Converted {convertedCount} materials on selected object");
        EditorUtility.DisplayDialog("Conversion Complete", $"Successfully converted {convertedCount} materials to HDRP/Lit", "OK");
    }
    
    bool IsVRMMaterial(Material material)
    {
        if (material.shader == null) return false;
        
        string shaderName = material.shader.name;
        return shaderName.Contains("VRM10") || 
               shaderName.Contains("MToon") || 
               shaderName.Contains("VRM/") ||
               shaderName == "Hidden/InternalErrorShader";
    }
    
    void ConvertMaterialToHDRP(Material material)
    {
        // HDRPのLitシェーダーを取得
        Shader hdrpLitShader = Shader.Find("HDRP/Lit");
        if (hdrpLitShader == null)
        {
            Debug.LogError("HDRP/Lit shader not found. Make sure HDRP is properly installed.");
            return;
        }
        
        // 元のプロパティを保存
        Color baseColor = Color.white;
        Texture mainTexture = null;
        Texture normalMap = null;
        float metallic = 0f;
        float smoothness = 0.5f;
        
        // VRM/MToonからプロパティを取得
        if (material.HasProperty("_Color"))
            baseColor = material.GetColor("_Color");
        else if (material.HasProperty("_BaseColor"))
            baseColor = material.GetColor("_BaseColor");
            
        if (material.HasProperty("_MainTex"))
            mainTexture = material.GetTexture("_MainTex");
        else if (material.HasProperty("_BaseColorTexture"))
            mainTexture = material.GetTexture("_BaseColorTexture");
            
        if (material.HasProperty("_BumpMap"))
            normalMap = material.GetTexture("_BumpMap");
        else if (material.HasProperty("_NormalTexture"))
            normalMap = material.GetTexture("_NormalTexture");
            
        if (material.HasProperty("_Metallic"))
            metallic = material.GetFloat("_Metallic");
            
        if (material.HasProperty("_Glossiness"))
            smoothness = material.GetFloat("_Glossiness");
        else if (material.HasProperty("_Smoothness"))
            smoothness = material.GetFloat("_Smoothness");
        
        // シェーダーを変更
        material.shader = hdrpLitShader;
        
        // HDRPプロパティに設定
        if (material.HasProperty("_BaseColor"))
            material.SetColor("_BaseColor", baseColor);
            
        if (material.HasProperty("_BaseColorMap") && mainTexture != null)
            material.SetTexture("_BaseColorMap", mainTexture);
            
        if (material.HasProperty("_NormalMap") && normalMap != null)
        {
            material.SetTexture("_NormalMap", normalMap);
            material.SetFloat("_NormalScale", 1.0f);
        }
        
        if (material.HasProperty("_Metallic"))
            material.SetFloat("_Metallic", metallic);
            
        if (material.HasProperty("_Smoothness"))
            material.SetFloat("_Smoothness", smoothness);
        
        // マテリアルをDirtyとしてマーク
        EditorUtility.SetDirty(material);
        
        Debug.Log($"Converted material: {material.name} to HDRP/Lit");
    }
    
    void RevertMaterialsToMToon()
    {
        // MToonシェーダーを取得
        Shader mtoonShader = Shader.Find("VRM10/MToon10");
        if (mtoonShader == null)
        {
            mtoonShader = Shader.Find("VRM/MToon");
        }
        
        if (mtoonShader == null)
        {
            EditorUtility.DisplayDialog("MToon Shader Not Found", "MToon shader not found. Make sure UniVRM is properly installed.", "OK");
            return;
        }
        
        string[] materialGuids = AssetDatabase.FindAssets("t:Material");
        int revertedCount = 0;
        
        foreach (string guid in materialGuids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            Material material = AssetDatabase.LoadAssetAtPath<Material>(path);
            
            if (material != null && material.shader != null && 
                material.shader.name.Contains("HDRP/Lit"))
            {
                material.shader = mtoonShader;
                EditorUtility.SetDirty(material);
                revertedCount++;
            }
        }
        
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        
        Debug.Log($"Reverted {revertedCount} materials to MToon");
        EditorUtility.DisplayDialog("Revert Complete", $"Successfully reverted {revertedCount} materials to MToon", "OK");
    }
    
    void UpdateVRMPrefabsInProject()
    {
        // プロジェクト内の全プレハブを検索
        string[] prefabGuids = AssetDatabase.FindAssets("t:Prefab");
        int updatedPrefabs = 0;
        
        foreach (string guid in prefabGuids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            
            if (prefab != null && IsVRMPrefab(prefab))
            {
                // プレハブのコンテンツを更新 - Unity 2018.3以降の新しいプレハブシステムに対応
                using (var editingScope = new PrefabUtility.EditPrefabContentsScope(path))
                {
                    Renderer[] renderers = editingScope.prefabContentsRoot.GetComponentsInChildren<Renderer>();
                    bool prefabUpdated = false;
                    
                    foreach (Renderer renderer in renderers)
                    {
                        Material[] materials = renderer.sharedMaterials;
                        for (int i = 0; i < materials.Length; i++)
                        {
                            if (materials[i] != null && IsVRMMaterial(materials[i]))
                            {
                                ConvertMaterialToHDRP(materials[i]);
                                prefabUpdated = true;
                            }
                        }
                    }
                    
                    if (prefabUpdated)
                    {
                        updatedPrefabs++;
                        EditorUtility.SetDirty(editingScope.prefabContentsRoot);
                    }
                }
            }
        }
        
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        
        Debug.Log($"Updated {updatedPrefabs} VRM prefabs in project");
        EditorUtility.DisplayDialog("Update Complete", $"Successfully updated {updatedPrefabs} VRM prefabs", "OK");
    }
    
    bool IsVRMPrefab(GameObject prefab)
    {
        // VRMコンポーネントまたは特定の構造を持つかチェック
        return prefab.GetComponent<Animator>() != null && 
               prefab.GetComponentsInChildren<SkinnedMeshRenderer>().Length > 0;
    }
}