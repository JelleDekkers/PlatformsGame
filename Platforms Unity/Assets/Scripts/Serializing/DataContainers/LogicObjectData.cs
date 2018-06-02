using UnityEngine;
using System.Xml;
using System.Xml.Serialization;

namespace Serialization {

    [XmlInclude(typeof(CounterData))]
    public class LogicObjectData : DataContainer {
        
        public LogicObjectData() { }
        public LogicObjectData(LogicObject obj) : base(obj) { }
    }

    public class CounterData : LogicObjectData {

        [XmlElement] public int targetValue;
        [XmlElement] public UnityEventDataCollection OnTargetReachedEvent;
        [XmlElement] public UnityEventDataCollection OnValueChangedAfterTargetReachedEvent;

        public CounterData() { }
        public CounterData(Counter counter) : base(counter) {
            targetValue = counter.targetValue;
            OnTargetReachedEvent = new UnityEventDataCollection(counter.OnTargetReachedEvent);
            OnValueChangedAfterTargetReachedEvent = new UnityEventDataCollection(counter.OnValueChangedAfterTargetReachedEvent);
        }
    }
}