using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TreaslandLib.Algorithms.AStar;

public class Zombie : MonoBehaviour {
    private Action currAction = null;
    private ActionForSpecificWay walking = null;
    private ActionForIdle idle = null;

    private bool isSearchingPath = false;

    void Awake()
    {
        this.walking = this.gameObject.AddComponent<ActionForSpecificWay>();
        this.walking.onActionEnd = this.OnWalkingEnd;
        this.idle = this.gameObject.AddComponent<ActionForIdle>();
    }

    private void SetAction(Action newAction)
    {
        if(this.currAction != null && this.currAction == newAction)
        {
            return;
        }

        if(this.currAction != null)
        {
            this.currAction.StopAction();
        }
        this.currAction = newAction;
        if(this.currAction != null)
        {
            this.currAction.StartAction();
        }

    }


    void Update()
    {
        if(this.currAction != null)
        {
            this.currAction.OnActionUpdate();
        }

        // Trigger path finding
        if (this.currAction != walking && !isSearchingPath)
        {
            this.isSearchingPath = true;
            Vector3 targetPos = GameWorldManager.GetInstance().GetRandomTargetPos(this.transform.position);
            AStarAgent.GetInstance().AddPathSearchTask(this.transform.position, targetPos, this.OnAStarFoundWay);
        }
    }


    void OnAStarFoundWay(List<Point> way)
    {
        this.walking.way = way;
        SetAction(this.walking);
        this.isSearchingPath = false;
    }

    void OnWalkingEnd()
    {
        SetAction(idle);
    }
	
}
