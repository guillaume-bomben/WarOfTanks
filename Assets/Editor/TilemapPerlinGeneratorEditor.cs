using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using WarOfTanks.MapGen;

[CustomEditor(typeof(TilemapPerlinGenerator))]
public class TilemapPerlinGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        TilemapPerlinGenerator mapGen = (TilemapPerlinGenerator)target;

        if (DrawDefaultInspector() && mapGen.autoUpdate)
        {
            mapGen.GenerateMap();
        }

        if (GUILayout.Button("Generate Terrain"))
        {                 
            mapGen.GenerateMap();
        }
    }
}
