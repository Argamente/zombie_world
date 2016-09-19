using UnityEngine;
using System.Collections;

public class ActionForWalking : Action
{
    public float speed = 1.0f;
 
    private enWalkingDir dir;

    public void SetWalkingDirection (enWalkingDir _dir)
    {
        this.dir = _dir;
    }

    public override void OnActionUpdate ()
    {
        base.OnActionUpdate ();

        float deltaMove = speed * Time.deltaTime;

        switch (this.dir)
        {
            case enWalkingDir.Left:
                {
                    this.currTrans.position -= new Vector3 (deltaMove, 0, 0);
                    this.currTrans.localScale = new Vector3 (1, 1, 1);
                }
                break;
            case enWalkingDir.Right:
                {
                    this.currTrans.position += new Vector3 (deltaMove, 0, 0);
                    this.currTrans.localScale = new Vector3 (-1, 1, 1);
                }
                break;
            case enWalkingDir.Up:
                {
                    this.currTrans.position += new Vector3 (0, deltaMove, 0);
                    
                }
                break;
            case enWalkingDir.Down:
                {
                    this.currTrans.position -= new Vector3 (0, deltaMove, 0);
                    
                }
                break;
        }

    }

	
}
