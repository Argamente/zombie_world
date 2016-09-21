using UnityEngine;
using System.Collections;
using TreaslandLib.Unity3D.Managers;
using System.Collections.Generic;
using TreaslandLib.Algorithms.AStar;

public class GameWorldManager : MonoBehaviour {
    private static GameWorldManager _instance = null;
    public static GameWorldManager GetInstance()
    {
        if(_instance == null)
        {
            _instance = ConstantHolder.AddComponent<GameWorldManager>();
        }
        return _instance;
    }


    public Dictionary<int,Dictionary<int,object>> GetMap()
    {
        return GameWorldCreator.GetInstance().worldMap;
    }


    private Vector2 CalculateOffset(Vector3 position)
    {
        int offsetX = (int)(position.x / Constants.unitWorldSize);
        int offsetY = (int)(position.y / Constants.unitWorldSize);
        return new Vector2(offsetX, offsetY);
    }

    public Point GetPointFromVector3(Vector3 position)
    {
        int offsetX = (int)(position.x / Constants.unitWorldSize);
        int offsetY = (int)(position.y / Constants.unitWorldSize);
        Point p = new Point(offsetX, offsetY);
        return p;
    }

    public Vector3 GetRandomTargetPos(Vector3 position,int searchRange = 50)
    {
        Vector2 offset = CalculateOffset(position);
        int halfSearchRange = searchRange / 2;
        int startX = (int)offset.x - halfSearchRange;
        int startY = (int)offset.y - halfSearchRange;
        int endX = (int)offset.x + halfSearchRange;
        int endY = (int)offset.y + halfSearchRange;

        List<Vector2> avaliableOffsetList = new List<Vector2>();
        for(int x = startX; x <= endX; ++x)
        {
            for(int y = startY; y <= endY; ++y)
            {
                if (GameWorldCreator.GetInstance().IsAvaliableOffset(x, y))
                {
                    avaliableOffsetList.Add(new Vector2(x, y));
                }
            }
        }

        int count = avaliableOffsetList.Count;
        if(count <= 0)
        {
            return new Vector3(offset.x, offset.y,0);
        }
        else
        {
            int select = Random.Range(0, count);
            return new Vector3(avaliableOffsetList[select].x, avaliableOffsetList[select].y, 0);
        }
    }
	
}
