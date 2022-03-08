using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CaveDisplay : MonoBehaviour
{
    public CaveDisplayTemplate caveDisplayTemplate;
    public Vector3 test;
    public float displayScale;
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
    public float minimumNormalHorizontal;
    public float minimumNormalVertical;

    [ExecuteInEditMode]
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

    [ExecuteInEditMode]
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

    public void updateScr()
    {
        this.gameObject.transform.localPosition = this.caveDisplayTemplate.caveObjectReference.position;;
        this.gameObject.transform.localRotation = Quaternion.Euler( this.caveDisplayTemplate.caveObjectReference.rotation );
        this.gameObject.transform.localScale = Vector3.one;

        GameObject thisObj = this.gameObject;

        Vector3 displayWidthOffset = thisObj.transform.localRotation * new Vector3(
            this.displayScale * this.caveDisplayTemplate.displayDimentions[0]/2f,0f,0f
        );

        Vector3 displayHeightOffset = thisObj.transform.localRotation * new Vector3(
            0f,this.displayScale * this.caveDisplayTemplate.displayDimentions[1]/2f,0f
        );

        this.displayCenter = this.gameObject.transform.position;
        this.displayNormal = thisObj.transform.forward;

        this.cornerUpperRight =  this.displayCenter + displayWidthOffset + displayHeightOffset;
        this.cornerUpperLeft = this.displayCenter - displayWidthOffset + displayHeightOffset;
        this.cornerLowerLeft =  this.displayCenter - displayWidthOffset - displayHeightOffset;
        this.cornerLowerRight =  this.displayCenter + displayWidthOffset - displayHeightOffset;

        this.edgeRight = this.displayCenter + displayWidthOffset;
        this.edgeTop = this.displayCenter + displayHeightOffset;
        this.edgeLeft = this.displayCenter - displayWidthOffset;
        this.edgeBottom = this.displayCenter - displayHeightOffset;

        this.minimumNormalHorizontal = this.caveDisplayTemplate.displayDimentions[0] * (0.5f / Mathf.Tan( 67.5f * Mathf.PI / 180f ) ) ;
        this.minimumNormalVertical =  this.minimumNormalHorizontal * 0.5f;
    }

    void Start()
    {
        //scrUpdate();
    }

    // Update is called once per frame
    void Update()
    {
        //scrUpdate();
    }
}
