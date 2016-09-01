using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor( typeof( LevelCreator ) )]
public class LevelCreatorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        LevelCreator lvlCreator = ( LevelCreator )target;
        base.OnInspectorGUI();
        if ( GUILayout.Button( "Read Colors" ) )
        {
            lvlCreator.findColors();
        }

        if ( GUILayout.Button( "Create Level" ) )
        {
            lvlCreator.loadMap();
        }
        
    }

}
