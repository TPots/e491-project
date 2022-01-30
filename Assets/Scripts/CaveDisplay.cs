using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CaveDisplay : MonoBehaviour
{
    public CaveDisplayTemplate caveDisplayTemplate;
    public Vector3 displayCenter;
    public Vector3 displayNormal;
    public Vector3 cornerUpperLeft;
    public Vector3 cornerUpperRight;
    public Vector3 cornerLowerLeft;
    public Vector3 cornerLowerRight;
    public Vector3 edgeRight;
    public Vector3 edgeTop;
    public Vector3 edgeLeft;
    public Vector3 edgeBottom;

    public void init( GameObject parentObj, CaveDisplayTemplate displayTemplate )
    {
        this.caveDisplayTemplate = displayTemplate;
        GameObject thisObj = this.gameObject;
        thisObj.transform.SetParent( parentObj.transform );
        thisObj.transform.position = parentObj.transform.position;
        thisObj.transform.rotation = parentObj.transform.rotation;
        thisObj.transform.localPosition = displayTemplate.caveObjectReference.position;
        thisObj.transform.localRotation = Quaternion.Euler( displayTemplate.caveObjectReference.rotation );

        Vector3 displayWidthOffset = thisObj.transform.rotation * new Vector3(
            this.caveDisplayTemplate.displayDimentions[0]/2f,0f,0f
        );

        Vector3 displayHeightOffset = thisObj.transform.rotation * new Vector3(
            0f,this.caveDisplayTemplate.displayDimentions[1]/2f,0f
        );

        this.displayCenter = thisObj.transform.position;
        this.displayNormal = thisObj.transform.forward;

        this.cornerUpperRight =  thisObj.transform.position + displayWidthOffset + displayHeightOffset;
        this.cornerUpperLeft = thisObj.transform.position - displayWidthOffset + displayHeightOffset;
        this.cornerLowerLeft =  thisObj.transform.position - displayWidthOffset - displayHeightOffset;
        this.cornerLowerRight =  thisObj.transform.position + displayWidthOffset - displayHeightOffset;

        this.edgeRight = thisObj.transform.position + displayWidthOffset;
        this.edgeTop = thisObj.transform.position + displayHeightOffset;
        this.edgeLeft = thisObj.transform.position - displayWidthOffset;
        this.edgeBottom = thisObj.transform.position - displayHeightOffset;

        /*
        GameObject indicatorSphere = new GameObject(name = "Display Center");
        indicatorSphere.transform.SetParent( thisObj.transform );
        indicatorSphere.transform.localPosition = thisObj.transform.localPosition;
        */
    }

    public void OnDrawGizmos(){
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine( this.displayCenter, this.displayCenter + this.displayNormal);
        Gizmos.color = Color.red;
        Gizmos.DrawLine( this.cornerUpperRight, this.cornerUpperLeft);
        Gizmos.DrawLine( this.cornerUpperLeft, this.cornerLowerLeft);
        Gizmos.DrawLine( this.cornerLowerLeft, this.cornerLowerRight);
        Gizmos.DrawLine( this.cornerLowerRight, this.cornerUpperRight);
        Gizmos.color = Color.white;
        Gizmos.DrawSphere( this.gameObject.transform.position, 0.1f );
    }

    public void OnDrawGizmosSelected(){
        Gizmos.color = Color.red;
        Gizmos.DrawSphere( this.edgeRight, 0.1f );
        Gizmos.color = Color.green;
        Gizmos.DrawSphere( this.edgeTop, 0.1f );
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere( this.edgeLeft, 0.1f );
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere( this.edgeBottom, 0.1f );
    }

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
