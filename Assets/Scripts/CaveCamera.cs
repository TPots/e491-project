using UnityEngine;

public class CaveCamera : MonoBehaviour{

    public GameObject displayObject;
    public CaveDisplay displayScr;
    public int camIdx;

    public Vector3 userToDisplay;

    public float normalDistance;

    public Matrix4x4 projectionM;

    public void updateScr()
    {

        //getAsymProjMatrix(this.displayScr.cornerLowerLeft, this.displayScr.cornerLowerRight, this.displayScr.cornerUpperLeft,this.transform.position,0.01f,100.0f);

        generalizedPerspectiveProjection();
    }


    // http://160592857366.free.fr/joe/ebooks/ShareData/Generalized%20Perspective%20Projection.pdf - Paper by Robert Kooima
    public void generalizedPerspectiveProjection(){
    Camera _camera = this.gameObject.GetComponent<Camera>();
    float n = 0.01f;
    float f = 100.0f;
    n = _camera.nearClipPlane;
    f = _camera.farClipPlane;

    // Display corners
    Vector3 pa = this.displayScr.cornerLowerLeft; 
    Vector3 pb = this.displayScr.cornerLowerRight;
    Vector3 pc = this.displayScr.cornerUpperLeft;
    Vector3 pe = this.transform.position;


    // Compute an orthonormal basis for the screen.
    Vector3 vr = (pb - pa).normalized;
    Vector3 vu = (pc - pa).normalized;
    Vector3 vn = Vector3.Cross(vu, vr).normalized;
    // Compute the screen corner vectors.
    Vector3 va = pa - pe;
    Vector3 vb = pb - pe;
    Vector3 vc = pc - pe;
    // Find the distance from the eye to screen plane.
    float d = -Vector3.Dot(va, vn);

    // Find the extent of the perpendicular projection. 
    //float nd = n / d;
    float nd = 1.0f;

    float l = Vector3.Dot(vr, va) * nd;
    float r = Vector3.Dot(vr, vb) * nd;
    float b = Vector3.Dot(vu, va) * nd;
    float t = Vector3.Dot(vu, vc) * nd;
    // Load the perpendicular projection.
    _camera.projectionMatrix = Matrix4x4.Frustum(l, r, b, t, n, f);
    _camera.transform.rotation = Quaternion.LookRotation(-vn, vu);
    }
    
    
    public void CalculateProjectionMatrix()
    {
        GameObject displayObj = this.displayObject;
        CaveDisplay displayScr = this.displayScr;
        Camera camera = this.gameObject.GetComponent<Camera>();

        //Debug.Log( this.gameObject.transform.position );

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
        camera.targetDisplay = this.camIdx;
        Debug.Log("Cam Idx = " +this. camIdx);
    }
    public void Update(){
        //updateScr();
    }

    public void LateUpdate(){
        updateScr();

    }
}