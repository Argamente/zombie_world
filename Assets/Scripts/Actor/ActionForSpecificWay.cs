using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TreaslandLib.Algorithms.AStar;
using TreaslandLib.Core;

public class ActionForSpecificWay : Action {

    public List<Point> way = null;
    private float speed = 1.0f;
    private Vector3 nextPos = Vector3.zero;
    private float arrivedValue = 0.2f;
    private int wayPointIndex = 0;
    private bool isFinishedWay = false;



    public override void StartAction()
    {
        base.StartAction();
        this.wayPointIndex = 0;
        this.isFinishedWay = false;

        if(this.way != null && this.wayPointIndex < this.way.Count)
        {
            Point point = this.way[this.wayPointIndex];
            nextPos = ConstructureWorldPosFromWayPoint(point);
        }
        else
        {
            this.isFinishedWay = true;
            if(this.onActionEnd != null)
            {
                this.onActionEnd();
            }
        }

    }

    public override void StopAction()
    {
        base.StopAction();
    }

    public override void OnActionUpdate()
    {
        base.OnActionUpdate();

        if (isFinishedWay)
        {
            return;
        }
        if (IsArrivedPos(nextPos))
        {
            ++this.wayPointIndex;
            if(this.wayPointIndex < this.way.Count)
            {
                Point point = this.way[this.wayPointIndex];
                nextPos = ConstructureWorldPosFromWayPoint(point);
            }
            else
            {
                this.isFinishedWay = true;
                if(this.onActionEnd != null)
                {
                    this.onActionEnd();
                }
            }
        }
        else
        {
            Vector3 dir = (nextPos - this.currTrans.position).normalized;
            Vector3 delteMove = dir * speed * Time.deltaTime;
            this.currTrans.position += delteMove;
        }


    }

    private bool IsArrivedPos(Vector3 pos)
    {
        float diffX = Mathf.Abs(pos.x - this.currTrans.position.x);
        float diffY = Mathf.Abs(pos.y - this.currTrans.position.y);
        if(Mathf.Abs(pos.x - this.currTrans.position.x) <= arrivedValue &&
            Mathf.Abs(pos.y - this.currTrans.position.y) <= arrivedValue)
        {
            return true;
        }
        return false;
    }


    private Vector3 ConstructureWorldPosFromWayPoint(Point point)
    {
        float x = point.x * Constants.unitWorldSize;
        float y = point.y * Constants.unitWorldSize;
        Vector3 vec = new Vector3(x, y, 0);
        return vec;
    }

    private void DisplayWay()
    {
        if(this.way != null)
        {
            Debug.Log("===========================");
            for(int i = 0; i < this.way.Count; ++i)
            {
                Debug.Log(this.way[i].x + "  " + this.way[i].y);
            }
            Debug.Log("===========================");
        }
    }

}
