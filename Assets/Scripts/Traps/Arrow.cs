using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Arrow : MonoBehaviour
{
    public void startSelfDestruction( float time )
    {
        Destroy( this.gameObject, time );
    }

    public void OnCollisionEnter2D( Collision2D coll )
    {
        if ( coll.transform.tag.Equals( "Player" ) )
        {
            coll.transform.GetComponent<PlayerControl>().die();
        }
    }
	
}
