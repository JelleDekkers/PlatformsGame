using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(StringStringDictionary))]
[CustomPropertyDrawer(typeof(ObjectColorDictionary))]
[CustomPropertyDrawer(typeof(CoordinateTileDictionary))]
[CustomPropertyDrawer(typeof(TileEdgeWallDictionary))]
[CustomPropertyDrawer(typeof(TileNameDictionary))]
[CustomPropertyDrawer(typeof(BlockNameDictionary))]
[CustomPropertyDrawer(typeof(WallNameDictionary))]
public class AnySerializableDictionaryPropertyDrawer : SerializableDictionaryPropertyDrawer {}
