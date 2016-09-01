using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Liquid : MonoBehaviour
{
    public void OnTriggerStay2D( Collider2D coll )
    {
        if ( coll.transform.tag.Equals( "Player" ) )
        {
            coll.transform.GetComponent<PlayerControl>().die();
        }
    }
	
}
