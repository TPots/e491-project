using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class CaveSetupWindow : EditorWindow
{

    CaveSetupTemplate caveSetupTemplate;
    CaveDisplayTemplate selectedDisplayTemplate = null;

    public static void OpenWindow(CaveSetupTemplate caveSetupTemplate)
    {
        CaveSetupWindow window = GetWindow<CaveSetupWindow>("Cave Editor");
        window.caveSetupTemplate = caveSetupTemplate;
        window.selectedDisplayTemplate = null;
        window.Repaint();
    }

    void OnGUI()
    {
        int spaceConst = 12;

        CaveDisplayTemplate[] displayArray = 
        {
            caveSetupTemplate.display1,
            caveSetupTemplate.display2,
            caveSetupTemplate.display3,
            caveSetupTemplate.display4,
            caveSetupTemplate.display5,
            caveSetupTemplate.display6,
            caveSetupTemplate.display7,
            caveSetupTemplate.display8
        };

        GUILayout.BeginVertical();
            GUILayout.Label("Setup");
            EditorGUI.indentLevel++;
            caveSetupTemplate.label = EditorGUILayout.TextField("Label", caveSetupTemplate.label);
            GUILayout.Space(spaceConst);
            EditorGUI.indentLevel--;
            GUILayout.Label("Root Coordinates -- absolute");
            EditorGUI.indentLevel++;
            caveSetupTemplate.rootObjectReference.position = EditorGUILayout.Vector3Field("Position", caveSetupTemplate.rootObjectReference.position);
            caveSetupTemplate.rootObjectReference.rotation = EditorGUILayout.Vector3Field("Rotation", caveSetupTemplate.rootObjectReference.rotation);
            caveSetupTemplate.rootScale = EditorGUILayout.FloatField("Root Scale", caveSetupTemplate.rootScale);
            GUILayout.Space(spaceConst);
            EditorGUI.indentLevel--;
            GUILayout.Label("User Coordinates -- relative to root");
            EditorGUI.indentLevel++;
            caveSetupTemplate.userObjectReference.position = EditorGUILayout.Vector3Field("Position", caveSetupTemplate.userObjectReference.position);
            caveSetupTemplate.userObjectReference.rotation = EditorGUILayout.Vector3Field("Rotation", caveSetupTemplate.userObjectReference.rotation);
            GUILayout.Space(spaceConst);
            EditorGUI.indentLevel--;
            caveSetupTemplate.numberOfDisplays = EditorGUILayout.IntSlider("Number of Displays", caveSetupTemplate.numberOfDisplays,0,8);
            GUILayout.Space(spaceConst);
        GUILayout.EndVertical();

        GUILayout.BeginHorizontal();
            GUILayout.BeginVertical("box",GUILayout.MaxWidth(150), GUILayout.ExpandHeight(true));
                for(int i = 0 ; i < caveSetupTemplate.numberOfDisplays; i++)
                {
                    if(GUILayout.Button(displayArray[i].tag))
                    {
                        selectedDisplayTemplate = displayArray[i];
                    }
                }
            GUILayout.EndVertical();
            GUILayout.BeginVertical("box", GUILayout.ExpandHeight(true));
                if( selectedDisplayTemplate != null)
                {
                    selectedDisplayTemplate.tag = EditorGUILayout.TextField("Tag",selectedDisplayTemplate.tag);
                    GUILayout.Label("Display Coordinates -- relative to root");
                    EditorGUI.indentLevel++;
                    selectedDisplayTemplate.caveObjectReference.position = EditorGUILayout.Vector3Field("Position",selectedDisplayTemplate.caveObjectReference.position);
                    selectedDisplayTemplate.caveObjectReference.rotation = EditorGUILayout.Vector3Field("Rotation",selectedDisplayTemplate.caveObjectReference.rotation);
                    EditorGUI.indentLevel--;
                    selectedDisplayTemplate.displayDimentions = EditorGUILayout.Vector2Field("Display Dimentions",selectedDisplayTemplate.displayDimentions);
                    selectedDisplayTemplate.drawDistance = EditorGUILayout.FloatField("Camera Draw Distance",selectedDisplayTemplate.drawDistance);
                }
                else
                {
                    if(caveSetupTemplate.numberOfDisplays == 0)
                    {
                        GUILayout.Label("Set the number of displays.");
                    }
                    else
                    {
                        GUILayout.Label("Select a display to edit.");
                    }
                }
            GUILayout.EndVertical();
        GUILayout.EndHorizontal();

        GUILayout.Box("Note that when generating objects, Unity can not be in Play mode. Game objects created while Unity is in Play mode will be deleted upon exiting Play mode.");

        if (GUILayout.Button("Generate Game Object"))
        {
            GenerateObjects( caveSetupTemplate );
        }

        if (GUILayout.Button("Randomize Setup"))
        {
            RandomizeDisplays( caveSetupTemplate );
        }

    }

    public void GenerateObjects( CaveSetupTemplate caveSetupTemplate ){

        string projectString = System.String.Format("Cave-" + caveSetupTemplate.label);
        SanitizeProject( projectString );

        GameObject setupObj = new GameObject( projectString );
        setupObj.AddComponent<CaveSetup>();
        CaveSetup setupScr = setupObj.GetComponent<CaveSetup>();

        setupScr.caveSetupTemplate = caveSetupTemplate;
        setupObj.transform.position = caveSetupTemplate.rootObjectReference.position;
        setupObj.transform.rotation = Quaternion.Euler( caveSetupTemplate.rootObjectReference.rotation );

        GameObject userObj = GenerateUser( caveSetupTemplate.userObjectReference, setupObj );

        setupScr.userObj = userObj;
        setupScr.userScr = userObj.GetComponent<CaveUser>();
        
        List<GameObject> displayObj = new List<GameObject>();
        List<CaveDisplay> displayScr = new List<CaveDisplay>();
        CaveDisplayTemplate[] displayArray = 
        {
            caveSetupTemplate.display1,
            caveSetupTemplate.display2,
            caveSetupTemplate.display3,
            caveSetupTemplate.display4,
            caveSetupTemplate.display5,
            caveSetupTemplate.display6,
            caveSetupTemplate.display7,
            caveSetupTemplate.display8
        };

        for(int i = 0 ; i < caveSetupTemplate.numberOfDisplays ; i++ )
        {
            GameObject dispObj = GenerateDisplay( displayArray[i], setupObj, caveSetupTemplate.rootScale );
            GameObject camObj = GenerateCamera( userObj, dispObj );
            displayObj.Add( dispObj );
            displayScr.Add( dispObj.GetComponent<CaveDisplay>() );
        } 

        setupScr.init(displayObj, displayScr);
        setupScr.updateScr();
    }

    private void SanitizeProject(string label){
        //bool mainCameraEnabled = Camera.main.enabled;
        //if (mainCameraEnabled){ Camera.main.enabled = false; }
        GameObject existingObj = GameObject.Find(label);
        if (existingObj == null) return;
        Debug.Log("Found existing GameObject with matching label. Destroying ...");
        DestroyImmediate(existingObj);
    }

    private GameObject GenerateUser(CaveObjectReference userObjectReference, GameObject setupObj)
    {
        GameObject userObj = new GameObject( "user");
        userObj.transform.SetParent( setupObj.transform );
        userObj.AddComponent<CaveUser>();
        CaveUser userScr = userObj.GetComponent<CaveUser>();
        userScr.caveObjectReference = userObjectReference;
        userScr.updateScr();
        return userObj;
    }

    private GameObject GenerateCamera( GameObject userObj, GameObject displayObj )
    {
        CaveDisplay displayScr = displayObj.GetComponent<CaveDisplay>();

        GameObject camObj = new GameObject("camera-" + displayScr.caveDisplayTemplate.tag);
        camObj.transform.SetParent( userObj.transform );
        camObj.transform.localPosition = Vector3.zero;
        camObj.transform.localRotation = Quaternion.Euler( Vector3.zero );
        camObj.transform.localScale = Vector3.one;

        camObj.AddComponent<CaveCamera>();
        CaveCamera camScr = camObj.GetComponent<CaveCamera>();

        camScr.displayObject = displayObj;
        camScr.displayScr = displayScr;

        camObj.AddComponent<Camera>();

        camScr.updateScr();
        return camObj;
    }

    private GameObject GenerateDisplay( CaveDisplayTemplate caveDisplayTemplate, GameObject setupObj, float rootScale )
    {
        GameObject dispObj = new GameObject( "display-" + caveDisplayTemplate.tag );
        dispObj.transform.SetParent( setupObj.transform );
        dispObj.AddComponent<CaveDisplay>();
        CaveDisplay dispScr = dispObj.GetComponent<CaveDisplay>();

        dispScr.caveDisplayTemplate = caveDisplayTemplate;
        dispScr.displayScale = rootScale;
        dispScr.updateScr();
        return dispObj;
    }

    private void RandomizeDisplays(CaveSetupTemplate caveSetupTemplate){
        var seed = new System.Random();
        CaveDisplayTemplate[] displayArray =  {
            caveSetupTemplate.display1,
            caveSetupTemplate.display2,
            caveSetupTemplate.display3,
            caveSetupTemplate.display4,
            caveSetupTemplate.display5,
            caveSetupTemplate.display6,
            caveSetupTemplate.display7,
            caveSetupTemplate.display8
            };
        for(int i = 0 ; i < caveSetupTemplate.numberOfDisplays ; i++ ){
            RandomizeVector( seed, displayArray[i]);
        }
            
    }

    private void RandomizeVector(System.Random seed, CaveDisplayTemplate displayTemplate){
        double min = 1;
        double max = 2;
        double range = max - min;
        float rngX = (float)((seed.NextDouble()*range) + min);
        float rngZ = (float)((seed.NextDouble()*range) + min);
        displayTemplate.caveObjectReference.position = new Vector3(rngX, 1f, rngZ);
    }

    void OnLostFocus()
    {
        this.selectedDisplayTemplate = null;
    }

    void OnFocus()
    {
       this.selectedDisplayTemplate = null; 
    }

}


