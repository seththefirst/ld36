using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TrapsManager : MonoBehaviour
{
    public Material trapsMaterial;
    public List<Sprite> wallSprites;
    public List<Sprite> gasSprites;
    public List<Sprite> waterSprites;
    public List<Sprite> snakeSprites;

    public static TrapsManager Instance { get; private set; }

    void Awake()
    {
        if ( Instance != null && Instance != this )
            Destroy( gameObject );
        Instance = this;
    }

}
