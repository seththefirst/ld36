using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Trap : MonoBehaviour
{
    public  bool trapArmed;
    public  bool trapFired;
    public  bool trapDone;
    protected  float _timeToActivate = .2f;
    public ParticleSystem fxParticle;

    protected List<Transform> _fxSpots;
    protected float _timeToActivateCD;
    protected Transform _thisTransform;
    protected AudioSource _audioFX;
    protected List<Transform> _childs;
    protected bool _isCRSpawnTileRunning;

    public virtual void Awake()
    {
        _thisTransform = transform;
        _audioFX = _thisTransform.GetComponent<AudioSource>();
    }

    public virtual void Start()
    {
        _fxSpots = new List<Transform>();
        foreach ( Transform child in _thisTransform )
        {
            if ( child.name == "fxparticles" )
            {
                _fxSpots.Add( child );
            }
        }
        resetTrap();
    }

    public virtual void Update()
    {
        if ( _timeToActivateCD > 0 )
        {
            _timeToActivateCD -= Time.deltaTime;
        }
        else if ( !trapFired )
        {
            fireTrap();
        }
    }

    public virtual void enableTrap()
    {
        trapFired = false;
        trapArmed = false;
        _timeToActivateCD = _timeToActivate;
        playFX();
    }

    public virtual void resetTrap()
    {        
        if ( _childs != null && _childs.Count > 0 )
        {
            _childs.ForEach( child => Destroy( child.gameObject ) );
        }
        _childs = new List<Transform>();
        trapArmed = true;
        trapFired = true;
        trapDone = false;
        if ( _isCRSpawnTileRunning )
        { 
            StopCoroutine( spawnTiles() );
            _isCRSpawnTileRunning = false;
        }
        playFX( true );
    }

    public virtual IEnumerator spawnTiles()
    {
        yield return null;
    }

    public virtual void addDieCollider( GameObject liquid )
    {
        liquid.AddComponent<Liquid>();
        BoxCollider2D boxColl = liquid.AddComponent<BoxCollider2D>();
        boxColl.isTrigger = true;
    }

    public virtual void doneTrap()
    {
        trapDone = true;
        playFX( true );
    }

    public virtual void fireTrap()
    {
        trapFired = true;
    }

    public virtual void playSound()
    {
        if ( _audioFX != null && !_audioFX.isPlaying )
        {
            _audioFX.Play();
        }
    }

    void playFX( bool stop = false )
    {
        if ( fxParticle != null && _fxSpots != null && _fxSpots.Count > 0 )
        {
            if ( stop )
            {
                var children = new List<GameObject>();
                foreach ( Transform fxSpot in _fxSpots )
                {
                    AudioSource tmpAs = fxSpot.GetComponent<AudioSource>();
                    if ( tmpAs != null )
                    {
                        tmpAs.Stop();
                    }
                    foreach ( Transform fx in fxSpot )
                    {
                        children.Add( fx.gameObject );
                    }
                }
                children.ForEach( child => Destroy( child ) );
            }
            else
            {
                foreach ( Transform fxSpot in _fxSpots )
                {
                    ParticleSystem part = ( ParticleSystem )Instantiate( fxParticle, fxSpot.position, fxSpot.rotation );
                    part.transform.parent = fxSpot;
                    part.Play();
                    AudioSource tmpAs = fxSpot.GetComponent<AudioSource>();
                    if ( tmpAs != null && !tmpAs.isPlaying )
                    {
                        tmpAs.Play();
                    }
                }
            }
        }
    }
        

}
