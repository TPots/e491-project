using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CaveDisplayTemplate
{
    /*
    Data class used to define display parameters
    tag : 
        a string labeling the display
        used to name the display game object in unity
    caveObjectReference:
        data class defined in Assets/Scripts/CaveObjectReference
        defines the XYZ displacement vector from the root node to the display and the XYZ rotation 
        value it written to the game objects local position and local rotation
    displayDimentions:
        two value XY vector
        describes the width and height of the physical display
    drawDistance:
        float value describing how far the far plane of the rendering camera is from the near plane
        used as part of the calculation for the projection matrix in Assets/Scripts/CaveCamera.cs
    enableAlignmentStructure:
        boolean flag which enables the alignment structure attached to the display game object as a child object.
        used for positioning the displays in unity and should be disabled afterwards.
    */
    public string tag;
    public CaveObjectReference caveObjectReference;
    public Vector2 displayDimentions = new Vector2(0.5f, 0.5f);
    public float drawDistance = 100f;
    public bool enableAlignmentStructure;
    public int alignmentStructureSelector;
    public CaveDisplayTemplate(string tag)
    {
        /*
        Constructor for the class CaveDisplayTemplate:
        Takes in a string argument that initlizes the <tag> field
        Used to automatically tag the game objects instantiated by the script Assets/Editor/CaveSetupWindow.cs
        */
        this.tag = tag;
    }
}


