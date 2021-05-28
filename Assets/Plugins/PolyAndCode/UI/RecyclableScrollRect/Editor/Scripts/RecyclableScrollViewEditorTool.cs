using UnityEditor;
using UnityEngine;

namespace PolyAndCode.UI.UnityEditor
{
    [ExecuteInEditMode]
    public static class RecyclableScrollViewEditorTool
    {
        [MenuItem("GameObject/UI/Recyclable Scroll View")]
        private static void CreateRecyclableScrollView()
        {
            GameObject selected = Selection.activeGameObject;

            GameObject asset = Resources.Load<GameObject>("PolyAndCode_RecyclableScrollView");

            GameObject item = Object.Instantiate(asset, selected.transform);
            item.name = "Recyclable Scroll View";
            item.transform.localPosition = Vector3.zero;
            Undo.RegisterCreatedObjectUndo(item, "Create Recycalable Scroll view");

            Selection.activeGameObject = item;
        }

        [MenuItem("GameObject/UI/Recyclable Scroll View", true)]
        private static bool CreateRecyclableScrollViewValidation()
        {
            GameObject selected = Selection.activeGameObject;
            return selected != null && selected.transform is RectTransform;
        }
    }
}
