
#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class DialogueEditor : EditorWindow
{

    private DialogueGraphView graphView;

    [MenuItem("Dialogue/Dialogue Graph")]
    public static void OpenGraph()
    {
        var window = GetWindow<DialogueEditor>();
        window.titleContent = new GUIContent("Dialogue Graph");
    }

    private void OnEnable()
    {
        ConstructGraphView();
        ConstructToolbar();
        RequestDataOperation(false);
    }

    private void ConstructGraphView()
    {
        graphView = new DialogueGraphView
        {
            name = "Dialogue Graph"
        };

        graphView.StretchToParentSize();
        rootVisualElement.Add(graphView);
    }

    private void ConstructToolbar()
    {
        var toolbar = new Toolbar();

        toolbar.Add(new Button(() => RequestDataOperation(true)) { text = "Save Graph" });

        rootVisualElement.Add(toolbar);
    }

    private void RequestDataOperation(bool save)
    {
        SaveAndLoadUtility _instance = SaveAndLoadUtility.GetInstance(graphView);
        if (save)
        {
            _instance.Save();
        }
        else
        {
            _instance.Load();
        }
    }
}
#endif
