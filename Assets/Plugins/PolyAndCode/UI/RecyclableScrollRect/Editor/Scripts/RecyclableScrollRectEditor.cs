//MIT License
//Copyright (c) 2020 Mohammed Iqubal Hussain
//Website : Polyandcode.com

using UnityEngine.UI;
using UnityEditor.AnimatedValues;
using UnityEditor;
using UnityEngine;

namespace PolyAndCode.UI.UnityEditor
{
    [CustomEditor(typeof(RecyclableScrollRect), true), CanEditMultipleObjects]
    /// <summary>
    /// Custom Editor for the Recyclable Scroll Rect Component which is derived from Scroll Rect.
    /// </summary>
    public sealed class RecyclableScrollRectEditor : Editor
    {
        private RecyclableScrollRect _script;

        //Inherited
        private SerializedProperty _content;
        private SerializedProperty _movementType;
        private SerializedProperty _elasticity;
        private SerializedProperty _inertia;
        private SerializedProperty _decelerationRate;
        private SerializedProperty _scrollSensitivity;
        private SerializedProperty _viewport;
        private SerializedProperty _onValueChanged;

        private SerializedProperty _protoTypeCell;
        private SerializedProperty _selfInitialize;
        private SerializedProperty _direction;
        private SerializedProperty _type;

        private AnimBool _showElasticity;
        private AnimBool _showDecelerationRate;

        private void OnEnable()
        {
            _script = target as RecyclableScrollRect;

            //Inherited
            _content           = serializedObject.FindProperty("m_Content");
            _movementType      = serializedObject.FindProperty("m_MovementType");
            _elasticity        = serializedObject.FindProperty("m_Elasticity");
            _inertia           = serializedObject.FindProperty("m_Inertia");
            _decelerationRate  = serializedObject.FindProperty("m_DecelerationRate");
            _scrollSensitivity = serializedObject.FindProperty("m_ScrollSensitivity");
            _viewport          = serializedObject.FindProperty("m_Viewport");
            _onValueChanged    = serializedObject.FindProperty("m_OnValueChanged");

            _protoTypeCell  = serializedObject.FindProperty(nameof(RecyclableScrollRect.PrototypeCell));
            _selfInitialize = serializedObject.FindProperty(nameof(RecyclableScrollRect.SelfInitialize));
            _direction      = serializedObject.FindProperty(nameof(RecyclableScrollRect.Direction));
            _type           = serializedObject.FindProperty(nameof(RecyclableScrollRect.IsGrid));

            _showElasticity       = new AnimBool(Repaint);
            _showDecelerationRate = new AnimBool(Repaint);
            SetAnimBools(true);
        }

        private void OnDisable()
        {
            _showElasticity.valueChanged.RemoveListener(Repaint);
            _showDecelerationRate.valueChanged.RemoveListener(Repaint);
        }

        private void SetAnimBools(bool instant)
        {
            SetAnimBool(_showElasticity, !_movementType.hasMultipleDifferentValues && _movementType.enumValueIndex == (int)ScrollRect.MovementType.Elastic, instant);
            SetAnimBool(_showDecelerationRate, !_inertia.hasMultipleDifferentValues && _inertia.boolValue, instant);
        }

        private void SetAnimBool(AnimBool a, bool value, bool instant)
        {
            if (instant)
                a.value = value;
            else
                a.target = value;
        }

        public override void OnInspectorGUI()
        {
            SetAnimBools(false);
            serializedObject.Update();

            _ = EditorGUILayout.PropertyField(_direction);
            _ = EditorGUILayout.PropertyField(_type, new GUIContent("Grid"));
            if (_type.boolValue)
            {
                string title = _direction.enumValueIndex == (int)RecyclableScrollRect.DirectionType.Vertical ? "Coloumns" : "Rows";
               _script.Segments =  EditorGUILayout.IntField(title, _script.Segments);
            }

            _ = EditorGUILayout.PropertyField(_selfInitialize);
            _ = EditorGUILayout.PropertyField(_viewport);
            _ = EditorGUILayout.PropertyField(_content);
            _ = EditorGUILayout.PropertyField(_protoTypeCell);
            EditorGUILayout.Space();

            _ = EditorGUILayout.PropertyField(_movementType);
            if (EditorGUILayout.BeginFadeGroup(_showElasticity.faded))
            {
                EditorGUI.indentLevel++;
                _ = EditorGUILayout.PropertyField(_elasticity);
                EditorGUI.indentLevel--;
            }
            EditorGUILayout.EndFadeGroup();

            _ = EditorGUILayout.PropertyField(_inertia);
            if (EditorGUILayout.BeginFadeGroup(_showDecelerationRate.faded))
            {
                EditorGUI.indentLevel++;
                _ = EditorGUILayout.PropertyField(_decelerationRate);
                EditorGUI.indentLevel--;
            }
            EditorGUILayout.EndFadeGroup();

            _ = EditorGUILayout.PropertyField(_scrollSensitivity);

            EditorGUILayout.Space();

            _ = EditorGUILayout.PropertyField(_onValueChanged);

            _ = serializedObject.ApplyModifiedProperties();
        }
    }
}
