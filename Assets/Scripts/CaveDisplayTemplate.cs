using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CaveDisplayTemplate
{
    public string tag;
    public CaveObjectReference caveObjectReference;
    public Vector2 displayDimentions = new Vector2(0.5f, 0.5f);
    public float drawDistance = 100f;

    public CaveDisplayTemplate(string tag)
    {
        this.tag = tag;
    }
}


