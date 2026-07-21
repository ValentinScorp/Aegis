using UnityEditor;
using UnityEngine;
using Aegis.View;
using Aegis.Core;

[CustomEditor(typeof(EntityView))]
public class EntityViewDebugEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        var view = (EntityView)target;
        
        var unit = view.GetUnit(); 
        if (unit == null) return;

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Debug (Core state)", EditorStyles.boldLabel);
        EditorGUILayout.LabelField("HP", $"{unit.Health.Current:0} / {unit.Health.Max:0}");
        EditorGUILayout.LabelField("State", unit.StateMachine.Current?.GetType().Name ?? "—");
        EditorGUILayout.LabelField("Is Alive", unit.Health.IsAlive.ToString());

        if (Application.isPlaying)
            Repaint();
    }
}