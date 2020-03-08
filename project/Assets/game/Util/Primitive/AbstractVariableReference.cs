using UnityEngine;

namespace Amheklerior.Gravity.Util {

    /** <summary> A generic value reference shared between multiple components/systems. </summary> */
    public abstract class AbstractVariableReference<T> : ScriptableObject, ISerializationCallbackReceiver {

        [Header("Settings")]
        [Tooltip("The default value")][SerializeField] protected T defaultValue;
        
        public T CurrentValue { get => current; set => current = value; }

        public virtual void ResetValue() => current = defaultValue;
        
        #region ISerializationCallbackReceiver implementation

        public void OnBeforeSerialize() { }
        public void OnAfterDeserialize() => ResetValue();

        #endregion

        #region SetUp / TearDown

        private void OnEnable() => ResetValue();
        private void OnDisable() => ResetValue();

        #endregion

        #region Debug View

        [Header("Debug View:")]
        [Tooltip(tooltip:"The current value.")] [SerializeField] private T current;

        #endregion
    }
}