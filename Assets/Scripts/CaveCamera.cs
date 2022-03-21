using UnityEngine;

public class CaveCamera : MonoBehaviour{

    public GameObject displayObject;
    public CaveDisplay displayScr;

    public Vector3 userToDisplay;

    public float normalDistance;

    public Matrix4x4 projectionM;

    public void updateScr()
    {
        CalculateProjectionMatrix();
    }

    public void CalculateProjectionMatrix()
    {
        GameObject displayObj = this.displayObject;
        CaveDisplay displayScr = this.displayScr;
        Camera camera = this.gameObject.GetComponent<Camera>();

        Debug.Log( this.gameObject.transform.position );

        this.normalDistance = Vector3.Dot( displayObj.transform.position - this.gameObject.transform.position, displayObj.transform.forward );
        float normalDistance = this.normalDistance;

        float displayWidth = displayScr.caveDisplayTemplate.displayDimentions[0];
        float displayHeight = displayScr.caveDisplayTemplate.displayDimentions[1];
        
        float near = Mathf.Max(normalDistance,displayScr.minimumNormalHorizontal) ;

        float far = camera.farClipPlane;

        float scaledDisplayWidth = Mathf.Abs( near / displayWidth );
        float scaledDisplayHeight = Mathf.Abs( near / displayHeight );

        Vector3 scaledRight = (displayScr.edgeRight - displayScr.displayCenter) * scaledDisplayWidth + displayScr.displayCenter;
        Vector3 scaledTop = (displayScr.edgeTop - displayScr.displayCenter) * scaledDisplayHeight + displayScr.displayCenter;
        Vector3 scaledLeft = (displayScr.edgeLeft - displayScr.displayCenter) * scaledDisplayWidth + displayScr.displayCenter;
        Vector3 scaledBottom = (displayScr.edgeBottom - displayScr.displayCenter) * scaledDisplayHeight + displayScr.displayCenter;

        float right = Vector3.Dot((scaledRight - this.gameObject.transform.position), displayObj.transform.right ) ;
        float left = Vector3.Dot((scaledLeft - this.gameObject.transform.position), displayObj.transform.right ) ;
        float top = Vector3.Dot((scaledTop - this.gameObject.transform.position), displayObj.transform.up );
        float bottom = Vector3.Dot((scaledBottom - this.gameObject.transform.position), displayObj.transform.up ) ;

        Vector3 dispUp = (displayScr.cornerUpperRight - displayScr.cornerLowerRight).normalized;
        Vector3 dispRight =  (displayScr.cornerUpperRight - displayScr.cornerUpperLeft).normalized;
        Vector3 dispForward = - Vector3.Cross( dispRight, dispUp );

        this.projectionM = Matrix4x4.Frustum(left,right,bottom,top,near,far);
    
        Matrix4x4 displayM = Matrix4x4.zero;
        displayM[0,0] = dispRight.x;
        displayM[0,1] = dispRight.y;
        displayM[0,2] = dispRight.z;

        displayM[1,0] = dispUp.x;
        displayM[1,1] = dispUp.y;
        displayM[1,2] = dispUp.z;

        displayM[2,0] = dispForward.x;
        displayM[2,1] = dispForward.y;
        displayM[2,2] = dispForward.z;

        displayM[3,3] = 1f;

        Matrix4x4 translationM = Matrix4x4.Translate(
            - this.gameObject.transform.position
        );

        Matrix4x4 rotationM = Matrix4x4.Rotate(
            displayObj.transform.rotation
        );

        if (displayM.ValidTRS() && translationM.ValidTRS())
        {
            camera.worldToCameraMatrix =  displayM * translationM;
            camera.nearClipPlane = near;
            camera.farClipPlane = far;
        }
        else
        {
            Debug.Log("displayM or translationM are not valid");
        }

        camera.projectionMatrix = projectionM;
        
    }

    public void Start(){
        Camera camera = this.gameObject.GetComponent<Camera>();
        camera.Reset();
    }
    public void Update(){
        updateScr();
    }

    public void LateUpdate(){
    }
}