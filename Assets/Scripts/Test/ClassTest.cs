using UnityEngine;
using System.Collections;

public class Grid
{
    public int value;
    public Grid parent;
}


public class ClassTest : MonoBehaviour
{

    // Use this for initialization
    void Start ()
    {
        Grid ga = new Grid ();
        ga.value = 10;

        Grid gb = new Grid ();
        gb.value = 20;
        gb.parent = ga;

        Debug.Log ("GA Value:" + ga.value);
        Debug.Log ("GB Value:" + gb.value);

        gb.parent.value = 100;

        Debug.Log ("GA Value:" + ga.value);
        Debug.Log ("GB Value:" + gb.value);

    }
	
    // Update is called once per frame
    void Update ()
    {
	
    }
}
