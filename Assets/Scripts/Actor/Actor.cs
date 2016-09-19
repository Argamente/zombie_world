using UnityEngine;
using System.Collections;

public class Actor : MonoBehaviour
{

    private Action currAction = null;
    private ActionForIdle idle = null;
    private ActionForWalking walking = null;
    private ActionForRandomWalking randomWalking = null;


    void Awake ()
    {
        this.idle = this.gameObject.AddComponent<ActionForIdle> ();
        this.walking = this.gameObject.AddComponent<ActionForWalking> ();
        this.randomWalking = this.gameObject.AddComponent<ActionForRandomWalking> ();
        this.SetAction (this.randomWalking);
    }


    private void SetAction (Action nextAction)
    {
        if (this.currAction != null && this.currAction == nextAction)
        {
            return;
        }

        if (this.currAction != null)
        {
            this.currAction.StopAction ();
        }
        this.currAction = nextAction;
        if (this.currAction != null)
        {
            this.currAction.StartAction ();
        }
    }


    void Update ()
    {
        if (this.currAction != null)
        {
            this.currAction.OnActionUpdate ();
        }

        if (Input.GetKey (KeyCode.A))
        {
            this.SetAction (walking);
            walking.SetWalkingDirection (enWalkingDir.Left);
        }
        else if (Input.GetKey (KeyCode.D))
        {
            this.SetAction (walking);
            walking.SetWalkingDirection (enWalkingDir.Right);
        }
        else if (Input.GetKey (KeyCode.W))
        {
            this.SetAction (walking);
            walking.SetWalkingDirection (enWalkingDir.Up);
        }
        else if (Input.GetKey (KeyCode.S))
        {
            this.SetAction (walking);
            walking.SetWalkingDirection (enWalkingDir.Down);
        }
        else
        {
            //this.SetAction (idle);
        }

    }



	
}
