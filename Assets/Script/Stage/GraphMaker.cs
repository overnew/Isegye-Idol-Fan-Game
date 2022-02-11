using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using UnityEngine;

public class GraphMaker
{
    const string graphDataBasePath = "DataBase";
    const string graphGraphPath = "StageGraph";
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
        string path = Path.Combine(Application.dataPath, graphDataBasePath, graphGraphPath, graphType);
        string jsonData = File.ReadAllText(path);
        edgeData = JsonUtility.FromJson<EdgeData>(jsonData);
    }
    private void MakeGraph()
    {
        for (int idx=0; idx <edgeData.edges.Length ; ++idx)
        {
            graph.Add(new Node(edgeData.edges[idx]));
        }
    }
}

public class Node
{
    const int NODE_NUM_IDX = 0;
    const int ADJACENT_IDX = 1;
    private int nodeNumber;
    private List<int> nextNodes = new List<int>();

    public Node(string adjacentEdges)
    {
        string[] splitted = adjacentEdges.Split(':');
        this.nodeNumber = Int32.Parse(splitted[NODE_NUM_IDX]);

        if (adjacentEdges.Equals(""))
            return;

        List<string> tempList = new List<string>();
        tempList.AddRange(splitted[ADJACENT_IDX].Split(','));

        for (int i=0; i<tempList.Count; ++i)    
        {
            if(!tempList[i].Equals("")) //빈 노드 확인
                nextNodes.Add(Int32.Parse(tempList[i]));
        }
    }

    public List<int> GetNextNodes() { return this.nextNodes; }
}