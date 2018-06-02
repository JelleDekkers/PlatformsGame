using System;
using System.Xml;
using System.Xml.Serialization;
using System.Collections.Generic;
using UnityEngine.Events;
using System.Reflection;

using System.Linq;

namespace Serialization {

    public class UnityEventDataCollection {
        [XmlArray] public UnityEventData[] data;

        public UnityEventDataCollection() { }
        public UnityEventDataCollection(UnityEvent uEvent) {
            List<UnityEventData> dataList = new List<UnityEventData>();
            for (int i = 0; i < uEvent.GetPersistentEventCount(); i++) {
                if ((uEvent.GetPersistentTarget(i) == null) || String.IsNullOrEmpty(uEvent.GetPersistentMethodName(i)))
                    continue;
                ISerializableGameObject serializableObject = uEvent.GetPersistentTarget(i) as ISerializableGameObject;
                if (serializableObject != null) 
                    dataList.Add(new UnityEventData(serializableObject.Guid.ID, uEvent.GetPersistentTarget(i), uEvent.GetPersistentMethodName(i)));
                else
                    UnityEngine.Debug.LogWarning(uEvent.GetPersistentTarget(i) + " does not have the required ISerializableGameObject interface and will not saved");
            }
            data = dataList.ToArray();
        }

        public void Deserialize(ref UnityEvent uEvent) {
            for (int i = 0; i < data.Length; i++) {
                MethodInfo method = null;
                UnityAction action = null;

                UnityEngine.Object target = GUID.GetObjectByGUID(data[i].targetGuid);
                method = target.GetType().GetMethod(data[i].methodName);

                if (method == null) {
                    UnityEngine.Debug.LogWarning("No method " + data[i].methodName + " found in " + target + " with type " + target.GetType());
                    return;
                }

                action = (UnityAction)Delegate.CreateDelegate(typeof(UnityAction), target, method);
#if UNITY_EDITOR
                UnityEditor.Events.UnityEventTools.AddVoidPersistentListener(uEvent, action);
#else
                uEvent.AddListener(action);
#endif
            }
        }

        public class UnityEventData {

            [XmlElement] public int targetGuid;
            [XmlElement] public string methodName;
            //[XmlElement] public Object[] args;

            public UnityEventData() { }
            public UnityEventData(int targetGuid, Object target, string methodName) {
                this.targetGuid = targetGuid;
                this.methodName = methodName;
            }

            //private Type GetLowestSubType(Object obj) {
            //    Type result = obj.GetType();
            //    for (int i = 0; i < 100; i++) {
            //        if (result.BaseType == typeof(UnityEngine.MonoBehaviour))
            //            return result;
            //        result = result.BaseType;
            //    }
            //    UnityEngine.Debug.LogWarning("No suitable type found");
            //    return result;
            //}

        }
    }
}