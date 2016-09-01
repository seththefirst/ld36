using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Torch : MonoBehaviour
{
    public Transform torchSpot;
    public float torchMinValue = 20f;
    public float torchMaxValue = 30f;

    public float maxTimeToDiminish = 2f;
    public float minTimeToDiminish = 4f;
    public int minAmountOfDiminishing = 15;
    public int maxAmountOfDiminishing = 10;

    public float curTorchMinValue;
    public float curTorchMaxValue;
    public bool torchEnabled;

    private int _amountOfDiminishing;
    private int _diminishingCount;

    private float _timeToDiminish;
    private float _timeToDiminishCD;

    private float random;

    private Light _light;
    private AudioSource _soundFX;
    public ParticleSystem torchParticles;

    private Transform _thisTransform;

    void Awake()
    {
        _thisTransform = transform;
        _light = _thisTransform.FindChild( "Light" ).GetComponent<Light>();
        torchParticles = _thisTransform.FindChild( "torchParticles" ).GetComponent<ParticleSystem>();
        _thisTransform.tag = "torch";
        if ( torchSpot != null )
        {
            _soundFX = _thisTransform.FindChild( "soundFX" ).GetComponent<AudioSource>();
        }
            
    }

    void Start()
    {
        random = Random.Range( 0f, 65535.0f );
        _amountOfDiminishing = Random.Range( minAmountOfDiminishing, maxAmountOfDiminishing );
        _timeToDiminish = Random.Range( minTimeToDiminish, maxTimeToDiminish );
        if ( torchSpot != null )
        {
            enableTorch();
        }
        else
        {
            disableTorch();
        }

    }

    void Update()
    {
        if ( torchSpot != null )
        {
            _thisTransform.position = new Vector3( torchSpot.position.x, torchSpot.position.y, _thisTransform.position.z );
        }
        float noise = Mathf.PerlinNoise( random, Time.time );
        _light.spotAngle = Mathf.Lerp( curTorchMinValue, curTorchMaxValue, noise );
        updateDiminishing();
    }

    void updateDiminishing()
    {
        if ( _diminishingCount > 0 )
        {
            _timeToDiminishCD -= Time.deltaTime;
            if ( _timeToDiminishCD <= 0 )
            {
                _diminishingCount--;
                _timeToDiminishCD = _timeToDiminish;
                curTorchMinValue--;
                curTorchMaxValue--;
                torchParticles.startLifetime -= .16f;
            }
            if ( _diminishingCount == 0 )
            {
                disableTorch();
            }
        }
    }

    float PingPong( float aValue, float aMin, float aMax )
    {
        return Mathf.PingPong( aValue, aMax - aMin ) + aMin;
    }

    void disableTorch()
    {
        _light.enabled = false;
        torchEnabled = false;
        torchParticles.gameObject.SetActive( false );
        torchParticles.Stop();
    }

    public void resetTorch()
    {
        enableTorch();
        if ( torchSpot == null )
        {            
            disableTorch();
        }
    }

    void enableTorch()
    {
        curTorchMinValue = torchMinValue;
        curTorchMaxValue = torchMaxValue;
        _diminishingCount = _amountOfDiminishing;
        _timeToDiminishCD = _timeToDiminish;
        _light.enabled = true;
        torchEnabled = true;
        torchParticles.gameObject.SetActive( true );
        torchParticles.Play();
        torchParticles.startLifetime = 2f;

    }

    public void playSoundFX()
    {
        if ( !_soundFX.isPlaying )
        {
            _soundFX.Play();
        }
    }

    public void OnTriggerEnter2D( Collider2D collision )
    {
        if ( collision.tag.Equals( "torch" ) )
        {
            Torch tmpTorch = collision.GetComponent<Torch>();
            if ( tmpTorch.torchEnabled )
            {
                if ( !this.torchEnabled )
                {
                    enableTorch();
                    if ( this.torchSpot != null )
                    {
                        this.playSoundFX();
                    }
                    else
                    {
                        tmpTorch.playSoundFX();
                    }
                    GameManager.Instance.torchEnabled();
                }
                if ( tmpTorch.curTorchMinValue > this.curTorchMinValue )
                {
                    this.curTorchMinValue = tmpTorch.curTorchMinValue;
                    this.curTorchMaxValue = tmpTorch.curTorchMaxValue;
                    this.torchParticles.startLifetime = tmpTorch.torchParticles.startLifetime;
                }
                else
                {
                    tmpTorch.curTorchMinValue = this.curTorchMinValue;
                    tmpTorch.curTorchMaxValue = this.curTorchMaxValue;
                    tmpTorch.torchParticles.startLifetime = this.torchParticles.startLifetime;
                }
            }
        }
    }
}
