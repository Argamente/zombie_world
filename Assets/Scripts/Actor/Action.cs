using UnityEngine;
using System.Collections;
using TreaslandLib.Core;

public class Action : MonoBehaviour
{
    public Listener onActionEnd = null;

    private bool _isRunning = false;

    protected Transform currTrans;

    void Awake ()
    {
        this.currTrans = this.gameObject.GetComponent<Transform> ();
    }

    public bool isRunning {
        get
        {
            return this._isRunning;
        }
    }

    public virtual void StartAction ()
    {
        this._isRunning = true;  
    }


    public virtual void StopAction ()
    {
        this._isRunning = false;
        
    }

    public virtual void OnActionUpdate ()
    {
        
    }

	
}
