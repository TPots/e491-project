using UnityEngine;

public class CaveCamera : MonoBehaviour{

    public GameObject userObject;
    public GameObject displayObject;
    public Vector3 userToDisplay;

    public FrustumPlanes fp;

    public void init( GameObject userObj, GameObject displayObj)
    {
        GameObject thisObj = this.gameObject;
        this.userObject = userObj;
        this.displayObject = displayObj;
        thisObj.transform.SetParent( userObj.transform );
        thisObj.transform.position = userObj.transform.position;
        thisObj.transform.rotation = userObj.transform.rotation;
        UpdateToUserPosition();
    }

    [ExecuteAlways]
    public void UpdateToUserPosition()
    {
        this.gameObject.transform.position = this.userObject.transform.position;
        //this.gameObject.transform.LookAt( this.displayObject.transform );
        this.userToDisplay = this.displayObject.transform.position - this.gameObject.transform.position;
        CalculateProjectionMatrix();
    }

    public void OnDrawGizmos()
    {
        GameObject thisObj = this.gameObject;
        Gizmos.color = Color.white;
        Gizmos.DrawLine( thisObj.transform.position, this.displayObject.transform.position);

        Gizmos.color = Color.red;
        Gizmos.DrawLine( 
            thisObj.transform.position,
            thisObj.transform.position + new Vector3( this.userToDisplay[0], 0f, 0f )
        );

        Gizmos.color = Color.blue;
        Gizmos.DrawLine( 
            thisObj.transform.position + new Vector3( this.userToDisplay[0], 0f, 0f ),
            thisObj.transform.position + new Vector3( this.userToDisplay[0], 0f, this.userToDisplay[2] )
        );

        Gizmos.color = Color.green;
        Gizmos.DrawLine( 
            thisObj.transform.position + new Vector3( this.userToDisplay[0], 0f, this.userToDisplay[2] ),
            thisObj.transform.position + this.userToDisplay
        );
    }

    public void CalculateProjectionMatrix()
    {
        GameObject displayObj = this.displayObject;
        CaveDisplay displayScr = displayObj.GetComponent<CaveDisplay>();
        GameObject thisObj = this.gameObject;
        Camera camera = thisObj.GetComponent<Camera>();

        Vector3 eye = thisObj.transform.position;
        Vector3 eyeToDisplay = displayScr.displayCenter - eye;
        //camera.ResetProjectionMatrix();
        
        float right = Vector3.Dot((displayScr.edgeRight - eye), displayObj.transform.right );
        float left = Vector3.Dot((displayScr.edgeLeft - eye), displayObj.transform.right );
        float top = Vector3.Dot((displayScr.edgeTop - eye), displayObj.transform.up );
        float bottom = Vector3.Dot((displayScr.edgeBottom - eye), displayObj.transform.up );
        float near = Vector3.Dot(eyeToDisplay, displayObj.transform.forward);
        float far =  near * 50f;// displayScr.caveDisplayTemplate.drawDistance + near;

        /*
        float x = 2.0F * near / (right - left);
        float y = 2.0F * near / (top - bottom);
        float a = (right + left) / (right - left);
        float b = (top + bottom) / (top - bottom);
        float c = -(far + near) / (far - near);
        float d = -(2.0F * far * near) / (far - near);
        float e = -1.0F;
        Matrix4x4 projectionM = new Matrix4x4();
        projectionM[0, 0] = x;
        projectionM[0, 1] = 0;
        projectionM[0, 2] = a;
        projectionM[0, 3] = 0;
        projectionM[1, 0] = 0;
        projectionM[1, 1] = y;
        projectionM[1, 2] = b;
        projectionM[1, 3] = 0;
        projectionM[2, 0] = 0;
        projectionM[2, 1] = 0;
        projectionM[2, 2] = c;
        projectionM[2, 3] = d;
        projectionM[3, 0] = 0;
        projectionM[3, 1] = 0;
        projectionM[3, 2] = e;
        projectionM[3, 3] = 0;
        */

        Matrix4x4 projectionM = Matrix4x4.Frustum(left,right,bottom,top,near,far);
        //Matrix4x4 projectionM = Matrix4x4.Ortho(left,right,bottom,top,near,far );
    


        Vector3 dispUp = (displayScr.cornerUpperRight - displayScr.cornerLowerRight).normalized;
        Vector3 dispRight =  (displayScr.cornerUpperRight - displayScr.cornerUpperLeft).normalized;
        Vector3 dispForward = - Vector3.Cross( dispRight, dispUp );

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
            -eye
        );

        Matrix4x4 rotationM = Matrix4x4.Rotate(
            //Quaternion.Inverse( thisObj.transform.rotation )
                Quaternion.FromToRotation(
                    userObject.transform.forward,
                    displayObj.transform.forward

            )
        );

        //projectionM[2,0] *= -1;
        //projectionM[2,1] *= -1;
        //projectionM[2,2] *= -1;
        //projectionM[2,3] *= -1;

        camera.nearClipPlane = near;
        camera.farClipPlane = far;

        camera.worldToCameraMatrix = displayM * translationM;

        camera.projectionMatrix = projectionM;
    }

    public void Start(){
        
    }
    public void Update(){
        UpdateToUserPosition();
    }

    public void LateUpdate(){
    }
}