using UnityEngine;
using System.Collections.Generic;


public class CaveSetup : MonoBehaviour{
    public CaveSetupTemplate caveSetupTemplate;
    public GameObject userObj;
    public CaveUser userScr;
    public List<GameObject> displayObjs;
    public List<CaveDisplay> displayScrs;
    public void init(List<GameObject> displayObjs, List<CaveDisplay> displayScrs)
    {
        this.displayObjs = displayObjs;
        this.displayScrs = displayScrs;
    }

    public void OnDrawGizmos()
    {
        Vector3 rootPosition = this.gameObject.transform.position;
        Vector3 userPosition = this.userObj.transform.position;
        Vector3 displayPosition = new Vector3();

        // Draw gizmo for the root object
        Gizmos.color = Color.gray;
        Gizmos.DrawSphere( this.gameObject.transform.position, 0.25f );

        // Draw gizmo for the user object
        Gizmos.color = Color.white;
        Gizmos.DrawSphere(  this.userObj.transform.position, 0.25f);
        Vector3 rootUserVector = userPosition - rootPosition;
        Vector3 userX = Vector3.Dot( rootUserVector, this.gameObject.transform.right ) * this.gameObject.transform.right;
        Vector3 userY = Vector3.Dot( rootUserVector, this.gameObject.transform.up ) * this.gameObject.transform.up;
        Vector3 userZ = Vector3.Dot( rootUserVector, this.gameObject.transform.forward ) * this.gameObject.transform.forward;

        Gizmos.color = Color.red;
        Gizmos.DrawLine( rootPosition, rootPosition + userX );
        Gizmos.color = Color.blue;
        Gizmos.DrawLine( rootPosition + userX, rootPosition + userX + userZ );
        Gizmos.color = Color.green;
        Gizmos.DrawLine( rootPosition + userX + userZ, rootPosition + userX + userY + userZ );

        Gizmos.color = Color.yellow;
    }

    public void updateScr()
    {
        this.gameObject.transform.position = this.caveSetupTemplate.rootObjectReference.position;
        this.gameObject.transform.rotation = Quaternion.Euler( this.caveSetupTemplate.rootObjectReference.rotation );
        this.gameObject.transform.localScale = Vector3.one * this.caveSetupTemplate.rootScale;

        this.userScr.caveObjectReference = this.caveSetupTemplate.userObjectReference;
        this.userScr.updateScr();

        CaveDisplayTemplate[] displayArray = 
        {
            this.caveSetupTemplate.display1,
            this.caveSetupTemplate.display2,
            this.caveSetupTemplate.display3,
            this.caveSetupTemplate.display4,
            this.caveSetupTemplate.display5,
            this.caveSetupTemplate.display6,
            this.caveSetupTemplate.display7,
            this.caveSetupTemplate.display8
        };

        for( int i = 0 ; i < this.displayScrs.Count ; i++ )
        {
            this.displayScrs[i].caveDisplayTemplate = displayArray[i];
            this.displayScrs[i].displayScale = this.caveSetupTemplate.rootScale; 
            this.displayScrs[i].updateScr();

        }
    }

    public void Start()
    {
        for( int i = 0 ; i < Display.displays.Length ; i++ )
        {
            Display.displays[i].Activate();
        }
    }

    public void Update()
    {
        updateScr();
    }   
}