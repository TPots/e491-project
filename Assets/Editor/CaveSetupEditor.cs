using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;

public class AssetHandler
{
    [OnOpenAsset()]
    public static bool OpenEditor(int instanceId, int line)
    {
        CaveSetupTemplate caveSetupTemplate = EditorUtility.InstanceIDToObject(instanceId) as CaveSetupTemplate;
        if( caveSetupTemplate != null )
        {
            CaveSetupWindow.OpenWindow( caveSetupTemplate );
            return true;
        }
        return false;
    }
}

[CustomEditor(typeof(CaveSetupTemplate))]
public class CaveSetupTemplateEditor : Editor{
    public override void OnInspectorGUI(){
        
        CaveSetupTemplate caveSetupTemplate = (CaveSetupTemplate)target;

        if (GUILayout.Button("Open Editor"))
        {
            CaveSetupWindow.OpenWindow( caveSetupTemplate );
        }
    }
}
/*
[CustomEditor(typeof(CaveSetup))]
public class CaveSetupEditor : Editor{
    public override void OnInspectorGUI(){
        
        CaveSetup caveSetup = (CaveSetup)target;

        if (GUILayout.Button("Open Editor"))
        {
            CaveSetupWindow.OpenWindow( caveSetup.caveSetupTemplate );
        }
    }
}
    */