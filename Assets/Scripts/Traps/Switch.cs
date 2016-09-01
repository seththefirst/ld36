using UnityEngine;
using System.Collections;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class Switch : MonoBehaviour
{
    public enum SwitchType
    {
        Simultaneously,
        Sequentially

    }

    public SwitchType switchType;
    private float _activationTime = .2f;
    public Trap [] traps;
    public bool showConnections;

    private bool _isSwitchFired;
    private bool _isSwitchActivated;
    private float _activationTimeCD;

    private Animator _animator;
    private Transform _thisTransform;
    private AudioSource _soundFX;
    private bool _isCREnableTrapsRunning;

    void Awake()
    {
        _thisTransform = transform;
        _animator = _thisTransform.GetComponent<Animator>();
        _soundFX = _thisTransform.GetComponent<AudioSource>();
    }

    void Start()
    {
        _isSwitchFired = false;
        _isSwitchActivated = false;
    }

    void Update()
    {
        if ( !_isSwitchFired && _isSwitchActivated )
        {
            if ( _activationTimeCD > 0 )
            {
                _activationTimeCD -= Time.deltaTime;
            }
            else
            {
                fireSwitch();
            }
        }

    }

    void OnTriggerStay2D( Collider2D collision )
    {
        if ( collision.transform.tag.Equals( "Player" ) )
        {
            if ( !_isSwitchActivated )
            {
                activateSwitch();
            }
        }
    }

    public bool disableSwitch()
    {
        bool returnValue = false;

        if ( isSwitchEnabled() )
        {
            _isSwitchActivated = true;
            _isSwitchFired = true;
            _animator.enabled = false;

            returnValue = true;
        }

        return returnValue;
    }

    bool isSwitchEnabled()
    {
        return ( !_isSwitchFired && !_isSwitchActivated );
    }


    void fireSwitch()
    {
        _isSwitchFired = true;
        _animator.enabled = false;
        if ( traps.Length > 0 )
        {
            StartCoroutine( enableTraps() );
        }
    }

    void activateSwitch()
    {
        _activationTimeCD = _activationTime;
        _isSwitchActivated = true;
        _isSwitchFired = false;
        _animator.SetTrigger( "pressSwitch" );
        if ( !_soundFX.isPlaying )
        {
            _soundFX.Play();
        }
    }

    public void resetSwitch()
    {
        _isSwitchActivated = false;
        _animator.enabled = true;
        if ( _isCREnableTrapsRunning )
        {
            StopCoroutine( enableTraps() );
            _isCREnableTrapsRunning = false;
        }
    }

    IEnumerator enableTraps()
    {        
        _isCREnableTrapsRunning = true;
        if ( switchType.Equals( SwitchType.Simultaneously ) )
        {            
            foreach ( Trap trap in traps )
            {
                trap.enableTrap();
                yield return null;
            }
        }
        else
        {
            for ( int itrap = 0; itrap < traps.Length; itrap++ )
            {
                Trap trap = traps[ itrap ];
                trap.enableTrap();
                while ( !trap.trapDone )
                    yield return null;
            }
        }
        _isCREnableTrapsRunning = false;
    }

    #if UNITY_EDITOR
    public void OnDrawGizmosSelected()
    {
        float squareSize = .4f;
        Color squareColor;
        squareColor = ( !_isSwitchFired ? Color.green : Color.red );

        if ( showConnections )
        { 
            Handles.DrawSolidRectangleWithOutline( new Rect( transform.position.x - squareSize, transform.position.y - squareSize, squareSize, squareSize ), squareColor, Color.white );
            if ( traps != null && traps.Length > 0 )
            {
                foreach ( Trap trap in traps )
                {
                    squareColor = ( trap.trapArmed ? Color.green : Color.red );
                    Handles.DrawSolidRectangleWithOutline( new Rect( trap.transform.position.x - squareSize, trap.transform.position.y - squareSize, squareSize, squareSize ), squareColor, Color.white );
                    Gizmos.color = switchType.Equals( SwitchType.Simultaneously ) ? Color.magenta : Color.green;
                    Gizmos.DrawLine( transform.position, trap.transform.position );
                }
            }
        }
    }
    #endif
}
