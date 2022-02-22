using UnityEngine;

[CreateAssetMenu(fileName = "New Cave Setup", menuName = "Cave Setup")]
[System.Serializable]
public class CaveSetupTemplate : ScriptableObject{
    public string label = "Untitled";
    public CaveObjectReference rootObjectReference;
    public float rootScale;
    public CaveObjectReference userObjectReference;
    public int numberOfDisplays;
    public Vector2 defaultDisplayDimentions = new Vector2(0.5f,0.5f);
    public CaveDisplayTemplate display1 = new CaveDisplayTemplate("Display 1");
    public CaveDisplayTemplate display2 = new CaveDisplayTemplate("Display 2");
    public CaveDisplayTemplate display3 = new CaveDisplayTemplate("Display 3");
    public CaveDisplayTemplate display4 = new CaveDisplayTemplate("Display 4");
    public CaveDisplayTemplate display5 = new CaveDisplayTemplate("Display 5");
    public CaveDisplayTemplate display6 = new CaveDisplayTemplate("Display 6");
    public CaveDisplayTemplate display7 = new CaveDisplayTemplate("Display 7");
    public CaveDisplayTemplate display8 = new CaveDisplayTemplate("Display 8");

}