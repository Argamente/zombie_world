using UnityEngine;
using System.Collections;

public class ActionForRandomWalking : Action
{

    private Vector3 destination;
    private Vector3 dir;
    private float speed = 1;
    private float randDistance = 5;


    public override void StartAction ()
    {
        base.StartAction ();
        this.GenerateRandomDestination ();
        this.GenerateRandomDestination ();
    }


    private void GenerateRandomDestination ()
    {
        Random.seed = Random.Range (0, 1000000);
        int degrees = Random.Range (0, 361);
        float radians = degrees * Mathf.Deg2Rad;
        float offsetX = randDistance * Mathf.Cos (radians);
        float offsetY = randDistance * Mathf.Sin (radians);
        this.destination = new Vector3 (offsetX, offsetY, 0);
        this.dir = (this.destination - this.currTrans.position).normalized;
        Debug.Log ("Destination:" + this.destination + "   " + this.dir);
    }

    private bool IsArrivedDestination ()
    {
        if (Vector3.Distance (this.currTrans.position, this.destination) < 0.1f)
        {
            return true;
        }
        else
        {
            return false;
        }
    }


    public override void OnActionUpdate ()
    {
        base.OnActionUpdate ();

        if (IsArrivedDestination ())
        {
            GenerateRandomDestination ();
        }

        float deltaMove = speed * Time.deltaTime;
        this.currTrans.position += dir * deltaMove;
        //Debug.Log (this.currTrans.position);
    }



}
