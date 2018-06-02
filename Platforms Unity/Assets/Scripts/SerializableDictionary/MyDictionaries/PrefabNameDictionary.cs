using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class TileNameDictionary : SerializableDictionary<Tile, string> { }

[Serializable]
public class BlockNameDictionary : SerializableDictionary<Block, string> { }

[Serializable]
public class WallNameDictionary : SerializableDictionary<Wall, string> { }

[Serializable]
public class LogicObjectNameDictionary : SerializableDictionary<LogicObject, string> { }