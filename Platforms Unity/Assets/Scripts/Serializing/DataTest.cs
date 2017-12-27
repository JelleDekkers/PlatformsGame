//using System;
//using UnityEngine;
//using UnityEditor;
//using System.Collections.Generic;

//namespace Testing {

//    [DataLink(typeof(TileDataContainerBase))]
//    public class TileBehaviourBase {

//        public DataLinkAttribute[] GetAttributes() {
//            return (DataLinkAttribute[])GetType().GetCustomAttributes(typeof(DataLinkAttribute), true);
//        }

//        public Type GetContainerType() {
//            return GetAttributes()[0].DataContainerType;
//        }
//    }

//    [DataLink(typeof(TileLaserDataContainer))]
//    public class TileLaser : TileBehaviourBase {

//    }

//    [DataLink(typeof(TilePlayerContainer))]
//    public class TilePlayer : TileBehaviourBase {

//    }

//    public class TileDataContainerBase {
//        public static Dictionary<Type, Type> dataLinks = new Dictionary<Type, Type>() {
//            {typeof(TileBehaviourBase), typeof(TileDataContainerBase) },
//            {typeof(TileLaser), typeof(TileLaserDataContainer) },
//            {typeof(TilePlayer), typeof(TilePlayerContainer) }
//        };

//        public TileDataContainerBase() {
//            Debug.Log("ctor for TileDataContainerBase");
//        }
//    }

//    public class TileLaserDataContainer : TileDataContainerBase {
//        public TileLaserDataContainer() {
//            Debug.Log("ctor for TileLaserDataContainer");
//        }
//    }

//    public class TilePlayerContainer : TileDataContainerBase {
//        public TilePlayerContainer() {
//            Debug.Log("ctor for TilePlayerContainer");
//        }
//    }

//    [AttributeUsage(AttributeTargets.Class)]
//    public class DataLinkAttribute : Attribute {
//        public Type DataContainerType { get; private set; }

//        public DataLinkAttribute(Type dataContainerType) {
//            DataContainerType = dataContainerType;
//        }
//    }

//    [InitializeOnLoad]
//    public static class Usage {
//        static Usage() {
            
//            // kiezen
//            TileLaser laserTile= new TileLaser();
//            Type attribute = laserTile.GetContainerType();
//            Type linkedDataContainer = TileDataContainerBase.dataLinks[laserTile.GetType()];
//            TileLaserDataContainer data = (TileLaserDataContainer)Activator.CreateInstance(attribute);
//        }
//    }
//}