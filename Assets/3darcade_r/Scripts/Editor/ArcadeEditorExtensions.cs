﻿/* MIT License

 * Copyright (c) 2020 Skurdt
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:

 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.

 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE. */

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using Unity.Mathematics;
using UnityEditor;
using UnityEditor.Presets;
using UnityEngine;

[assembly: SuppressMessage("Code Quality", "IDE0051:Remove unused private members", Justification = "UnityEditor", Scope = "namespaceanddescendants", Target = "Arcade_r.ArcadeEditorExtensions")]

namespace Arcade_r.ArcadeEditorExtensions
{
    internal enum ModelType
    {
        None,
        Arcade,
        Game,
        Prop
    }

    internal static class GlobalPaths
    {
        internal const string MODELS_FOLDER = "Assets/3darcade/models";
        internal const string ARCADEMODELS_FOLDER = MODELS_FOLDER + "/arcades";
        internal const string GAMEMODELS_FOLDER = MODELS_FOLDER + "/games";
        internal const string PROPMODELS_FOLDER = MODELS_FOLDER + "/props";
        internal const string RESOURCES_FOLDER = "Assets/Resources";
        internal const string ARCADEPREFABS_FOLDER = RESOURCES_FOLDER + "/Arcades";
        internal const string GAMEPREFABS_FOLDER = RESOURCES_FOLDER + "/Games";
        internal const string PROPPREFABS_FOLDER = RESOURCES_FOLDER + "/Props";
    }

    internal sealed class ModelAssetPresetImportPerFolderPreprocessor : AssetPostprocessor
    {
        private void OnPreprocessModel()
        {
            if (assetImporter.importSettingsMissing)
            {
                string assetDirectory = Path.GetDirectoryName(assetPath);
                while (!string.IsNullOrEmpty(assetDirectory))
                {
                    string[] presetGuids = AssetDatabase.FindAssets("t:Preset", new[] { assetDirectory });
                    foreach (string presetGuid in presetGuids)
                    {
                        string presetPath = AssetDatabase.GUIDToAssetPath(presetGuid);
                        if (Path.GetDirectoryName(presetPath) == assetDirectory)
                        {
                            Preset preset = AssetDatabase.LoadAssetAtPath<Preset>(presetPath);
                            if (preset.ApplyTo(assetImporter))
                            {
                                return;
                            }
                        }
                    }
                    assetDirectory = Path.GetDirectoryName(assetDirectory);
                }
            }
        }
    }

    internal sealed class ImportAssistantWindow : EditorWindow
    {
        private static string _savedBrowseDir = string.Empty;
        private static string _externalPath = string.Empty;
        private static ModelType _modelType = ModelType.Game;

        private static bool _closeAfterImport = true;

        [MenuItem("3DArcade_r/Import a new Model...", false, 10003)]
        private static void ShowWindow()
        {
            ImportAssistantWindow window = GetWindow<ImportAssistantWindow>("Import Assistant");
            window.minSize = new Vector2(290f, 120f);
        }

        private void OnGUI()
        {
            GUILayout.Space(8f);
            GUILayout.BeginHorizontal();
            {
                GUILayout.Label("Close after import:", GUILayout.Width(110f));
                _closeAfterImport = GUILayout.Toggle(_closeAfterImport, string.Empty, GUILayout.Height(EditorGUIUtility.singleLineHeight));
            }
            GUILayout.EndHorizontal();

            GUILayout.Space(8f);
            GUILayout.BeginHorizontal();
            {
                GUILayout.Label("Model file:", GUILayout.Width(80f));
                if (GUILayout.Button("Browse", GUILayout.Width(60f), GUILayout.Height(EditorGUIUtility.singleLineHeight)))
                {
                    _externalPath = ShowSelectModelFileDialog();
                }
                _externalPath = GUILayout.TextField(_externalPath);
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            {
                GUILayout.Label("Model type:", GUILayout.Width(80f));
                _modelType = (ModelType)EditorGUILayout.EnumPopup(_modelType);
            }
            GUILayout.EndHorizontal();

            GUILayout.Space(8f);
            if (string.IsNullOrEmpty(_externalPath) || !Path.GetExtension(_externalPath).Equals(".fbx", System.StringComparison.OrdinalIgnoreCase))
            {
                EditorGUILayout.HelpBox("No valid model selected!", MessageType.Error);
            }
            else if (_modelType == ModelType.None)
            {
                EditorGUILayout.HelpBox("Select a model type other than 'None'!", MessageType.Error);
            }
            else if (_externalPath.Contains(Application.dataPath))
            {
                EditorGUILayout.HelpBox("The model must be located outside of the project!", MessageType.Error);
            }
            else
            {
                if (GUILayout.Button("Import", GUILayout.Height(32f)))
                {
                    string assetName = Path.GetFileName(_externalPath);
                    string assetNameNoExt = Path.GetFileNameWithoutExtension(assetName);
                    string destinationFolder;
                    switch (_modelType)
                    {
                        case ModelType.Arcade:
                            destinationFolder = $"{GlobalPaths.ARCADEMODELS_FOLDER}/{assetNameNoExt}";
                            break;
                        case ModelType.Game:
                            destinationFolder = $"{GlobalPaths.GAMEMODELS_FOLDER}/{assetNameNoExt}";
                            break;
                        case ModelType.Prop:
                            destinationFolder = $"{GlobalPaths.PROPMODELS_FOLDER}/{assetNameNoExt}";
                            break;
                        default:
                            return;
                    }

                    if (!Directory.Exists(destinationFolder))
                    {
                        _ = Directory.CreateDirectory(destinationFolder);
                    }

                    string destinationPath = Path.Combine(destinationFolder, assetName);
                    File.Copy(_externalPath, destinationPath, true);
                    AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);

                    GameObject asset = AssetDatabase.LoadAssetAtPath<GameObject>(destinationPath);
                    string assetPath = AssetDatabase.GetAssetPath(asset);
                    ModelImporter modelImporter = AssetImporter.GetAtPath(assetPath) as ModelImporter;
                    if (modelImporter != null)
                    {
                        if (Utils.ExtractTextures(assetPath, modelImporter))
                        {
                            Utils.ExtractMaterials(assetPath);
                        }
                    }
                    else
                    {
                        Debug.LogError("modelImporter is null");
                    }

                    Utils.SaveAsPrefab(asset, _modelType);

                    if (_closeAfterImport)
                    {
                        GetWindow<ImportAssistantWindow>().Close();
                    }
                }
            }
        }

        private string ShowSelectModelFileDialog()
        {
            string filePath = EditorUtility.OpenFilePanel("Select FBX", _savedBrowseDir, "fbx");
            if (!string.IsNullOrEmpty(filePath))
            {
                _savedBrowseDir = Path.GetDirectoryName(filePath);
            }
            return filePath;
        }
    }

    internal sealed class ContextMenus
    {
        // ***************
        // Assets
        // ***************
        [MenuItem("Assets/ArcadeEditorExtensions_r/Set as transparent", false, 1000)]
        private static void AssetsSetAsTransparent()
        {
            Material selectedObj = Selection.activeObject as Material;
            Undo.RecordObject(selectedObj, "Set as transparent");
            Utils.SetupTransparentMaterial(selectedObj);
        }

        [MenuItem("Assets/ArcadeEditorExtensions_r/Set as emissive", false, 1001)]
        private static void AssetsSetAsEmissive()
        {
            Material selectedObj = Selection.activeObject as Material;
            Undo.RecordObject(selectedObj, "Set as emissive");
            Utils.SetupEmissiveMaterial(selectedObj, Color.white);
        }

        // ***************
        // GameObject
        // ***************
        [MenuItem("GameObject/ArcadeEditorExtensions_r/Node/Add BoxCollider (Replace existing)", false, 11)]
        private static void GameObjectAddBoxCollider()
        {
            GameObject selectedObj = Selection.activeObject as GameObject;

            Undo.SetCurrentGroupName("Add/Replace BoxCollider");

            if (selectedObj.TryGetComponent(out Collider collider))
            {
                Undo.DestroyObjectImmediate(collider);
            }
            Utils.AddBoxCollider(selectedObj);

            Undo.CollapseUndoOperations(Undo.GetCurrentGroup());
        }

        [MenuItem("GameObject/ArcadeEditorExtensions_r/Node/Add Non-Kinematic RigidBody (Replace existing)", false, 12)]
        private static void GameObjectAddNonKinematicRigidBody()
        {
            GameObject selectedObj = Selection.activeObject as GameObject;

            Undo.SetCurrentGroupName("Add/Replace Non-Kinematic Rigidbody");

            if (selectedObj.TryGetComponent(out Rigidbody rigidbody))
            {
                Undo.DestroyObjectImmediate(rigidbody);
            }
            Utils.AddRigidBody(selectedObj, false, 80f);

            Undo.CollapseUndoOperations(Undo.GetCurrentGroup());
        }

        [MenuItem("GameObject/ArcadeEditorExtensions_r/Node/Add Kinematic RigidBody (Replace existing)", false, 13)]
        private static void GameObjectAddKinematicRigidBody()
        {
            GameObject selectedObj = Selection.activeObject as GameObject;

            Undo.SetCurrentGroupName("Add/Replace Kinematic Rigidbody");

            if (selectedObj.TryGetComponent(out Rigidbody rigidbody))
            {
                Undo.DestroyObjectImmediate(rigidbody);
            }
            Utils.AddRigidBody(selectedObj, true, 1f);

            Undo.CollapseUndoOperations(Undo.GetCurrentGroup());
        }

        [MenuItem("GameObject/ArcadeEditorExtensions_r/Node/Set as marquee", false, 14)]
        private static void GameObjectSetAsMarquee()
        {
            GameObject selectedObj = Selection.activeObject as GameObject;
            Utils.SetGameObjectAsMarquee(selectedObj);
        }

        [MenuItem("GameObject/ArcadeEditorExtensions_r/Node/Set as monitor", false, 15)]
        private static void GameObjectSetAsMonitor()
        {
            GameObject selectedObj = Selection.activeObject as GameObject;
            Utils.SetGameObjectAsMonitor(selectedObj);
        }

        [MenuItem("GameObject/ArcadeEditorExtensions_r/Node/Set as generic", false, 16)]
        private static void GameObjectSetAsGeneric()
        {
            GameObject selectedObj = Selection.activeObject as GameObject;

            Undo.SetCurrentGroupName("Set as generic");

            Transform parent = selectedObj.transform.parent;
            if (parent != null && parent.childCount >= 3)
            {

                NodeTag nodeTag = selectedObj.GetComponent<NodeTag>();
                if (nodeTag != null)
                {
                    Undo.DestroyObjectImmediate(nodeTag);
                }
                _ = Undo.AddComponent<GenericNodeTag>(selectedObj);

            }

            Undo.CollapseUndoOperations(Undo.GetCurrentGroup());
        }

        [MenuItem("GameObject/ArcadeEditorExtensions_r/Material/Set as transparent", false, 21)]
        private static void GameObjectSetAsTransparent()
        {
            GameObject selectedObj = Selection.activeObject as GameObject;

            MeshRenderer meshRenderer = selectedObj.GetComponent<MeshRenderer>();
            Undo.RecordObject(meshRenderer.sharedMaterial, "Set as transparent");
            Utils.SetupTransparentMaterial(meshRenderer.sharedMaterial);
        }

        [MenuItem("GameObject/ArcadeEditorExtensions_r/Material/Set as emissive", false, 22)]
        private static void GameObjectSetAsEmissive()
        {
            GameObject selectedObj = Selection.activeObject as GameObject;

            MeshRenderer meshRenderer = selectedObj.GetComponent<MeshRenderer>();
            Undo.RecordObject(meshRenderer.sharedMaterial, "Set as emissive");
            Utils.SetupEmissiveMaterial(meshRenderer.sharedMaterial, Color.white);
        }

        // ***************
        // Validation
        // ***************
        [MenuItem("Assets/ArcadeEditorExtensions_r/Set as transparent", true)]
        private static bool AssetsSetAsTransparentValidation()
        {
            Object selectedObj = Selection.activeObject;
            return selectedObj != null
                && selectedObj.GetType() == typeof(Material)
                && !selectedObj.name.Equals("Default-Material");
        }

        [MenuItem("Assets/ArcadeEditorExtensions_r/Set as emissive", true)]
        private static bool AssetsSetAsEmissiveValidation()
        {
            return AssetsSetAsTransparentValidation();
        }

        [MenuItem("GameObject/ArcadeEditorExtensions_r/Node/Add BoxCollider (Replace existing)", true)]
        private static bool GameObjectAddBoxColliderValidation()
        {
            Object selectedObj = Selection.activeObject;
            return selectedObj != null
                && selectedObj.GetType() == typeof(GameObject);
        }

        [MenuItem("GameObject/ArcadeEditorExtensions_r/Node/Add Non-Kinematic RigidBody (Replace existing)", true)]
        private static bool GameObjectAddNonKinematicRigidBodyValidation()
        {
            return GameObjectAddBoxColliderValidation();
        }

        [MenuItem("GameObject/ArcadeEditorExtensions_r/Node/Add Kinematic RigidBody (Replace existing)", true)]
        private static bool GameObjectAddKinematicRigidBodyValidation()
        {
            return GameObjectAddBoxColliderValidation();
        }

        [MenuItem("GameObject/ArcadeEditorExtensions_r/Node/Set as marquee", true)]
        private static bool GameObjectSetAsMarqueeValidation()
        {
            GameObject selectedObj = Selection.activeObject as GameObject;
            return selectedObj != null
                && selectedObj.transform.parent != null
                && selectedObj.TryGetComponent(out MeshRenderer meshRenderer)
                && meshRenderer.sharedMaterials.Length == 1
                && !meshRenderer.sharedMaterial.name.Equals("Default-Material");
        }

        [MenuItem("GameObject/ArcadeEditorExtensions_r/Node/Set as monitor", true)]
        private static bool GameObjectSetAsMonitorValidation()
        {
            return GameObjectSetAsMarqueeValidation();
        }

        [MenuItem("GameObject/ArcadeEditorExtensions_r/Node/Set as generic", true)]
        private static bool GameObjectSetAsGenericValidation()
        {
            return GameObjectSetAsMarqueeValidation();
        }

        [MenuItem("GameObject/ArcadeEditorExtensions_r/Material/Set as transparent", true)]
        private static bool GameObjectSetAsTransparentValidation()
        {
            return GameObjectSetAsMarqueeValidation();
        }

        [MenuItem("GameObject/ArcadeEditorExtensions_r/Material/Set as emissive", true)]
        private static bool GameObjectSetAsEmissiveValidation()
        {
            return GameObjectSetAsMarqueeValidation();
        }
    }

    internal static class Utils
    {
        private static readonly Color MARQUEE_EMISSIVE_COLOR = Color.white;
        private static readonly Color MONITOR_EMISSIVE_COLOR = Color.white;

        internal static ModelType GetModelType(string assetPath)
        {
            ModelType result = ModelType.None;
            if (assetPath.StartsWith(GlobalPaths.ARCADEMODELS_FOLDER, System.StringComparison.OrdinalIgnoreCase))
            {
                result = ModelType.Arcade;
            }
            else if (assetPath.StartsWith(GlobalPaths.GAMEMODELS_FOLDER, System.StringComparison.OrdinalIgnoreCase))
            {
                result = ModelType.Game;
            }
            else if (assetPath.StartsWith(GlobalPaths.PROPMODELS_FOLDER, System.StringComparison.OrdinalIgnoreCase))
            {
                result = ModelType.Prop;
            }
            return result;
        }

        internal static bool ExtractTextures(string assetPath, ModelImporter modelImporter)
        {
            bool result = false;

            string modelPath = Path.GetDirectoryName(assetPath);
            string texturesDirectory = Path.Combine(modelPath, "textures");

            if (modelImporter.ExtractTextures(texturesDirectory))
            {
                AssetDatabase.Refresh(ImportAssetOptions.Default);
                result = true;
                Debug.Log("Extracted textures");
            }
            else
            {
                Debug.LogError("Failed to extract textures");
            }

            return result;
        }

        internal static void ExtractMaterials(string assetPath)
        {
            string modelPath          = Path.GetDirectoryName(assetPath);
            string materialsDirectory = Path.Combine(modelPath, "materials");
            _ = Directory.CreateDirectory(materialsDirectory);

            HashSet<string> assetsToReload = new HashSet<string>();
            IEnumerable<Object> materials  = AssetDatabase.LoadAllAssetsAtPath(assetPath).Where(x => x.GetType() == typeof(Material));
            foreach (Object material in materials)
            {
                Material mat = material as Material;
                if (mat != null && mat.mainTexture != null)
                {
                    mat.color = Color.white;
                }
                string newAssetPath = Path.Combine(materialsDirectory, $"{material.name}.mat");
                newAssetPath = AssetDatabase.GenerateUniqueAssetPath(newAssetPath);
                string error = AssetDatabase.ExtractAsset(material, newAssetPath);
                if (string.IsNullOrEmpty(error))
                {
                    _ = assetsToReload.Add(assetPath);
                }
            }

            if (assetsToReload.Count > 0)
            {
                foreach (string path in assetsToReload)
                {
                    _ = AssetDatabase.WriteImportSettingsIfDirty(path);
                    AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
                }
                Debug.Log("Extracted materials");
            }
            else
            {
                Debug.LogWarning("Failed to extract materials (Already extracted?)");
            }
        }

        internal static void SaveAsPrefab(GameObject obj, ModelType modelType)
        {
            GameObject tempObj   = Object.Instantiate(obj);
            string prefabsFolder = string.Empty;
            switch (modelType)
            {
                case ModelType.Arcade:
                {
                    prefabsFolder = GlobalPaths.ARCADEPREFABS_FOLDER;
                    AddRigidBody(tempObj, true, 1f);
                }
                break;
                case ModelType.Game:
                {
                    prefabsFolder = GlobalPaths.GAMEPREFABS_FOLDER;
                    AddBoxCollider(tempObj);
                    AddRigidBody(tempObj, false, 80f);
                }
                break;
                case ModelType.Prop:
                {
                    prefabsFolder = GlobalPaths.PROPPREFABS_FOLDER;
                    AddBoxCollider(tempObj);
                    AddRigidBody(tempObj, false, 80f);
                }
                break;
                case ModelType.None:
                default:
                    break;
            }

            if (!Directory.Exists(prefabsFolder))
            {
                _ = Directory.CreateDirectory(prefabsFolder);
            }

            RenameNodes(tempObj.transform, modelType);

            string destinationPath = Path.Combine(prefabsFolder, $"{obj.name}.prefab");
            if (File.Exists(destinationPath))
            {
                File.Delete(destinationPath);
                File.Delete($"{destinationPath}.meta");
            }

            GameObject newObj = PrefabUtility.SaveAsPrefabAsset(tempObj, destinationPath, out bool success);
            if (success)
            {
                Debug.Log($"{modelType} prefab '{obj.name}' created");
                Selection.activeGameObject = newObj;
                EditorGUIUtility.PingObject(newObj);
                _ = AssetDatabase.OpenAsset(newObj);
            }
            else
            {
                Debug.LogError($"{modelType} prefab '{obj.name}' creation failed");
            }

            Object.DestroyImmediate(tempObj);
        }

        internal static void AddBoxCollider(GameObject obj)
        {
            if (obj.GetComponent<Collider>() != null || obj.GetComponentInChildren<Collider>() != null)
            {
                return;
            }

            BoxCollider boxCollider = Undo.AddComponent<BoxCollider>(obj);

            Renderer[] renderers = obj.GetComponentsInChildren<Renderer>();
            if (renderers.Length > 0)
            {
                Bounds bounds = new Bounds(renderers[0].bounds.center, renderers[0].bounds.size);
                foreach (Renderer renderer in renderers)
                {
                    bounds.Encapsulate(renderer.bounds);
                }
                boxCollider.center = boxCollider.transform.InverseTransformPoint(bounds.center);
                Vector3 size       = boxCollider.transform.InverseTransformVector(bounds.size);
                boxCollider.size   = new Vector3(math.abs(size.x), math.abs(size.y), math.abs(size.z));
            }
        }

        internal static void AddRigidBody(GameObject obj, bool kinematic, float mass)
        {
            if (obj.GetComponent<Rigidbody>() != null)
            {
                return;
            }

            Rigidbody rigidbody   = Undo.AddComponent<Rigidbody>(obj);
            rigidbody.useGravity  = !kinematic;
            rigidbody.isKinematic = kinematic;
            rigidbody.mass        = mass;
        }

        internal static void SetGameObjectAsMarquee(GameObject obj)
        {
            Undo.SetCurrentGroupName("Set as marquee");

            Transform parent = obj.transform.parent;
            if (parent != null && parent.childCount >= 1)
            {
                Undo.RegisterFullObjectHierarchyUndo(parent, "Set as marquee");
                obj.transform.SetAsFirstSibling();
                NodeTag nodeTag = obj.GetComponent<NodeTag>();
                if (nodeTag != null)
                {
                    Undo.DestroyObjectImmediate(nodeTag);
                }
                _ = Undo.AddComponent<MarqueeNodeTag>(obj);
            }

            MeshRenderer meshRenderer = obj.GetComponent<MeshRenderer>();
            Undo.RecordObject(meshRenderer.sharedMaterial, "Set as marquee");
            SetupEmissiveMaterial(meshRenderer.sharedMaterial, MARQUEE_EMISSIVE_COLOR);

            Undo.CollapseUndoOperations(Undo.GetCurrentGroup());
        }

        internal static void SetGameObjectAsMonitor(GameObject obj)
        {
            Undo.SetCurrentGroupName("Set as monitor");

            Transform parent = obj.transform.parent;
            if (parent != null && parent.childCount >= 2)
            {
                Undo.RegisterFullObjectHierarchyUndo(parent, "Set as monitor");
                obj.transform.SetSiblingIndex(1);
                NodeTag nodeTag = obj.GetComponent<NodeTag>();
                if (nodeTag != null)
                {
                    Undo.DestroyObjectImmediate(nodeTag);
                }
                _ = Undo.AddComponent<ScreenNodeTag>(obj);
            }

            MeshRenderer meshRenderer = obj.GetComponent<MeshRenderer>();
            Undo.RecordObject(meshRenderer.sharedMaterial, "Set as monitor");
            SetupEmissiveMaterial(meshRenderer.sharedMaterial, MONITOR_EMISSIVE_COLOR);

            Undo.CollapseUndoOperations(Undo.GetCurrentGroup());
        }

        internal static void SetupEmissiveMaterial(Material material, Color color)
        {
            material.EnableKeyword("_EMISSION");
            material.globalIlluminationFlags = MaterialGlobalIlluminationFlags.RealtimeEmissive;
            Texture mainTex = material.GetTexture("_MainTex");
            if (mainTex != null)
            {
                material.SetTexture("_EmissionMap", material.GetTexture("_MainTex"));
            }
            material.SetColor("_EmissionColor", color);
        }

        internal static void SetupTransparentMaterial(Material material)
        {
            material.EnableKeyword("_ALPHAPREMULTIPLY_ON");
            material.renderQueue = 3000;
            material.SetOverrideTag("RenderType", "Transparent");
            material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            material.SetInt("_Mode", 3);
            material.SetInt("_ZWrite", 0);
        }

        private static void RenameNodes(Transform transform, ModelType modelType)
        {
            List<Transform> children = GetAllChildren(transform, modelType == ModelType.Arcade);
            if (children.Count > 0)
            {
                string meshNumberFormat = children.Count > 100 ? "{0:000}" : "{0:00}";
                for (int i = 0; i < children.Count; ++i)
                {
                    Transform child = children[i];
                    if (!child.name.StartsWith(transform.name))
                    {
                        child.gameObject.StripCloneFromName();
                        child.name += $"_Mesh{string.Format(meshNumberFormat, i)}";
                    }
                }
            }
        }

        private static List<Transform> GetAllChildren(Transform parentTansform, bool getNestedNodes)
        {
            List<Transform> result = new List<Transform>();
            foreach (Transform childTransform in parentTansform)
            {
                result.Add(childTransform);
                if (getNestedNodes)
                {
                    result.AddRange(GetAllChildren(childTransform, getNestedNodes));
                }
            }
            return result;
        }
    }
}
