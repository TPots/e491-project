using UnityEngine;

[CreateAssetMenu(fileName = "New Cave Setup", menuName = "Cave Setup")]
[System.Serializable]
public class CaveSetupTemplate : ScriptableObject{
    public string label = "Untitled";
    public CaveObjectReference rootObjectReference;
    public float rootScale = 1f;
    public CaveObjectReference userObjectReference;
    public bool trackUser;
    public Vector3 trackingDeviceSignal;
    public Vector3 retinaOffset = Vector3.zero;
    public int numberOfDisplays;
    public Vector2 defaultDisplayDimentions = new Vector2(0.5f,0.5f);
    public CaveDisplayTemplate display1 = new CaveDisplayTemplate("1");
    public CaveDisplayTemplate display2 = new CaveDisplayTemplate("2");
    public CaveDisplayTemplate display3 = new CaveDisplayTemplate("3");
    public CaveDisplayTemplate display4 = new CaveDisplayTemplate("4");
    public CaveDisplayTemplate display5 = new CaveDisplayTemplate("5");
    public CaveDisplayTemplate display6 = new CaveDisplayTemplate("6");
    public CaveDisplayTemplate display7 = new CaveDisplayTemplate("7");
    public CaveDisplayTemplate display8 = new CaveDisplayTemplate("8");

}