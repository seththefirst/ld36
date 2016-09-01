using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EndLevel : MonoBehaviour
{
    
    void OnTriggerStay2D( Collider2D collision )
    {
        if ( collision.transform.tag.Equals( "Player" ) )
        {
            GameManager.Instance.endLevel( true );
        }
    }

	
}
