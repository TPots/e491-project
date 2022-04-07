using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

[CustomEditor(typeof(CaveSetupTemplate))]
public class CaveInspector : Editor
{

    public override VisualElement CreateInspectorGUI()
    {
    // Create a new VisualElement to be the root of our inspector UI
    VisualElement myInspector = new VisualElement();

    // Add a simple label
    myInspector.Add(new Label("This is a custom inspector"));

    // Load and clone a visual tree from UXML
    VisualTreeAsset visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/cave.uxml");
    visualTree.CloneTree(myInspector);

    // Return the finished inspector UI
    return myInspector;
    }

}