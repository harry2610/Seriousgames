using System;
using UnityEngine;
using System.Security.Cryptography;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Build;
#endif

public class ScriptableObjectIdAttribute : PropertyAttribute
{
}

#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(ScriptableObjectIdAttribute))]
public class ScriptableObjectIdDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        if (property.ulongValue == 0)
        {
            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
            var data = new byte[8];
            rng.GetBytes(data);
            property.longValue = BitConverter.ToInt64(data);
        }
        var propertyRect = new Rect(position);
        propertyRect.xMax -= 100;
        var buttonRect = new Rect(position);
        buttonRect.xMin = position.xMax - 100;
        EditorGUI.PropertyField(propertyRect, property, label, true);
        if (GUI.Button(buttonRect, "Regenerate ID"))
        {
            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
            var data = new byte[8];
            rng.GetBytes(data);
            property.longValue = BitConverter.ToInt64(data);
        }
    }
}
#endif