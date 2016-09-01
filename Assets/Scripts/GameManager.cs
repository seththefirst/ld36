using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public List<GameObject> levels;
    public int curActiveLevel;
    public Transform player;
    public List<Trap> traps;
    public List<Switch> switchs;
    public List<Torch> torchs;
    public int amountOfTrapsOfLevel;
    public int trapsFound;
    public int amountOfTorchs;
    public int torchsEnabled;

    void Awake()
    {
//        _thisTransform = transform;
        if ( Instance != null && Instance != this )
        {
            Destroy( gameObject );
        }
        Instance = this;

        player = GameObject.FindWithTag( "Player" ).transform;
    }

    void Start()
    {
        findLevels();
    }

    void findLevels()
    {
        Transform tmpLevel;
        GameObject lvlsHolder = GameObject.Find( "LevelsHolder" );
        levels = new List<GameObject>();

        for ( int xlevel = 1; xlevel <= lvlsHolder.transform.childCount; xlevel++ )
        {
            tmpLevel = lvlsHolder.transform.FindChild( "level" + xlevel );
            if ( tmpLevel != null )
            {
                tmpLevel.gameObject.SetActive( false );
                levels.Add( tmpLevel.gameObject );
            }
        }

        curActiveLevel = 0;
    }

    void posPlayer()
    {
        Transform playerSpawnPoint = levels[ curActiveLevel ].transform.FindChild( "MapHolder" ).FindChild( "PlayerStart(Clone)" );
        if ( playerSpawnPoint == null )
        {
            Debug.LogError( levels[ curActiveLevel ].name + " does not have a player spawn point" );
        }
        player.position = playerSpawnPoint.position;
        Camera.main.transform.position = new Vector3( player.position.x, player.position.y + 1, Camera.main.transform.position.z );
        player.gameObject.SetActive( true );
    }

    public void startGame()
    {
        curActiveLevel = 0;
        levels[ curActiveLevel ].SetActive( true );
        restartLevel();
    }

    public void nextLevel()
    {
        levels[ curActiveLevel ].SetActive( false );
        curActiveLevel++;
        if ( curActiveLevel >= levels.Count )
        {
            UIManager.Instance.endGame();
        }
        else
        {
            levels[ curActiveLevel ].SetActive( true );
            restartLevel();
        }
    }

    public void restartLevel()
    {
        posPlayer();
        getItens();
        resetItens();
        loadAmounts();
    }

    public void resetItens()
    {
        switchs.ForEach( i => i.resetSwitch() );
        traps.ForEach( i => i.resetTrap() );
        torchs.ForEach( i => i.resetTorch() );
    }

    public void getItens()
    {
        torchs = ( GameObject.FindObjectsOfType( typeof( Torch ) ) as Torch [] ).ToList();
        switchs = ( GameObject.FindObjectsOfType( typeof( Switch ) ) as Switch [] ).ToList();
        traps = ( GameObject.FindObjectsOfType( typeof( Trap ) ) as Trap [] ).ToList();
    }

    public void loadAmounts()
    {
        amountOfTrapsOfLevel = levels[ curActiveLevel ].transform.FindChild( "SwitchsHolder" ).childCount;
        trapsFound = 0;

        amountOfTorchs = levels[ curActiveLevel ].transform.FindChild( "TorchHolder" ).childCount;
        torchsEnabled = 0;
    }

    public void trapFound()
    {
        trapsFound++;
    }

    public void torchEnabled()
    {
        torchsEnabled++;
    }

    public void endLevel( bool alive )
    {
        player.gameObject.SetActive( false );
        UIManager.Instance.endLevel( alive );
    }

}
