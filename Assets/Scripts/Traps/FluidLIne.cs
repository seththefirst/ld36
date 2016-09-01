using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class FluidLine
{
    public int curLevel;
    public List<SpriteRenderer> fluidSpots;

    public FluidLine( int level )
    {
        curLevel = level;
        fluidSpots = new List<SpriteRenderer>();
    }
}

