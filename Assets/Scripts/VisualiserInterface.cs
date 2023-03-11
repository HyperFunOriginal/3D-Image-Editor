using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public abstract class IVisualisable
{
    public long hashIdentifier;
    public object[] objects;
    public abstract void PrepareForVisualiser();
    public abstract void Finally();
    public abstract void FinishingUp();
    public abstract void StartInVisualiser();
    public abstract void UpdateInVisualiser();
    public abstract void LateUpdateInVisualiser();
}

public class VisualiserInterface : MonoBehaviour
{
    public static VisualiserInterface instance;
    public static bool visualising;
    public List<IVisualisable> componentsToRun;
    internal static Sketch persistent;

    // Start is called before the first frame update
    void Start()
    {
        if (FindObjectsOfType<VisualiserInterface>().Length > 1)
        {
            DestroyImmediate(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
        DontDestroyOnLoad(this);
    }

    private void Update()
    {
        if (visualising && componentsToRun != null)
            for (int i = 0; i < componentsToRun.Count; i++)
                componentsToRun[i].UpdateInVisualiser();
    }
    private void LateUpdate()
    {
        if (visualising && componentsToRun != null)
            for (int i = 0; i < componentsToRun.Count; i++)
                componentsToRun[i].LateUpdateInVisualiser();
    }

    public void ViewVisualiser()
    {
        if (visualising)
            return;

        persistent = Sketch.Yield();
        if (componentsToRun != null)
            for (int i = 0; i < componentsToRun.Count; i++)
                componentsToRun[i].PrepareForVisualiser();
        SceneManager.LoadScene("Visualizer");
        if (componentsToRun != null)
            for (int i = 0; i < componentsToRun.Count; i++)
                componentsToRun[i].StartInVisualiser();
        visualising = true;

        Debug.Log("Entering visualiser!");
    }
    public void LeaveVisualiser()
    {
        if (!visualising)
            return;

        if (componentsToRun != null)
            for (int i = 0; i < componentsToRun.Count; i++)
                componentsToRun[i].FinishingUp();
        SceneManager.LoadScene("EditorMain");
        visualising = false;

        StartCoroutine(LeaveVisualiserLoad());
        Debug.Log("Leaving visualiser! Loading autosaved sketch.");
    }

    IEnumerator LeaveVisualiserLoad()
    {
        yield return persistent.LoadSketch();
        yield return new WaitForEndOfFrame();
        if (componentsToRun != null)
            for (int i = 0; i < componentsToRun.Count; i++)
                componentsToRun[i].Finally();
    }
}
