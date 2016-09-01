using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Rock : MonoBehaviour
{
    private Transform _thisTransform;
    private Rigidbody2D _rb2d;

    void Awake()
    {
        _thisTransform = transform;
        _rb2d = _thisTransform.GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        _rb2d.AddForce( Vector2.left * 50f, ForceMode2D.Force );
    }

    void Update()
    {
        Vector2 dir = _rb2d.velocity.normalized;
        float angle = Mathf.Atan2( dir.y, dir.x ) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis( angle, Vector3.forward );
    }

    void OnCollisionEnter2D( Collision2D colInfo )
    {
        if ( !colInfo.transform.tag.Equals( "Player" ) )
        {
            _rb2d.AddTorque( _rb2d.velocity.y * 10 ); 
            CameraManager.Instance.shakeEffect( .02f, .02f );
        }
    }


	
}
