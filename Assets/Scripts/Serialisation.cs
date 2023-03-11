using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

[System.Serializable]
public struct NodeData
{
    public string nodeClassName;
    public long hashCode;
    public string[] recordedValues;
    public long[] inputs;
    public Vector3 position;

    public NodeData(BaseNode node)
    {
        nodeClassName = node.GetType().AssemblyQualifiedName;
        hashCode = node.hashCode;
        recordedValues = new string[node.fields.Count];
        for (int i = 0; i < recordedValues.Length; i++)
            recordedValues[i] = node.Read(node.fields[i]);
        inputs = new long[node.inputs.Count];
        for (int i = 0; i < node.inputs.Count; i++)
            inputs[i] = (node.inputs[i] == null) ? -1 : node.inputs[i].hashCode;
        position = node.transform.position;
    }
}

[System.Serializable]
public struct NodeConnection
{
    public long originHash;
    public string target;
    public long inFinalHash;

    public NodeConnection(FollowCursor node)
    {
        originHash = node.hashCode;
        target = node.followName;
        if (node.followName != "" && node.followName != "cursor")
            inFinalHash = GameObject.Find(node.followName).GetComponent<NodeInput>().hashCode;
        else
            inFinalHash = -1;
    }
}

[System.Serializable]
public struct Sketch
{
    public NodeData[] nodes;
    public NodeConnection[] connections;
    public float canvasZoom;

    public static Sketch Load(string path) => JsonUtility.FromJson<Sketch>(File.ReadAllText(path));
    public static void Save(string path, Sketch s) => File.WriteAllText(path, JsonUtility.ToJson(s, false));
    public IEnumerator LoadSketch()
    {
        while (CanvasZoom.instance == null)
            yield return new WaitForEndOfFrame();

        BaseNode[] sceneNodes = Object.FindObjectsOfType<BaseNode>();

        for (int i = 0; i < sceneNodes.Length; i++)
            Object.Destroy(sceneNodes[i].gameObject);

        CanvasZoom.zoom = canvasZoom;
        yield return new WaitForSeconds(.1f);

        for (int i = 0; i < nodes.Length; i++)
            BaseNode.Instantiate(nodes[i]);

        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();

        FollowCursor[] nodeConn = Object.FindObjectsOfType<FollowCursor>();
        for (int i = 0; i < nodeConn.Length; i++)
            for (int j = 0; j < connections.Length; j++)
            {
                if (connections[j].inFinalHash == -1)
                    continue;
                if (nodeConn[i].hashCode == connections[j].originHash)
                {
                    nodeConn[i].followName = connections[j].target;
                    NodeInput n = GameObject.Find(connections[j].target).GetComponent<NodeInput>();
                    n.reference.inputs[n.index] = nodeConn[i].reference;
                    break;
                }
            }
    }
    public static Sketch Yield()
    {
        Sketch result = new Sketch();
        BaseNode[] sceneNodes = Object.FindObjectsOfType<BaseNode>();
        FollowCursor[] nodeConn = Object.FindObjectsOfType<FollowCursor>();
        result.nodes = new NodeData[sceneNodes.Length];
        result.connections = new NodeConnection[nodeConn.Length];
        result.canvasZoom = CanvasZoom.zoom;

        for (int i = 0; i < result.nodes.Length; i++)
            result.nodes[i] = new NodeData(sceneNodes[i]);
        for (int i = 0; i < result.connections.Length; i++)
            result.connections[i] = new NodeConnection(nodeConn[i]);
        return result;
    }
}