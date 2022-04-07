using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CaveController : MonoBehaviour
{
    public CaveSetupTemplate cave;

    // Start is called before the first frame update
    void Start()
    {

        
    }

    // Update is called once per frame
    void Update()
    {
     cave.rootObjectReference.position = gameObject.transform.position;
     cave.rootObjectReference.rotation = gameObject.transform.eulerAngles;
     cave.rootObjectReference.position.y += gameObject.GetComponent<CharacterController>().height;
     //cave.rootObjectReference.rotation.x -= 20;
    }
}
