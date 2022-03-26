﻿using UnityEngine;

public class CaveCamera : MonoBehaviour{

    public GameObject displayObject;
    public CaveDisplay displayScr;
    public int camIdx;

    public Vector3 userToDisplay;

    public float normalDistance;

    public Matrix4x4 projectionM;

    public void updateScr()
    {
        ///CalculateProjectionMatrix();
        //getAsymProjMatrix(LowerLeft, LowerRight,UpperLeft,this.transform.position,0.01f,100.0f);
        //Debug.Log ("Display " + this.camIdx + ", upperLeft:" + this.displayScr.cornerUpperLeft);
        getAsymProjMatrix(this.displayScr.cornerLowerLeft, this.displayScr.cornerLowerRight, this.displayScr.cornerUpperLeft,this.transform.position,0.02f,120.0f);
    }

    /// <summary>
        /// Upper right (quadrant 1) corner world space coordinate
        /// </summary>
        public Vector3 UpperRight
        {
            get
            {
                return this.displayScr.transform.localToWorldMatrix * new Vector4(this.displayScr.halfWidth, this.displayScr.halfHeight, 0.0f, 1.0f);
            }
        }

        /// <summary>
        /// Upper left (quadrant 2) corner world space coordinate
        /// </summary>
        public Vector3 UpperLeft
        {
            get
            {
                return this.displayScr.transform.localToWorldMatrix * new Vector4(-this.displayScr.halfWidth, this.displayScr.halfHeight, 0.0f, 1.0f);
            }
        }

        /// <summary>
        /// Lower left (quadrant 3) corner world space coordinate
        /// </summary>
        public Vector3 LowerLeft
        {
            get
            {
                return this.displayScr.transform.localToWorldMatrix * new Vector4(-this.displayScr.halfWidth, -this.displayScr.halfHeight, 0.0f, 1.0f);
            }
        }

        /// <summary>
        /// Lower right (quadrant 4) corner world space coordinate
        /// </summary>
        public Vector3 LowerRight
        {
            get
            {
                return this.displayScr.transform.localToWorldMatrix * new Vector4(this.displayScr.halfWidth, -this.displayScr.halfHeight, 0.0f, 1.0f);
            }
        }

        /// <summary>
        /// Return the matrix projecting from from, through the quad specified
        /// </summary>
        /// <param name="lowerLeft">lower left point of quad</param>
        /// <param name="lowerRight">lower right point of quad</param>
        /// <param name="upperLeft">upper left point of quad</param>
        /// <param name="from">position of the eye</param>
        /// <param name="ncp">near clip plane</param>
        /// <param name="fcp">far clip plane</param>
        /// <returns></returns>
        public Matrix4x4 getAsymProjMatrix(Vector3 lowerLeft, Vector3 lowerRight, Vector3 upperLeft, Vector3 from, float ncp, float fcp)
        {
            //compute orthonormal basis for the screen - could pre-compute this...
            Vector3 vr = (lowerRight - lowerLeft).normalized;
            Vector3 vu = (upperLeft - lowerLeft).normalized;
            Vector3 vn = Vector3.Cross(vr, vu).normalized;

            //compute screen corner vectors
            Vector3 va = lowerLeft - from;
            Vector3 vb = lowerRight - from;
            Vector3 vc = upperLeft - from;

            //find the distance from the eye to screen plane
            float n = ncp;
            float f = fcp;
            float d = Vector3.Dot(va, vn); // distance from eye to screen
            float nod = n / d;
            float l = Vector3.Dot(vr, va) * nod;
            float r = Vector3.Dot(vr, vb) * nod;
            float b = Vector3.Dot(vu, va) * nod;
            float t = Vector3.Dot(vu, vc) * nod;

            //put together the matrix - bout time amirite?
            Matrix4x4 m = Matrix4x4.zero;

            //from http://forum.unity3d.com/threads/using-projection-matrix-to-create-holographic-effect.291123/
            // m[0, 0] = 2.0f * n / (r - l);
            // m[0, 2] = (r + l) / (r - l);
            // m[1, 1] = 2.0f * n / (t - b);
            // m[1, 2] = (t + b) / (t - b);
            // m[2, 2] = -(f + n) / (f - n);
            // m[2, 3] = ((-2.0f * f * n) / (f - n));
            // m[3, 2] = -1.0f;
            
            m[0, 0] = 2.0f * n / (r - l);
            m[0, 2] = (r + l) / (r - l);
            m[1, 1] = 2.0f * n / (t - b);
            m[1, 2] = (t + b) / (t - b);
            m[2, 2] = (f + n) / (n-f);
            m[2, 3] = ((2.0f * f * n) / (n - f));
            m[3, 2] = -1.0f;
            
            Camera camera = this.gameObject.GetComponent<Camera>();

            camera.nearClipPlane = ncp;
            camera.farClipPlane = fcp;
            camera.projectionMatrix = m;

            return m;
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