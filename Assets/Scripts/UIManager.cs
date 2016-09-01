using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.Audio;
using UnityEngine.EventSystems;

public class UIManager : MonoBehaviour
{
    public enum MenuState
    {
        Main,
        Credits,
        Options,
        Pause,
        EndLevel,
        Game
    }


    public MenuState lastState;
    public Canvas mainMenuCanvas;
    public Canvas creditsCanvas;
    public Canvas optionsCanvas;
    public Canvas pauseCanvas;
    public Canvas endLevelCanvas;
    public Canvas endGameCanvas;
    public Canvas prologueCanvas;

    public AudioMixer audioMixer;
    public AudioClip menuMusic;
    public AudioClip gameMusic;

    private const string MASTER_VOLUME = "MasterVolume";
    private const string FX_VOLUME = "FXVolume";
    private const string BG_VOLUME = "BGMusicVolume";

    private float _curTimeScale;
    private bool _canPause;
    private MenuState _curMenuState;

    private AudioSource _bgAudioSource;

    public float MixerMasterVolume
    {
        get
        { 
            float volValue;
            audioMixer.GetFloat( MASTER_VOLUME, out volValue );
            return volValue;
        }

        set { audioMixer.SetFloat( MASTER_VOLUME, value ); }
    }

    public float MixerFXVolume
    {
        get
        { 
            float volValue;
            audioMixer.GetFloat( FX_VOLUME, out volValue );
            return volValue;
        }

        set { audioMixer.SetFloat( FX_VOLUME, value ); }
    }

    public float MixerBGVolume
    {
        get
        { 
            float volValue;
            audioMixer.GetFloat( BG_VOLUME, out volValue );
            return volValue;
        }

        set { audioMixer.SetFloat( BG_VOLUME, value ); }
    }

    public MenuState CurMenuState
    {
        get { return _curMenuState; }
        set
        {
            _curMenuState = value;
            changeMusic();
        }
    }

    public static UIManager Instance { get; private set; }

    void Awake()
    {
        if ( Instance != null && Instance != this )
        {
            Destroy( gameObject );
        }
        Instance = this;

    }

    void Start()
    {
        _bgAudioSource = CameraManager.Instance.transform.FindChild( "BGSounds" ).FindChild( "BGMusic" ).GetComponent<AudioSource>();
        CurMenuState = MenuState.Main;
        _curTimeScale = Time.timeScale;
        disableCanvas();
        setupSoundSlider();
        showMainMenu();
    }

    void Update()
    {
        if ( Input.GetKeyDown( KeyCode.Escape ) )
        {
            if ( _canPause && CurMenuState.Equals( MenuState.Game ) )
            { 
                showPause();
            }
            else
            { 
                back();
            }
        }
    }

    void setupSoundSlider()
    {
        optionsCanvas.transform.FindChild( "sliderMaster" ).GetComponent<Slider>().value = MixerMasterVolume;
        optionsCanvas.transform.FindChild( "sliderBG" ).GetComponent<Slider>().value = MixerBGVolume;
        optionsCanvas.transform.FindChild( "sliderFX" ).GetComponent<Slider>().value = MixerFXVolume;
    }

    public void back()
    {
        disableCanvas();
        switch ( CurMenuState )
        {
            case MenuState.Credits:
                showMainMenu();
                break;

            case MenuState.Options:
                if ( lastState.Equals( MenuState.Main ) )
                    showMainMenu();
                else
                    showPause();
                break;

            case MenuState.Pause:
                backToGame();
                break;
        }
    }

    public void showOptions()
    {
        disableCanvas();

        lastState = CurMenuState;
        CurMenuState = MenuState.Options;
        optionsCanvas.gameObject.SetActive( true );
    }

    public void showCredits()
    {
        disableCanvas();
        CurMenuState = MenuState.Credits;
        creditsCanvas.gameObject.SetActive( true );
    }

    public void showMainMenu()
    {
        disableCanvas();
        CurMenuState = MenuState.Main;
        Time.timeScale = 0;
        _canPause = false;
        mainMenuCanvas.gameObject.SetActive( true );
    }

    public void showPrologue()
    {
        disableCanvas();
        prologueCanvas.gameObject.SetActive( true );
    }


    public void backToMainMenu()
    {
        showMainMenu();
    }

    public void showPause()
    {
        disableCanvas();

        CurMenuState = MenuState.Pause;
        Time.timeScale = 0;
        pauseCanvas.gameObject.SetActive( true );
    }

    public void exitGame()
    {
        Application.Quit();
    }

    public void backToGame()
    {
        disableCanvas();
        Time.timeScale = _curTimeScale;
        CurMenuState = MenuState.Game;
    }

    public void startGame()
    {
        disableCanvas();
        Time.timeScale = _curTimeScale;
        CurMenuState = MenuState.Game;
        GameManager.Instance.startGame();
        _canPause = true;
    }

    public void nextLevel()
    {
        disableCanvas();
        Time.timeScale = _curTimeScale;
        CurMenuState = MenuState.Game;
        GameManager.Instance.nextLevel();
        _canPause = true;
    }

    public void restartLevel()
    {
        disableCanvas();
        Time.timeScale = _curTimeScale;
        CurMenuState = MenuState.Game;
        GameManager.Instance.restartLevel();
        _canPause = true;
    }

    public void endLevel( bool alive )
    {
        disableCanvas();
        Time.timeScale = 0;
        CurMenuState = MenuState.EndLevel;
        _canPause = false;
        endLevelCanvas.transform.FindChild( "btnnext" ).gameObject.SetActive( alive );
        updateEndLevelData( alive );
        endLevelCanvas.gameObject.SetActive( true );
    }

    public void endGame()
    {
        disableCanvas();
        endGameCanvas.gameObject.SetActive( true );
    }

    public void goPrologue()
    {
        disableCanvas();
        prologueCanvas.gameObject.SetActive( false );
    }


    void updateEndLevelData( bool alive )
    {
        string trapsTXT = GameManager.Instance.trapsFound.ToString() + " / " + GameManager.Instance.amountOfTrapsOfLevel.ToString() + " traps";
        trapsTXT += "\n" + GameManager.Instance.torchsEnabled.ToString() + " / " + GameManager.Instance.amountOfTorchs.ToString() + " torchs";
        if ( !alive )
        {
            trapsTXT = "DEAD";
        }
        endLevelCanvas.transform.FindChild( "txttraps" ).GetComponent<Text>().text = trapsTXT;
    }

    void disableCanvas()
    {
        mainMenuCanvas.gameObject.SetActive( false );
        creditsCanvas.gameObject.SetActive( false );
        optionsCanvas.gameObject.SetActive( false );
        pauseCanvas.gameObject.SetActive( false );
        endLevelCanvas.gameObject.SetActive( false );
        endGameCanvas.gameObject.SetActive( false );
        prologueCanvas.gameObject.SetActive( false );
    }

    void changeMusic()
    {
        AudioClip tmpAc = CurMenuState.Equals( MenuState.Game ) ? gameMusic : menuMusic;

        if ( _bgAudioSource.clip != tmpAc )
        {
            _bgAudioSource.Stop();
            _bgAudioSource.clip = tmpAc;
            _bgAudioSource.Play();
        }
    }
}
