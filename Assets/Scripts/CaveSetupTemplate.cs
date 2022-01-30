using UnityEngine;

[CreateAssetMenu(fileName = "New Cave Setup", menuName = "Cave Setup")]
public class CaveSetupTemplate : ScriptableObject{
    public string label = "Untitled";
    public CaveObjectReference rootObjectReference;
    public CaveObjectReference userObjectReference;
    public int numberOfDisplays;
    public Vector2 defaultDisplayDimentions = new Vector2(0.5f,0.5f);
    public CaveDisplayTemplate display1;
    public CaveDisplayTemplate display2;
    public CaveDisplayTemplate display3;
    public CaveDisplayTemplate display4;
    public CaveDisplayTemplate display5;
    public CaveDisplayTemplate display6;
    public CaveDisplayTemplate display7;
    public CaveDisplayTemplate display8;

}