using UnityEngine;
using System.Collections;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class PlayerControl : MonoBehaviour
{
    public float moveSpeed = .1f;
    public float jumpForce = 1f;
    public float groundCheckRadius = .5f;
    public LayerMask whatsIsGround;
    public LayerMask whaysIsSwitch;

    private float _defaulAnimSpeed;
    private bool _facingRight;
    private bool _isGrounded;
    private bool _isClimbing;
    private bool _canClimb;
    private float _moveX;
    private float _moveY;


    private Transform _thisTransform;
    private Animator _animator;
    private Rigidbody2D _rigidBody;
    private Transform _groundCheck;
    private AudioSource _footStepAS;
    private AudioSource _jumpAS;
    private AudioSource _playerDieAS;
    private AudioSource _disarmTrapAS;


    private const string ANIM_BEGINJUMP = "beginJump";
    private const string ANIM_GROUNDED = "grounded";
    private const string ANIM_CLIMB = "climb";
    private const string ANIM_HORIZSPEED = "horizontalSpeed";
    private const string ANIM_VERTISPEED = "verticalSpeed";

    void Awake()
    {
        _thisTransform = transform;
        _animator = _thisTransform.GetComponent<Animator>();
        _rigidBody = _thisTransform.GetComponent<Rigidbody2D>();
        _groundCheck = _thisTransform.FindChild( "groundCheck" ).transform;
        _footStepAS = _thisTransform.FindChild( "footstep" ).GetComponent<AudioSource>();
        _jumpAS = _thisTransform.FindChild( "jump" ).GetComponent<AudioSource>();
        _playerDieAS = _thisTransform.FindChild( "die" ).GetComponent<AudioSource>();
        _disarmTrapAS = _thisTransform.FindChild( "disarmtrap" ).GetComponent<AudioSource>();
    }

    void Start()
    {
        _facingRight = true;
        _defaulAnimSpeed = _animator.speed;
    }

    void FixedUpdate()
    {
        _moveX = Input.GetAxis( "Horizontal" );
        _moveY = Input.GetAxis( "Vertical" );

        _animator.SetFloat( ANIM_HORIZSPEED, Mathf.Abs( _moveX ) );
        _rigidBody.velocity = new Vector2( _moveX * moveSpeed, _rigidBody.velocity.y );

        if ( _moveX > 0 && !_facingRight )
        {
            flip();
        }
        else if ( _moveX < 0 && _facingRight )
        {
            flip();
        }

        if ( _canClimb )
        {
            if ( _moveY != 0 )
            {
                _isClimbing = true;
                checkClimb();
                _rigidBody.velocity = new Vector2( _rigidBody.velocity.x, _moveY * moveSpeed );
                _animator.speed = ( Mathf.Abs( _rigidBody.velocity.y ) != 0 ? Mathf.Abs( _rigidBody.velocity.y ) : Mathf.Abs( _rigidBody.velocity.x ) );
                //_rigidBody.gravityScale = 0;
            }
        }
    }

    void Update()
    {
        checkGround();
        checkClimb();

        _animator.SetFloat( ANIM_VERTISPEED, _rigidBody.velocity.y );

        if ( ( _isGrounded || _isClimbing ) && Input.GetKeyDown( KeyCode.Space ) )
        {
            _rigidBody.gravityScale = 1;
            _animator.SetTrigger( ANIM_BEGINJUMP );
            _rigidBody.AddForce( Vector2.up * jumpForce );
            playJumpSound();
        }

        if ( _isGrounded && Input.GetKeyDown( KeyCode.P ) )
        {
            Vector2 point = ( Vector2 )_thisTransform.position + new Vector2( ( _facingRight ? .5f : -.5f ), 0 );

            Debug.DrawLine( _thisTransform.position, point, Color.red );
            if ( Physics2D.OverlapCircle( point, groundCheckRadius, whaysIsSwitch ) )
            {
                Collider2D [] switchs = Physics2D.OverlapCircleAll( point, 1, whaysIsSwitch );
                foreach ( Collider2D coll in switchs )
                {
                    Switch tmpSwitch = coll.GetComponent<Switch>();
                    if ( tmpSwitch != null )
                    {
                        if ( tmpSwitch.disableSwitch() )
                        {
                            playDisarmTrapSound();
                            GameManager.Instance.trapFound();
                        }
                    }
                }
            }
        }

        playFootSound();
    }

    void playFootSound()
    {
        if ( Mathf.Abs( _moveX ) > 0 && _isGrounded )
        {
            if ( !_footStepAS.isPlaying )
            { 
                _footStepAS.Play();
            }
        }
        else
        {
            _footStepAS.Stop();
        }
    }

    void playJumpSound()
    {
        if ( !_jumpAS.isPlaying )
        {
            _jumpAS.Play();
        }
    }

    void playDieSound()
    {
        _playerDieAS.Play();
    }

    void playDisarmTrapSound()
    {
        _disarmTrapAS.Play();
    }

    void flip()
    {
        _facingRight = !_facingRight;
        Vector3 localScale = _thisTransform.localScale;
        localScale.x *= -1;
        _thisTransform.localScale = localScale;
    }

    void checkGround()
    {
        _isGrounded = Physics2D.OverlapCircle( _groundCheck.position, groundCheckRadius, whatsIsGround );
        _animator.SetBool( ANIM_GROUNDED, _isGrounded );
    }

    void checkClimb()
    {
        if ( ( !_canClimb && _isClimbing ) || _isGrounded )
        {
            _isClimbing = false;
        }

        if ( !_isClimbing )
        {
            _rigidBody.gravityScale = 1;
            _animator.speed = _defaulAnimSpeed;

        }

        if ( _canClimb && _isClimbing && !_isGrounded )
        {
            _rigidBody.gravityScale = 0;
        }

        _animator.SetBool( ANIM_CLIMB, _isClimbing );
    }

    public void die()
    {
        playDieSound();
        GameManager.Instance.endLevel( false );
    }

    public void OnTriggerEnter2D( Collider2D collision )
    {
        if ( collision.transform.tag.Equals( "ladder" ) )
        {
            _canClimb = true;
        }
    }

    public void OnTriggerExit2D( Collider2D collision )
    {
        if ( collision.transform.tag.Equals( "ladder" ) )
        {
            _canClimb = false;
        }
    }

    public void stopSounds()
    {
        Debug.Log( "stopsounds" );
        if ( _jumpAS.isPlaying )
            _jumpAS.Stop();
        
        if ( _footStepAS.isPlaying )
            _footStepAS.Stop();
    }
        
    #if UNITY_EDITOR
    public void OnDrawGizmos()
    {
        Handles.Label( transform.position, "IsGrouded: " + _isGrounded.ToString() + "\nCanClimb: " + _canClimb.ToString() + "\nIsClimbing: " + _isClimbing.ToString() );
        if ( _groundCheck != null )
        {
            Handles.color = Color.red;
            Handles.CircleCap( 0, _groundCheck.position, _groundCheck.rotation, groundCheckRadius );
        }
    }
    #endif
}
