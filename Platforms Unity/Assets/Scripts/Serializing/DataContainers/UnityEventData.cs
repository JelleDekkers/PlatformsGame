using System;
using System.Xml;
using System.Xml.Serialization;
using System.Collections.Generic;
using UnityEngine.Events;
using System.Reflection;

namespace Serialization {

    public class UnityEventData {

        [XmlAttribute] public string typeName;
        [XmlAttribute] public string[] typeArgs;
        [XmlAttribute] public string methodName;

        public static readonly Dictionary<string, Type> DataLinks = new Dictionary<string, Type>() {
            {typeof(Tile).FullName, typeof(Tile) },
            {typeof(Block).FullName, typeof(Block) },
            {typeof(Wall).FullName, typeof(Wall) }
        };

        public UnityEventData() { } // default ctor necessary for xml
        public UnityEventData(Object target, string methodName) {
            if (target is ISerializableEventTarget) {
                ISerializableEventTarget targetEvent = target as ISerializableEventTarget;
                Init(target, methodName, targetEvent.GetEventArgsForDeserialization());
            } else {
                UnityEngine.Debug.LogWarning(target + " is not a serializable event type");
            }
        }

        private void Init(Object obj, string methodName, params string[] typeArgs) {
            typeName = GetLowestSubType(obj).FullName;
            this.methodName = methodName;
            this.typeArgs = typeArgs;
        }

        private Type GetLowestSubType(Object obj) {
            Type result = obj.GetType();
            for (int i = 0; i < 100; i++) {
                if (result.BaseType == typeof(UnityEngine.MonoBehaviour))
                    return result;
                result = result.BaseType;
            }
            UnityEngine.Debug.LogWarning("No suitable type found");
            return result;
        }

        public static UnityEventData[] GetEventData(UnityEvent e) {
            List<UnityEventData> data = new List<UnityEventData>();
            for (int i = 0; i < e.GetPersistentEventCount(); i++) {
                if ((e.GetPersistentTarget(i) == null) || String.IsNullOrEmpty(e.GetPersistentMethodName(i)))
                    continue;
                data.Add(new UnityEventData(e.GetPersistentTarget(i), e.GetPersistentMethodName(i)));
            }
            return data.ToArray();
        }

        public static UnityEngine.Object GetTarget(UnityEventData data) {
            Type t = DataLinks[data.typeName];
            if (t == typeof(Tile)) {
                return LevelManager.CurrentLevel.Tiles.GetTile(new IntVector2(int.Parse(data.typeArgs[0]), int.Parse(data.typeArgs[1])));
            } else if (t == typeof(Block)) {
                return LevelManager.CurrentLevel.Tiles.GetTile(new IntVector2(int.Parse(data.typeArgs[0]), int.Parse(data.typeArgs[1]))).occupant;
            } else if (t == typeof(Wall)) {
                return LevelManager.CurrentLevel.Walls.GetWall(new IntVector2(int.Parse(data.typeArgs[0]), int.Parse(data.typeArgs[1])),
                                                               new IntVector2(int.Parse(data.typeArgs[2]), int.Parse(data.typeArgs[3])));
            } else {
                UnityEngine.Debug.LogWarning("No corresponding type found for " + data.typeName);
                return null;
            }
        }

        public static void DeserializeEvents(UnityEvent e, UnityEventData[] data) {
            for (int i = 0; i < data.Length; i++) {
                MethodInfo method = null;
                UnityAction action = null;

                if (!DataLinks.ContainsKey(data[i].typeName)) {
                    UnityEngine.Debug.LogWarning(data[i].typeName + " is not found in UnityEventData.DataLinks");
                    return;
                }

                UnityEngine.Object target = GetTarget(data[i]);
                method = target.GetType().GetMethod(data[i].methodName);

                if(method == null) {
                    UnityEngine.Debug.LogWarning("No method " + data[i].methodName + " found in " + target);
                    return;
                }

                action = (UnityAction)Delegate.CreateDelegate(typeof(UnityAction), target, method);
#if UNITY_EDITOR
                UnityEditor.Events.UnityEventTools.AddVoidPersistentListener(e, action);
#else
                e.AddListener(action);
#endif
            }
        }
    }
}