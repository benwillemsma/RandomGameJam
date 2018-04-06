using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

[CustomPropertyDrawer(typeof(Condition), true)]
[CanEditMultipleObjects]
public class ConditionDrawer : PropertyDrawer
{
    public override void OnGUI(Rect pos, SerializedProperty prop, GUIContent label)
    {
        EditorGUI.BeginProperty(pos, label, prop);
        EditorGUI.BeginChangeCheck();
        GUILayout.BeginHorizontal();
        {
            try
            {
                UnityEngine.Object condition = prop.objectReferenceValue;
                if (condition as TimedCondition)
                    DrawCondition(pos, condition as TimedCondition, label);

                else if (condition as AmountCondition)
                    DrawCondition(pos, condition as AmountCondition, label);

                else if (condition as AreaCondition)
                    DrawCondition(pos, condition as AreaCondition, label);

                else if (condition as DestroyedCondition)
                    DrawCondition(pos, condition as DestroyedCondition, label);

                else if (condition as InputCondition)
                    DrawCondition(pos, condition as InputCondition, label);

                else if (condition as TriggerCondition)
                    DrawCondition(pos, condition as TriggerCondition, label);

                else if (condition)
                    DrawDefault(pos, condition.GetType());
            }
            catch (InvalidCastException e)
            {
                Debug.Log("property was not a Condition");
                Debug.LogException(e);
            }
        }
        GUILayout.EndHorizontal();

        if (EditorGUI.EndChangeCheck())
        {
            Undo.RegisterCompleteObjectUndo(prop.objectReferenceValue, "Condition Change");
            EditorUtility.SetDirty(prop.objectReferenceValue);
        }
        EditorGUI.EndProperty();
    }

    #region Helpers
    private class InLine
    {
        static public Rect position;
        public static float LineHeight = 20;

        static public int line = 0;
        static public float indent = 0;
        static public float items = 1;
        static public int currentItem = 0;

        public static Rect NextRect(bool toggleButton = false)
        {
            Rect temp = new Rect(position.x + indent + (currentItem * (position.width - indent) / items), position.y + 5 + (LineHeight * line), (position.width - indent) / items, 15);
            if (toggleButton)
                temp.width = 10;
            currentItem++;
            return temp;
        }
        public static void Reset()
        {
            position = new Rect();
            line = 0;
            indent = 0;
            items = 1;
            currentItem = 0;
        }

        public static void SetRect(Rect newPosition, bool toggleButton = false)
        {
            position = newPosition;
            currentItem = 0;
        }
        public static void SetRect(Rect newPosition, int newLine, bool toggleButton = false)
        {
            position = newPosition;
            line = newLine;
            currentItem = 0;
        }
        public static void SetRect(Rect newPosition, int newLine, float newIndent, bool toggleButton = false)
        {
            position = newPosition;
            line = newLine;
            currentItem = 0;
            indent = newIndent;
        }
        public static void SetRect(Rect newPosition, int newLine, float newIndent, float itemAmount, bool toggleButton = false)
        {
            position = newPosition;
            line = newLine;
            indent = newIndent;
            items = itemAmount;
            currentItem = 0;
        }

        public static Rect GetRect(Rect newPosition, bool toggleButton = false)
        {
            SetRect(newPosition, toggleButton);
            return NextRect();
        }
        public static Rect GetRect(Rect newPosition, int newLine, bool toggleButton = false)
        {
            SetRect(newPosition, newLine, toggleButton);
            return NextRect();
        }
        public static Rect GetRect(Rect newPosition, int newLine, float newIndent, bool toggleButton = false)
        {
            SetRect(newPosition, newLine, newIndent, toggleButton);
            return NextRect();
        }
        public static Rect GetRect(Rect newPosition, int newLine, float newIndent, float itemAmount, bool toggleButton = false)
        {
            SetRect(newPosition, newLine, newIndent, itemAmount, toggleButton);
            return NextRect();
        }

        public static void SetLine(int newLine, bool toggleButton = false)
        {
            line = newLine;
            currentItem = 0;
        }
        public static void SetLine(int newLine, float newIndent, bool toggleButton = false)
        {
            line = newLine;
            indent = newIndent;
            currentItem = 0;
        }
        public static void SetLine(int newLine, float newIndent, float itemAmount, bool toggleButton = false)
        {
            line = newLine;
            indent = newIndent;
            items = itemAmount;
            currentItem = 0;
        }

        public static Rect GetLine(int newLine, bool toggleButton = false)
        {
            SetLine(newLine, toggleButton);
            return NextRect();
        }
        public static Rect GetLine(int newLine, float newIndent, bool toggleButton = false)
        {
            SetLine(newLine, newIndent, toggleButton);
            return NextRect();
        }
        public static Rect GetLine(int newLine, float newIndent, float itemAmount, bool toggleButton = false)
        {
            SetLine(newLine, newIndent, itemAmount, toggleButton);
            return NextRect();
        }
    }
    private string[] FindLayerNames()
    {
        List<string> layerNames = new List<string>();
        for (int i = 8; i <= 31; i++)
        {
            string layerN = LayerMask.LayerToName(i);
            if (layerN.Length > 0)
                layerNames.Add(layerN);
        }
        return layerNames.ToArray();
    }
    #endregion

    #region Draw Functions
    protected virtual void DrawDefault(Rect pos, Type type)
    {
        EditorGUI.LabelField(pos, type.ToString());
        EditorGUILayout.HelpBox("No Draw function For condition Type: " + type + ", has been Declared", MessageType.Warning);
    }

    protected void DrawCondition(Rect pos, TimedCondition condition, GUIContent label)
    {
        EditorGUI.LabelField(InLine.GetRect(pos, 0, 0, 2), "Timer Amount");
        condition.timerAmount = EditorGUI.FloatField(InLine.GetRect(pos, 0, 100, 1), condition.timerAmount);
        
        InLine.SetRect(pos, 1, 0, 6);
        {
            EditorGUI.LabelField(InLine.NextRect(), "Loop");
            condition.loop = EditorGUI.Toggle(InLine.NextRect(true), condition.loop);
        }
        InLine.Reset();
    }

    protected void DrawCondition(Rect pos, AreaCondition condition, GUIContent label)
    {
        InLine.SetRect(pos, 0, 0, 6);
        {
            EditorGUI.LabelField(InLine.NextRect(), "Area");
        }
        condition.triggerArea = EditorGUI.BoundsField(InLine.GetLine(0, pos.width / 2, 1), condition.triggerArea);

        InLine.SetRect(pos, 0, 50, 6);
        {
            EditorGUI.LabelField(InLine.NextRect(), "UsePlayer");
            condition.UsePlayer = EditorGUI.Toggle(InLine.NextRect(true), condition.UsePlayer);
        }
        if (!condition.UsePlayer )
        {
            condition.checkObject = (GameObject)EditorGUI.ObjectField(InLine.GetLine(1, 0, 2), condition.checkObject, typeof(GameObject), true);
            if(!condition.checkObject)
                EditorGUILayout.HelpBox("CheckObject is null for AreaCondition", MessageType.Warning);
        }
        InLine.Reset();
    }

    protected void DrawCondition(Rect pos, DestroyedCondition condition, GUIContent label)
    {
        EditorGUI.LabelField(InLine.GetRect(pos, 0, 0, 2), "Object");
        condition.checkObject = EditorGUI.ObjectField(InLine.GetRect(pos, 0, 100, 1), condition.checkObject, typeof(UnityEngine.Object), true);

        InLine.SetRect(pos, 1, 0, 6);
        {
            EditorGUI.LabelField(InLine.NextRect(), "IsAlive");
            condition.isAlive = EditorGUI.Toggle(InLine.NextRect(true), condition.isAlive);
        }
        InLine.Reset();
    }

    protected void DrawCondition(Rect pos, AmountCondition condition, GUIContent label)
    {
        InLine.SetRect(pos, 0, 0, 6); 
        {
            condition.typeOfFind = (FindType)EditorGUI.EnumPopup(InLine.NextRect(), condition.typeOfFind);
            switch (condition.typeOfFind)
            {
                case FindType.Tag:
                    condition.checkTag = EditorGUI.TagField(InLine.NextRect(), condition.checkTag);
                    break;
                case FindType.Layer:
                    condition.layer = EditorGUI.MaskField(InLine.NextRect(), condition.layer, FindLayerNames());
                    break;
                case FindType.ByType:
                    condition.typeTemplate = EditorGUI.ObjectField(InLine.NextRect(), condition.typeTemplate, typeof(UnityEngine.Object), true);
                    if (condition.typeTemplate == null) EditorGUILayout.HelpBox("A Amount Condition needs a type template", MessageType.Warning);
                    break;
            }
            EditorGUI.LabelField(InLine.GetLine(0, pos.width / 3, 5f),"Current", GUIStyle.none);
            EditorGUI.IntField(InLine.NextRect(), condition.numOfObjects, GUIStyle.none);
        }

        InLine.SetRect(pos, 1, 0, 6);
        {
            EditorGUI.LabelField(InLine.NextRect(), "Remaining");
            condition.typeOfCompare = (CompareType)EditorGUI.EnumPopup(InLine.NextRect(), condition.typeOfCompare);
            condition.amount = EditorGUI.IntField(InLine.NextRect(), condition.amount);
        }

        condition.triggerArea = EditorGUI.BoundsField(InLine.GetLine(0, pos.width / 2, 1), condition.triggerArea);

        InLine.Reset();
    }

    private void DrawCondition(Rect pos, InputCondition condition, GUIContent label)
    {
        InLine.SetRect(pos, 0, 0, 3);
        {
            EditorGUI.LabelField(InLine.NextRect(), "Button");
            condition.button = EditorGUI.TextField(InLine.NextRect(), condition.button);
        }
        InLine.SetRect(pos, 1, 0, 3);
        {
            EditorGUI.LabelField(InLine.NextRect(), "InputType");
            condition.type = (InputType)EditorGUI.EnumPopup(InLine.NextRect(), condition.type);
        }
    }

    private void DrawCondition(Rect pos, TriggerCondition condition, GUIContent label)
    {
        EditorGUI.LabelField(InLine.GetRect(pos, 0, 0, 2), "Trigger");
        condition.referenceTrigger = (Trigger)EditorGUI.ObjectField(InLine.GetRect(pos, 0, 100, 1), condition.referenceTrigger, typeof(Trigger), true);
        InLine.SetRect(pos, 1, 0, 6);
        {
            EditorGUI.LabelField(InLine.NextRect(), "IsTrue");
            condition.isTrue = EditorGUI.Toggle(InLine.NextRect(true), condition.isTrue);
        }
    }
    #endregion
}