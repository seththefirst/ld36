using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LevelCreator : MonoBehaviour
{
    public Texture2D levelTexture;
    public Material bgMaterial;
    public List<ColorPrefab> colorPrefabs;
    public Dictionary<Color, Transform []> dictPrefabs;
    public List<Sprite> bgSprites;
    public List<Sprite> decorators;

    private GameObject _mapHolder;
    private GameObject _trapsHolder;
    private GameObject _bgHolder;
    private GameObject _torchHolder;
    private GameObject _switchsHolder;

    private Vector2 _minCorner;
    private Vector2 _maxCorner;

    void Awake()
    {
//        _thisTransform = transform;
    }

    void Start()
    {
        // fillDictPrefabs();
        // loadMap();
    }

    void Update()
    {
    }

    public void loadMap()
    {
        Color [] allPixels = levelTexture.GetPixels();
        int levelWidth = levelTexture.width;
        int levelHeight = levelTexture.height;

        fillDictPrefabs();

//        cleanMap();

        _minCorner = new Vector2( 0, 0 );
        _maxCorner = new Vector2( 0, 0 );

        GameObject level = new GameObject( levelTexture.name );
        _mapHolder = new GameObject( "MapHolder" );
        _mapHolder.transform.SetParent( level.transform );
        _trapsHolder = new GameObject( "TrapsHolder" );
        _trapsHolder.transform.SetParent( level.transform );
        _torchHolder = new GameObject( "TorchHolder" );
        _torchHolder.transform.SetParent( level.transform );
        _bgHolder = new GameObject( "BGHolder" );
        _bgHolder.transform.SetParent( level.transform );
        _switchsHolder = new GameObject( "SwitchsHolder" );
        _switchsHolder.transform.SetParent( level.transform );

        for ( int iwidth = 0; iwidth < levelWidth; iwidth++ )
        {
            for ( int iheight = 0; iheight < levelHeight; iheight++ )
            {
                Color tileColor = allPixels[ ( iheight * levelWidth ) + iwidth ];
                spawnTileAt( tileColor, iwidth, iheight );
                checkMinMax( iwidth, iheight );
            }
        }

        placeBgSprites();
    }

    void checkMinMax( int x, int y )
    {
        _minCorner.x = _minCorner.x > x ? x : _minCorner.x;
        _minCorner.y = _minCorner.y > y ? y : _minCorner.y;
        _maxCorner.x = _maxCorner.x < x ? x : _maxCorner.x;
        _maxCorner.y = _maxCorner.y < y ? y : _maxCorner.y;
    }

    void placeBgSprites()
    {
        Sprite tmpSprt;

        for ( int x = ( int )_minCorner.x; x <= _maxCorner.x; x++ )
        {
            for ( int y = ( int )_minCorner.y; y <= _maxCorner.y; y++ )
            {                
                if ( Random.Range( 0, 101 ) > 95 )
                { 
                    tmpSprt = decorators[ Random.Range( 0, decorators.Count ) ];
                }
                else
                { 
                    tmpSprt = bgSprites[ Random.Range( 0, bgSprites.Count ) ];
                }

                GameObject bg = new GameObject( "bg" );
                bg.transform.position = new Vector3( x, y );
                SpriteRenderer sprt = bg.AddComponent<SpriteRenderer>();
                sprt.sprite = tmpSprt;
                sprt.sortingOrder = -4;
                sprt.sharedMaterial = bgMaterial;
                bg.transform.parent = _bgHolder.transform;
            }
        }
    }

    void spawnTileAt( Color color, int xPos, int yPos )
    {
        if ( color.a == 0 )
        {
            return;
        }

        if ( dictPrefabs.ContainsKey( color ) )
        {
            Transform [] tmpTransforms = dictPrefabs[ color ];
            GameObject tile = tmpTransforms[ Random.Range( 0, tmpTransforms.Length ) ].gameObject;
            GameObject go = ( GameObject )Instantiate( tile, new Vector3( xPos, yPos ), Quaternion.identity );

            if ( tile.name == "SwitchHolder" )
            {
                go.transform.parent = _switchsHolder.transform;
            }
            else if ( go.GetComponent<Trap>() != null )
            {
                go.transform.parent = _trapsHolder.transform;
            }
            else if ( go.GetComponent<Torch>() != null )
            {
                go.transform.parent = _torchHolder.transform;
            }
            else
            {
                go.transform.parent = _mapHolder.transform;
            }
        }
        else
        {
            Debug.LogError( "Color " + color.ToString() + " not found!" );
        }

    }

    void cleanMap()
    {
        DestroyImmediate( _mapHolder );
        DestroyImmediate( _trapsHolder );
        DestroyImmediate( _bgHolder );
    }

    void fillDictPrefabs()
    {
        dictPrefabs = new Dictionary<Color, Transform []>();
        foreach ( ColorPrefab clrP in colorPrefabs )
        {
            dictPrefabs[ clrP.color ] = clrP.transforms;
        }
    }

    public void findColors()
    {
        Color [] allPixels = levelTexture.GetPixels();
        int levelWidth = levelTexture.width;
        int levelHeight = levelTexture.height;

//        colorPrefabs = new List<ColorPrefab>();
        for ( int iwidth = 0; iwidth < levelWidth; iwidth++ )
        {
            for ( int iheight = 0; iheight < levelHeight; iheight++ )
            {
                Color tileColor = allPixels[ ( iheight * levelWidth ) + iwidth ];
                if ( tileColor.a == 1 )
                {
                    if ( !colorPrefabs.Exists( i => i.color.r == tileColor.r && i.color.g == tileColor.g && i.color.b == tileColor.b ) )
                    {
                        colorPrefabs.Add( new ColorPrefab( tileColor ) );
                    }
                }
            }
        }
    }
}

[System.Serializable]
public struct ColorPrefab
{
    public Color color;
    public Transform [] transforms;

    public ColorPrefab( Color32 color )
    {
        this.color = color;
        this.transforms = new Transform[ 0 ];
    }
}
