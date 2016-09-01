using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CameraManager : MonoBehaviour
{

    public float smoothTimeX = 1f;
    public float smoothTimeY = 1f;

    private Vector2 _velocity;
    private Vector3 _velocity3;
    private Transform _thisTransform;
    private PlayerControl _player;
    private float _shakeCD;
    private float _shakeAmount;

    public static CameraManager Instance { get; private set; }

    void Awake()
    {
        if ( Instance != null && Instance != this )
        {
            // If that is the case, we destroy other instances
            Destroy( gameObject );
        }
        Instance = this;
        _thisTransform = transform; 
    }

    void Start()
    {
        _player = ( FindObjectOfType( typeof( PlayerControl ) ) as PlayerControl );
    }

    void Update()
    {
        updatePos2();
        if ( _shakeAmount > 0 )
        {
            cameraShake();
        }
    }

    void updatePos()
    {
        float newPosX = Mathf.SmoothDamp( _thisTransform.position.x, _player.transform.position.x, ref _velocity.x, smoothTimeX );
        float newPosY = Mathf.SmoothDamp( _thisTransform.position.y, _player.transform.position.y + 1, ref _velocity.y, smoothTimeY );
        _thisTransform.position = new Vector3( newPosX, newPosY, _thisTransform.position.z );
    }

    void updatePos2()
    {
        Vector3 point = Camera.main.WorldToViewportPoint( _player.transform.position );
        Vector3 delta = _player.transform.position - Camera.main.ViewportToWorldPoint( new Vector3( 0.5f, .3f, point.z ) ); //(new Vector3(0.5, 0.5, point.z));
        Vector3 destination = _thisTransform.position + delta;
        _thisTransform.position = Vector3.SmoothDamp( _thisTransform.position, destination, ref _velocity3, smoothTimeX );
    }


    public void shakeEffect( float shakeAmount, float howLong )
    {
        _shakeCD = howLong;
        _shakeAmount = shakeAmount;
    }

    void cameraShake()
    {
        if ( _shakeCD > 0 )
        {
            _shakeCD -= Time.deltaTime;
            Vector2 shakePos = Random.insideUnitCircle * _shakeAmount;
            _thisTransform.position = new Vector3( _thisTransform.position.x + shakePos.x 
                                                 , _thisTransform.position.y + shakePos.y 
                                                 , _thisTransform.position.z );
        }
        else
        { 
            _shakeAmount = 0;
        }
    }

}
