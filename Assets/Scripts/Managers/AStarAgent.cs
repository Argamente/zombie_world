using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TreaslandLib.Core;
using TreaslandLib.Algorithms.AStar;
using TreaslandLib.Unity3D.Managers;

public class AStarAgent : MonoBehaviour {
    private static AStarAgent _instance = null;
    public static AStarAgent GetInstance()
    {
        if (_instance == null)
        {
            _instance = ConstantHolder.AddComponent<AStarAgent>();
        }
        return _instance;
    }

	
    public void AddPathSearchTask(Vector3 startPos,Vector3 targetPos,Listener<List<Point>> onWayFound)
    {
        Point startP = GameWorldManager.GetInstance().GetPointFromVector3(startPos);
        Point goalP = GameWorldManager.GetInstance().GetPointFromVector3(targetPos);
        AStar astar = new AStar();

        astar.Init(
            GameWorldManager.GetInstance().GetMap(),
            startP,
            goalP,
            null,
            enWayStyle.BaseDirection,
            onWayFound);

        StartCoroutine(astar.CalculatePath());
    }
}
