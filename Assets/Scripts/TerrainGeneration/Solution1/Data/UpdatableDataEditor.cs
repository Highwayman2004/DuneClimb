using UnityEditor;
using UnityEngine;


[CustomEditor(typeof(UpdatableData), true)]
public class UpdatableDataEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        UpdatableData data = (UpdatableData)target;

        if(GUILayout.Button("Upddate"))
        {
            data.NotifyOfUpdatedValues();
        }
    }
}
