﻿using System;
using System.Xml;
using System.Xml.Serialization;
using System.Collections.Generic;
using UnityEngine;

namespace Serializing {

    [XmlRoot("level")]
    public class LevelData {

        [XmlAttribute("name")] public string name;
        [XmlAttribute("date")] public string time;
        [XmlArray("tiles")] public TileData[] tiles;
        [XmlArray("blocks")] public BlockData[] blocks;
        [XmlArray("portals")] public PortalData[] portals;

        private List<Block> tempBlocks = new List<Block>();

        private LevelData() { }

        public LevelData(Level level) {
            time = DateTime.Today.ToString("dd/M/yy") + " " + DateTime.Now.ToString("HH:mm:ss");
            tiles = ParseToTileData(level.Tiles);
            blocks = ParseToBlockData(tempBlocks);
            portals = ParseToPortalData(level.Walls);
        }

        public TileData[] ParseToTileData(CoordinateTileDictionary dict) {
            TileData[] t = new TileData[dict.Count];
            int i = 0;
            foreach (var pair in dict) {
                Type linkedDataType = TileData.GetLinkedDataType(pair.Value.GetType());
                t[i] = (TileData)Activator.CreateInstance(linkedDataType, new object[] { pair.Value });
                if (pair.Value.occupant != null)
                    tempBlocks.Add(pair.Value.occupant);
                i++;
            }
            return t;
        }

        public BlockData[] ParseToBlockData(List<Block> blocks) {
            BlockData[] b = new BlockData[tempBlocks.Count];
            for(int i = 0; i < tempBlocks.Count; i++) {
                Type linkedDataType = BlockData.GetLinkedDataType(tempBlocks[i].GetType());
                b[i] = (BlockData)Activator.CreateInstance(linkedDataType, new object[] { tempBlocks[i] });
            }
            return b;
        }

        public PortalData[] ParseToPortalData(TileEdgeWallDictionary dict) {
            PortalData[] t = new PortalData[dict.Count];
            int i = 0;
            foreach (var pair in dict) {
                t[i] = new PortalData(pair.Value as Portal);
                i++;
            }
            return t;
        }
    }
}