using UnityEngine;
using System.Collections;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class SnakesTrap : Trap
{
    private const int TRAPWIDTH = 5;
    private const int TRAPHEIGHT = 1;

    public bool drawHandler = false;
    private float _timeBetweenLevels = .2f;

    public float timeBetweenUpdates = 1f;

    private float _timeBetweenUpdatesCd;
    private float _timeBetweenLevelsCD;
    private int _curYLevel;
    private FluidLine _cursnakeLine;
    private int _amountOfLineToBeCreated;

    public override void fireTrap()
    {        
        base.fireTrap();
        StartCoroutine( spawnTiles() );
    }

    public override void Update()
    {
        base.Update();
        if ( trapDone )
        {
            if ( _timeBetweenUpdatesCd > 0 )
            {
                _timeBetweenUpdatesCd -= Time.deltaTime;
            }
            else
            {
                animateSnakes();
                _timeBetweenUpdatesCd = timeBetweenUpdates;
            }
        }
    }

    public override IEnumerator spawnTiles()
    {
        _isCRSpawnTileRunning = true;
        _curYLevel = ( int )_thisTransform.position.y;
        _amountOfLineToBeCreated = TRAPHEIGHT;

        for ( int line = 1; line <= _amountOfLineToBeCreated; line++ )
        {
            _cursnakeLine = spawnsnakeLine( _curYLevel );
            while ( _cursnakeLine.curLevel < 3 )
            {
                _timeBetweenLevelsCD = _timeBetweenLevels;
                while ( _timeBetweenLevelsCD > 0 )
                {
                    _timeBetweenLevelsCD -= Time.deltaTime;
                    yield return null;
                }
                updateSnakeLevels( _cursnakeLine );

                yield return null;

                if ( line == _amountOfLineToBeCreated
                     && _cursnakeLine.curLevel == 2 )
                {
                    break;
                }
            }
            _curYLevel++;
        }
        doneTrap();
        _isCRSpawnTileRunning = false;
    }

    FluidLine spawnsnakeLine( float yPos )
    {
        FluidLine snakeLine = new FluidLine( -1 );
        for ( int x = ( int )_thisTransform.position.x; x < _thisTransform.position.x + TRAPWIDTH; x++ )
        {
            GameObject newsnake = new GameObject( "snake" );
            addDieCollider( newsnake );
            newsnake.transform.parent = _thisTransform;
            _childs.Add( newsnake.transform );
            newsnake.transform.position = new Vector3( x, yPos );
            SpriteRenderer sprite = newsnake.AddComponent<SpriteRenderer>();
            sprite.sortingOrder = 6;
            sprite.sharedMaterial = TrapsManager.Instance.trapsMaterial;
            snakeLine.fluidSpots.Add( sprite );
        }
        updateSnakeLevels( snakeLine );
        return snakeLine;
    }

    void updateSnakeLevels( FluidLine snakeLine )
    {
        if ( !trapFired )
            return;
        
        if ( snakeLine.curLevel < 3 )
        {
            snakeLine.curLevel++;
            foreach ( SpriteRenderer sprite in snakeLine.fluidSpots )
            {
                if ( !trapFired )
                    break;
                if ( sprite != null )
                    sprite.sprite = TrapsManager.Instance.snakeSprites[ snakeLine.curLevel ];
            }
        }
    }

    void animateSnakes()
    {
        foreach ( SpriteRenderer sprite in _cursnakeLine.fluidSpots )
        {
            sprite.sprite = TrapsManager.Instance.snakeSprites[ Random.Range( 0, TrapsManager.Instance.snakeSprites.Count ) ];
        }
    }

    #if UNITY_EDITOR
    public void OnDrawGizmosSelected()
    {
        if ( drawHandler )
        {
            Handles.DrawSolidRectangleWithOutline( new Rect( transform.position.x - .5f, transform.position.y - .5f, TRAPWIDTH, TRAPHEIGHT ), Color.magenta, Color.blue );
        }
    }
    #endif

	
}
