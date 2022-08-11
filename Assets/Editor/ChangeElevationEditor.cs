using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ChangeElevation))]
public class ChangeElevationEditor : Editor
{
    SerializedProperty firstPoint;
    SerializedProperty secondPoint;




    void OnEnable()
    {
        firstPoint = serializedObject.FindProperty("firstPoint");
        secondPoint = serializedObject.FindProperty("secondPoint");
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        ChangeElevation myScript = (ChangeElevation)target;
        if (GUILayout.Button("West Elevation"))
        {
            //rotate myScript by -25 degrees
            myScript.transform.rotation = Quaternion.identity;
            myScript.transform.Rotate(0, 0, 25);
            myScript.BuildPoints(false);
        }

        if (GUILayout.Button("North Elevation"))
        {
            myScript.transform.rotation = Quaternion.identity;
            myScript.transform.Rotate(0, 0, -25);
            myScript.BuildPoints(true);
        }

        if (GUILayout.Button("Spawn points"))
        {

        }
    }
}