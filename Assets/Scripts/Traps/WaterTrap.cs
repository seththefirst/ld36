using UnityEngine;
using System.Collections;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class WaterTrap : Trap
{
    private const int TRAPWIDTH = 6;
    private const int TRAPHEIGHT = 4;

    public bool drawHandler = false;
    public float timeBetweenLevels = 1f;

    private float _timeBetweenLevelsCD;
    private int _curYLevel;
    private FluidLine _curWaterLine;
    private int _amountOfLineToBeCreated;


    public override void fireTrap()
    {        
        base.fireTrap();
        StartCoroutine( spawnTiles() );
    }

    public override void resetTrap()
    {
        base.resetTrap();
  
    }

    public override IEnumerator spawnTiles()
    {
        _isCRSpawnTileRunning = true;
        _curYLevel = ( int )_thisTransform.position.y;
        _amountOfLineToBeCreated = TRAPHEIGHT;

        for ( int line = 1; line <= _amountOfLineToBeCreated; line++ )
        {
            _curWaterLine = spawnWaterLine( _curYLevel );
            while ( _curWaterLine.curLevel < 3 )
            {
                _timeBetweenLevelsCD = timeBetweenLevels;
                while ( _timeBetweenLevelsCD > 0 )
                {
                    _timeBetweenLevelsCD -= Time.deltaTime;
                    yield return null;
                }
                updateWaterLevels( _curWaterLine );

                yield return null;

                if ( line == _amountOfLineToBeCreated
                     && _curWaterLine.curLevel == 2 )
                {
                    break;
                }
            }
            _curYLevel++;
        }
        doneTrap();
        _isCRSpawnTileRunning = false;
    }

    FluidLine spawnWaterLine( float yPos )
    {
        FluidLine waterLine = new FluidLine( -1 );
        for ( int x = ( int )_thisTransform.position.x; x < _thisTransform.position.x + TRAPWIDTH; x++ )
        {
            GameObject newWater = new GameObject( "water" );
            addDieCollider( newWater );
            newWater.transform.parent = _thisTransform;
            _childs.Add( newWater.transform );
            newWater.transform.position = new Vector3( x, yPos );
            SpriteRenderer sprite = newWater.AddComponent<SpriteRenderer>();
            sprite.sortingOrder = 6;
            sprite.sharedMaterial = TrapsManager.Instance.trapsMaterial;
            waterLine.fluidSpots.Add( sprite );
        }
        updateWaterLevels( waterLine );
        return waterLine;
    }

    void updateWaterLevels( FluidLine waterLine )
    {
        if ( !trapFired )
            return;
        
        if ( waterLine.curLevel < 3 )
        {
            waterLine.curLevel++;
            foreach ( SpriteRenderer sprite in waterLine.fluidSpots )
            {
                if ( !trapFired )
                    break;
                
                if ( sprite != null )
                    sprite.sprite = TrapsManager.Instance.waterSprites[ waterLine.curLevel ];
            }
        }
    }

    #if UNITY_EDITOR
    public void OnDrawGizmosSelected()
    {
        if ( drawHandler )
        {
            Handles.DrawSolidRectangleWithOutline( new Rect( transform.position.x - .5f, transform.position.y - .5f, TRAPWIDTH, TRAPHEIGHT ), Color.cyan, Color.blue );
        }
    }
    #endif
	
}