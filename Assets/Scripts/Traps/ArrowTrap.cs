using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ArrowTrap : Trap
{
    public GameObject arrowPrefab;
    public float arrowLaunchStrenght = 5f;
    public bool shootLeft;

    private List<Transform> _arrowSpawnPoints;

    public override void Awake()
    {
        base.Awake();
        _arrowSpawnPoints = new List<Transform>();
        if ( arrowPrefab == null )
        {
            Debug.Log( "Arror trap without arrow at : " + _thisTransform.position );
        }
    }

    public override void Start()
    {
        base.Start();
        foreach ( Transform child in _thisTransform )
        {
            if ( child.name.Equals( "arrowSpawnPoint" ) )
            {
                _arrowSpawnPoints.Add( child );
            }                
        }
    }

    public override void fireTrap()
    {        
        base.fireTrap();
        Debug.Log( "firetrap fired " );
        StartCoroutine( fireArrows() );
    }

    IEnumerator fireArrows()
    {
        foreach ( Transform arrowSpot in _arrowSpawnPoints )
        {
            launchArrow( arrowSpot.position );
            yield return new WaitForSeconds( Random.Range( .1f, .2f ) );
        }
        trapDone = true;
    }

    void launchArrow( Vector3 fromSpot )
    {
        GameObject go = ( GameObject )Instantiate( arrowPrefab, fromSpot, Quaternion.identity );
        go.transform.GetComponent<Arrow>().startSelfDestruction( 1f );
        go.transform.GetComponent<Rigidbody2D>().AddRelativeForce( ( shootLeft ? Vector2.left : Vector2.right ) * arrowLaunchStrenght, ForceMode2D.Impulse );
        playSound();
    }

}
