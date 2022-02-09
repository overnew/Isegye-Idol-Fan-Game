using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using UnityEngine;

public class GraphMaker : MonoBehaviour
{
    const string graphDataPath = "DataBase/StageGraph";
    private EdgeData edgeData;
    private List<Node> graph = new List<Node>();

    public List<Node> GetLineGraph(string graphType)
    {
        LoadGraphDataFromJson(graphType);
        MakeGraph();
        return graph;
    }

    private void LoadGraphDataFromJson(string graphType)
    {
        string path = Path.Combine(Application.dataPath, graphDataPath, graphType);
        string jsonData = File.ReadAllText(path);
        edgeData = JsonUtility.FromJson<EdgeData>(jsonData);
    }
    private void MakeGraph()
    {
        for (int idx=0; idx <edgeData.edges.Length ; ++idx)
        {
            graph.Add(new Node(idx, edgeData.edges[idx]));
        }
    }
}

public class Node
{
    private int nodeNumber;
    private List<int> nextNodes = new List<int>();

    public Node(int _nodeNumber, string adjacentEdges)
    {
        this.nodeNumber = _nodeNumber;

        List<string> tempList = new List<string>();
        tempList.AddRange(adjacentEdges.Split('.'));

        for (int i=0; i<tempList.Count ; ++i)
            nextNodes.Add(Int32.Parse(tempList[i]));
    }

}