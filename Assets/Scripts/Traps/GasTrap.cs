using UnityEngine;
using System.Collections;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class GasTrap : Trap
{
    private const int TRAPWIDTH = 6;
    private const int TRAPHEIGHT = 3;

    public bool drawHandler = false;
    private float _timeBetweenLevels = .5f;

    private float _timeBetweenLevelsCD;
    private int _curYLevel;
    private FluidLine _curGasLine;
    private int _amountOfLineToBeCreated;

    public override void fireTrap()
    {        
        base.fireTrap();
        StartCoroutine( spawnTiles() );
    }

    public override IEnumerator spawnTiles()
    {
        _isCRSpawnTileRunning = true;
        _curYLevel = ( int )_thisTransform.position.y;
//        _amountOfLineToBeCreated = ( int )( fromPos.y - toPos.y );
        _amountOfLineToBeCreated = TRAPHEIGHT;

        for ( int line = 1; line <= _amountOfLineToBeCreated; line++ )
        {
            _curGasLine = spawnGasLine( _curYLevel );
            while ( _curGasLine.curLevel < 3 )
            {
                _timeBetweenLevelsCD = _timeBetweenLevels;
                while ( _timeBetweenLevelsCD > 0 )
                {
                    _timeBetweenLevelsCD -= Time.deltaTime;
                    yield return null;
                }
                updateGasLevels( _curGasLine );

                yield return null;

                if ( line == _amountOfLineToBeCreated
                     && _curGasLine.curLevel == 2 )
                {
                    break;
                }
            }
            _curYLevel--;
        }
        doneTrap();
        _isCRSpawnTileRunning = false;
    }

    FluidLine spawnGasLine( float yPos )
    {
        FluidLine gasLine = new FluidLine( -1 );
        for ( int x = ( int )_thisTransform.position.x; x < _thisTransform.position.x + TRAPWIDTH; x++ )
//            for ( int x = ( int )fromPos.x; x <= toPos.x; x++ )
        {
            GameObject newGas = new GameObject( "gas" );
            addDieCollider( newGas );
            newGas.transform.parent = _thisTransform;
            _childs.Add( newGas.transform );
            newGas.transform.position = new Vector3( x, yPos );
            SpriteRenderer sprite = newGas.AddComponent<SpriteRenderer>();
            sprite.sortingOrder = 6;
            sprite.sharedMaterial = TrapsManager.Instance.trapsMaterial;
            gasLine.fluidSpots.Add( sprite );
        }
        updateGasLevels( gasLine );
        return gasLine;
    }

    void updateGasLevels( FluidLine waterLine )
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
                    sprite.sprite = TrapsManager.Instance.gasSprites[ waterLine.curLevel ];
            }
        }
    }

    #if UNITY_EDITOR
    public void OnDrawGizmosSelected()
    {
        if ( drawHandler )
        {
            Handles.DrawSolidRectangleWithOutline( new Rect( ( transform.position.x - .5f ), ( transform.position.y - TRAPHEIGHT + .5f ), TRAPWIDTH, TRAPHEIGHT ), Color.green, Color.blue );
        }
    }
    #endif
    	
}
