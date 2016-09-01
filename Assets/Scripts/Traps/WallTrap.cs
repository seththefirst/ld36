using UnityEngine;
using System.Collections;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class WallTrap : Trap
{
    private const int TRAPWIDTH = 1;
    private const int TRAPHEIGHT = 3;

    public bool drawHandler = false;
    private float _timeBetweenBlocks = .2f;

    private float _timeBetweenBlocksCD;
    private int _amountOfBlocksToBeCreated;
    private int _curYLevel;

    public override void fireTrap()
    {        
        base.fireTrap();
        StartCoroutine( spawWall() );
    }

    IEnumerator spawWall()
    {
        _curYLevel = ( int )_thisTransform.position.y;
        _amountOfBlocksToBeCreated = TRAPHEIGHT;

        for ( int line = 1; line <= _amountOfBlocksToBeCreated; line++ )
        {
            spawnBlock( _curYLevel );
            _timeBetweenBlocksCD = _timeBetweenBlocks;
            while ( _timeBetweenBlocksCD > 0 )
            {
                _timeBetweenBlocksCD -= Time.deltaTime;
                yield return null;
            }

            yield return null;

            _curYLevel++;
        }
        doneTrap();
    }


    void spawnBlock( float yPos )
    {
        for ( int x = ( int )_thisTransform.position.x; x < _thisTransform.position.x + TRAPWIDTH; x++ )
        {            
            GameObject newBlock = new GameObject( "block" );
            newBlock.transform.parent = _thisTransform;
            _childs.Add( newBlock.transform );
            newBlock.transform.position = new Vector3( x, yPos );
            newBlock.AddComponent<BoxCollider2D>();
            SpriteRenderer sprite = newBlock.AddComponent<SpriteRenderer>();
            sprite.sprite = TrapsManager.Instance.wallSprites[ Random.Range( 0, TrapsManager.Instance.wallSprites.Count ) ];
            sprite.sortingOrder = 4;
            sprite.sharedMaterial = TrapsManager.Instance.trapsMaterial;
            CameraManager.Instance.shakeEffect( .01f, .02f );
            playSound();
        }
    }

    #if UNITY_EDITOR
    public void OnDrawGizmosSelected()
    {
        if ( drawHandler )
        {
            Handles.DrawSolidRectangleWithOutline( new Rect( transform.position.x - .5f, transform.position.y - .5f, TRAPWIDTH, TRAPHEIGHT ), Color.gray, Color.blue );
        }
    }
    #endif

}
