using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(CaveSetupTemplate))]
public class CaveSetupEditor : Editor{
    public override void OnInspectorGUI(){
        base.OnInspectorGUI();
        CaveSetupTemplate caveSetupTemplate = (CaveSetupTemplate)target;
        if (GUILayout.Button("Generate Game Object")){
            GenerateObjects( caveSetupTemplate );
        }

        if (GUILayout.Button("Randomize Setup")){
            RandomizeDisplays( caveSetupTemplate );
        }
    }

    public void GenerateObjects( CaveSetupTemplate caveSetupTemplate ){

        string projectString = System.String.Format("Cave-" + caveSetupTemplate.label);

        SanitizeProject( projectString );

        GameObject setupObj = GenerateSetupObject( caveSetupTemplate, projectString );
        setupObj.transform.position = caveSetupTemplate.rootObjectReference.position;
        setupObj.transform.rotation = Quaternion.Euler( caveSetupTemplate.rootObjectReference.rotation );
        GameObject userObj = GenerateUserObject( setupObj );
        GenerateCameraHead( setupObj, userObj );   
    }

    private void SanitizeProject(string label){
        //bool mainCameraEnabled = Camera.main.enabled;
        //if (mainCameraEnabled){ Camera.main.enabled = false; }
        GameObject existingObj = GameObject.Find(label);
        if (existingObj == null) return;
        Debug.Log("Found existing GameObject with matching label. Destroying ...");
        DestroyImmediate(existingObj);
    }

    private GameObject GenerateSetupObject(CaveSetupTemplate setupTemplate, string label){
        GameObject setupObj = new GameObject(name = label );
        setupObj.transform.position = Vector3.zero;
        setupObj.transform.rotation = Quaternion.Euler(Vector3.zero);
        setupObj.AddComponent<CaveSetup>();
        CaveSetup setupScr = setupObj.GetComponent<CaveSetup>();
        setupScr.caveSetupTemplate = setupTemplate;
        return setupObj;
    }

    private GameObject GenerateUserObject( GameObject setupObj ){
        CaveSetup setupScr = setupObj.GetComponent<CaveSetup>();

        GameObject userObj = new GameObject(name="User");
        if (setupScr != null){
            userObj.AddComponent<CaveUser>();
            CaveUser userScr = userObj.GetComponent<CaveUser>();
            userScr.init( setupObj, setupScr.caveSetupTemplate.userObjectReference );
        }
        else{ Debug.Log("<caveSetupScript> returned null.");}
        return userObj;
    }

    private void GenerateCameraHead( GameObject setupObj, GameObject userObj ){
        CaveSetup setupScr = setupObj.GetComponent<CaveSetup>();
        if (setupScr != null){
            CaveDisplayTemplate[] displayArray = {
                setupScr.caveSetupTemplate.display1,
                setupScr.caveSetupTemplate.display2,
                setupScr.caveSetupTemplate.display3,
                setupScr.caveSetupTemplate.display4,
                setupScr.caveSetupTemplate.display5,
                setupScr.caveSetupTemplate.display6,
                setupScr.caveSetupTemplate.display7,
                setupScr.caveSetupTemplate.display8
            };
            for( int i = 0 ; i < setupScr.caveSetupTemplate.numberOfDisplays ; i++ ){
                GameObject displayObj = GenerateDisplayObject( setupObj, displayArray[i], i );
                GenerateCameraObject( userObj, displayObj, i );
            }
        }
        else{ Debug.Log("<setupObj> returned null.");}
    }

    private GameObject GenerateDisplayObject( GameObject setupObj, CaveDisplayTemplate displayTemplate, int displayIdx )
    {
        string label = "Display-" + displayIdx.ToString();
        GameObject displayObj = new GameObject( name = label );
        displayObj.AddComponent<CaveDisplay>();
        CaveDisplay displayScr = displayObj.GetComponent<CaveDisplay>();
        displayScr.init(setupObj, displayTemplate);
        return displayObj;
    }

    private void GenerateCameraObject( GameObject userObj, GameObject displayObj, int displayIdx )
    {
        string label = "Camera-" + displayIdx.ToString();

        GameObject cameraObj = new GameObject(name = label );

        cameraObj.AddComponent<CaveCamera>();
        CaveCamera cameraScr = cameraObj.GetComponent<CaveCamera>();

        cameraObj.AddComponent<Camera>();
        Camera camera = cameraObj.GetComponent<Camera>();

        cameraScr.init( userObj, displayObj );

        camera.targetDisplay = displayIdx;
    }

    private void AttachIndicatorSphere( GameObject gameObj, string label ){
        GameObject indicatorSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        indicatorSphere.transform.SetParent(gameObj.transform);
        indicatorSphere.transform.position = gameObj.transform.localPosition;
        indicatorSphere.transform.localScale = new Vector3( 0.25f, 0.25f, 0.25f );
        indicatorSphere.name = label;
        indicatorSphere.tag = "EditorOnly";
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
}