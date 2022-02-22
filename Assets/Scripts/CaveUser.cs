using UnityEngine;

public class CaveUser : MonoBehaviour{
    
    public CaveObjectReference caveObjectReference;

    [ExecuteAlways]
    private void scrUpdate()
    {
        this.gameObject.transform.localPosition = this.caveObjectReference.position;
        this.gameObject.transform.localRotation = Quaternion.Euler( this.caveObjectReference.rotation );
        this.gameObject.transform.localScale = Vector3.one;
    }

    void Update()
    {
        scrUpdate();
    }

    void Start()
    {
        scrUpdate();
    }
}