using UnityEngine;

public class CaveUser : MonoBehaviour{
    
    public CaveObjectReference caveObjectReference;
    public GameObject setupObj;

    public void init( GameObject parentObj, CaveObjectReference caveObjectReference )
    {
        GameObject thisObj = this.gameObject;
        thisObj.transform.SetParent( parentObj.transform );
        thisObj.transform.position = parentObj.transform.position;
        thisObj.transform.rotation = parentObj.transform.rotation;
        this.setupObj = parentObj;
        this.caveObjectReference = caveObjectReference;
        thisObj.transform.localPosition = this.caveObjectReference.position;
        thisObj.transform.localRotation = Quaternion.Euler( this.caveObjectReference.rotation );
    }

    [ExecuteAlways]
    public void UpdatePosition(){
        float vConst = 10f;
        if (Input.GetKey("w")){
            this.gameObject.transform.localPosition = this.gameObject.transform.localPosition + (this.gameObject.transform.forward / vConst);  
        }
        if (Input.GetKey("d")){
            this.gameObject.transform.localPosition = this.gameObject.transform.localPosition + (this.gameObject.transform.right / vConst);
        }
        if (Input.GetKey("s")){
            this.gameObject.transform.localPosition = this.gameObject.transform.localPosition - (this.gameObject.transform.forward / vConst);
        }
        if (Input.GetKey("a")){
            this.gameObject.transform.localPosition = this.gameObject.transform.localPosition - (this.gameObject.transform.right / vConst);
        }
        this.caveObjectReference.position = this.gameObject.transform.localPosition;
        this.caveObjectReference.rotation = this.gameObject.transform.localEulerAngles;
    }

    public void OnDrawGizmos(){
        Gizmos.color = Color.black;
        Gizmos.DrawLine( this.setupObj.transform.position, this.gameObject.transform.position);
        Gizmos.color = Color.white;
        Gizmos.DrawSphere( this.gameObject.transform.position, 0.1f );

    }

    public void Start(){
    }

    public void Update(){
        UpdatePosition();
    }

}