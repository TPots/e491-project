using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class CaveSetupWindow : EditorWindow
{

    CaveSetupTemplate caveSetupTemplate;
    CaveDisplayTemplate selectedDisplayTemplate = null;
    GameObject setupObj = null;

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

        if (caveSetupTemplate == null){
            this.Close();
            return;
        }
        caveSetupTemplate.label = "Cave-" + caveSetupTemplate.name; 
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
            GUILayout.Label(caveSetupTemplate.label);
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
            caveSetupTemplate.trackUser = EditorGUILayout.Toggle("Enable User Tracking", caveSetupTemplate.trackUser);
            EditorGUILayout.Vector3Field("Retina Offset", caveSetupTemplate.retinaOffset);
            if(GUILayout.Button("Set User Offset"))
            {
                caveSetupTemplate.retinaOffset = caveSetupTemplate.trackingDeviceSignal - caveSetupTemplate.userObjectReference.position;
            }
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
                    selectedDisplayTemplate.enableAlignmentStructure = EditorGUILayout.Toggle("Enable Alignment Structure", selectedDisplayTemplate.enableAlignmentStructure);
                    if(selectedDisplayTemplate.enableAlignmentStructure)
                    {
                        selectedDisplayTemplate.alignmentStructureSelector = EditorGUILayout.IntSlider("Alignment Structure", selectedDisplayTemplate.alignmentStructureSelector,0,1); 
                    }
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

        if (GUILayout.Button("Generate Objects"))
        {
            GenerateObjects(caveSetupTemplate);
        }
        if (GUILayout.Button("Randomize Setup"))
        {
            RandomizeDisplays( caveSetupTemplate );
        }

        // Mark CaveSetup As Dirty so we save the object
        EditorUtility.SetDirty(caveSetupTemplate);
        //updateObjects();
    }
    public void updateObjects( ){
        if(setupObj == null){
            return;
        }
        CaveSetup setupScr = setupObj.GetComponent<CaveSetup>();

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
            GameObject camObj = GenerateCamera(  setupScr.userObj, dispObj, i );
            displayObj.Add( dispObj );
            displayScr.Add( dispObj.GetComponent<CaveDisplay>() );
        } 

        setupScr.init(displayObj, displayScr);
        setupScr.updateScr();
    }

    public void GenerateObjects( CaveSetupTemplate caveSetupTemplate ){

        SanitizeProject(  caveSetupTemplate.label );
        setupObj = new GameObject( caveSetupTemplate.label  );
        setupObj.AddComponent<CaveSetup>();
        CaveSetup setupScr = setupObj.GetComponent<CaveSetup>();

        setupScr.caveSetupTemplate = caveSetupTemplate;
        setupObj.transform.position = caveSetupTemplate.rootObjectReference.position;
        setupObj.transform.rotation = Quaternion.Euler( caveSetupTemplate.rootObjectReference.rotation );
        GenerateZMQ( setupObj );
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
            GameObject camObj = GenerateCamera( userObj, dispObj, i );
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

    private GameObject GenerateCamera( GameObject userObj, GameObject displayObj, int cameraIdx )
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
        camScr.camIdx = cameraIdx;

        camObj.AddComponent<Camera>();
        Camera cam = camObj.GetComponent<Camera>();
        //cam.targetDisplay = cameraIdx;
        //Display.displays[ cameraIdx ].Activate();

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
        GameObject alignmentStructure = GenerateAlignmentTorus(caveDisplayTemplate, dispObj );
        dispScr.alignmentStructureRing = alignmentStructure;
        alignmentStructure = GenerateAlignmentPlane(caveDisplayTemplate, dispObj );
        dispScr.alignmentStructurePlane = alignmentStructure;
        dispScr.updateScr();
        return dispObj;
    }

    private GameObject GenerateAlignmentTorus( CaveDisplayTemplate caveDisplayTemplate, GameObject displayObj )
    {

        GameObject alignmentStructure = PrefabUtility.InstantiatePrefab( Resources.Load("Alignment Rings") ) as GameObject;
        alignmentStructure.transform.SetParent( displayObj.transform );
        alignmentStructure.transform.localPosition = Vector3.zero;
        alignmentStructure.transform.localRotation = Quaternion.identity;
        alignmentStructure.transform.localScale = new Vector3( 1f, 1f, 1f) * Mathf.Min( caveDisplayTemplate.displayDimentions[0], caveDisplayTemplate.displayDimentions[1] );

        return alignmentStructure;
    }

    private GameObject GenerateAlignmentPlane( CaveDisplayTemplate caveDisplayTemplate, GameObject displayObj )
    {
        CaveDisplay caveDisplay = displayObj.GetComponent<CaveDisplay>();
        GameObject alignmentStructure = PrefabUtility.InstantiatePrefab( Resources.Load("Plane") ) as GameObject;
        alignmentStructure.transform.SetParent( displayObj.transform );
        alignmentStructure.transform.localPosition = new Vector3( 0f, -caveDisplay.caveDisplayTemplate.displayDimentions[1]/2f, 0f );
        alignmentStructure.transform.localRotation = Quaternion.identity;
        
        return alignmentStructure;
    }

    private void GenerateZMQ( GameObject setupObj )
    {
        GameObject zmq = new GameObject("ZMQ");
        zmq.transform.SetParent( setupObj.transform );
        zmq.transform.position = Vector3.zero;
        zmq.transform.rotation = Quaternion.identity;

        CaveSetup setupScr = setupObj.GetComponent<CaveSetup>();

        zmq.AddComponent<PubSub.UserTracker>();
        PubSub.UserTracker ut = zmq.GetComponent<PubSub.UserTracker>();
        ut.setupObj = setupObj;
        ut.setupScr = setupScr;
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


