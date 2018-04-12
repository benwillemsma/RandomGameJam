using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

[CustomEditor(typeof(EffectManager))]
public class EffectManagerEditor : Editor
{
    private enum types
    {
        Damage,
        Knockback
    };

    private EffectManager m_target;
    private SerializedProperty effectsProp;
    private int type;

    private void OnEnable()
    {
        m_target = (EffectManager)target;
        effectsProp = serializedObject.FindProperty("effects");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        {
            DrawDefaultInspector();

            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Effects", EditorStyles.boldLabel);
            for (int i = 0; i < m_target.effects.Count; i++)
            {
                if (m_target.effects[i])
                {
                    EditorGUILayout.Space();
                    EditorGUILayout.PropertyField(effectsProp.GetArrayElementAtIndex(i), GUIContent.none);
                    EditorGUI.indentLevel = 1;
                    ShowEffectValues(m_target.effects[i], "" + m_target.effects[i].GetType());
                    EditorGUI.indentLevel = 0;
                    EditorGUILayout.Space();
                    EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
                }
            }
            EditorGUILayout.Space();

            type = EditorGUILayout.Popup("EffectType", type, Enum.GetNames(typeof(types)));
            EditorGUILayout.BeginHorizontal();
            {
                if (GUILayout.Button("Add Effect"))
                    AddEffect();

                if (GUILayout.Button("Remove Effect"))
                    RemoveEffect();
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();
        }
        serializedObject.ApplyModifiedProperties();
    }

    private void ShowEffectValues(Effect effect, string type)
    {
        type = type.Replace("Effect", "");
        effect.initial_value = EditorGUILayout.FloatField("Initial_" + type, effect.initial_value);
        effect.final_value = EditorGUILayout.FloatField("Final_" + type, effect.final_value);
        effect.overTime_value = EditorGUILayout.FloatField("OverTime_" + type, effect.overTime_value);
    }

    private int CheckForType<T>() where T: Effect
    {
        for (int i = 0; i < m_target.effects.Count; i++)
        {
            if (m_target.effects[i] && m_target.effects[i].GetType() == typeof(T))
                return i;
        }
        return -1;
    }

    private void AddEffect()
    {
        switch ((types)type)
        {
            case types.Damage:
                if (CheckForType<DamageEffect>() == -1)
                    m_target.effects.Add(m_target.gameObject.AddComponent<DamageEffect>());
                break;
            case types.Knockback:
                if (CheckForType<KnockBackEffect>() == -1)
                    m_target.effects.Add(m_target.gameObject.AddComponent<KnockBackEffect>());
                break;
            default:
                break;
        }
    }

    private void RemoveEffect()
    {
        int index = -1;
        switch ((types)type)
        {
            case types.Damage:
                index = CheckForType<DamageEffect>();
                break;
            case types.Knockback:
                index = CheckForType<KnockBackEffect>();
                break;
            default:
                break;
        }
        Debug.Log(m_target.effects.Count);
        if (index >= 0 && m_target.effects.Count > 0)
        {
            DestroyImmediate(m_target.effects[index]);
            m_target.effects.RemoveAt(index);
        }
    }
}
