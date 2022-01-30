using UnityEngine;

public class CaveSetup : MonoBehaviour{
    public CaveSetupTemplate caveSetupTemplate;
    public void OnDrawGizmos()
    {
        Gizmos.color = Color.gray;
        Gizmos.DrawSphere( this.gameObject.transform.position, 0.25f );
    }

}