
using Sirenix.OdinInspector;
using UnityEngine;

namespace Services
{
    /// <summary>
    /// Represents a singleton script that can be invoked from
    /// everywhere else in the scene.
    /// </summary>
    public abstract class MonoService :SerializedMonoBehaviour, Service
    {
        private void OnEnable()
        {
            ServiceLocator.AddService(this);
        }

        private void OnDisable()
        {
            ServiceLocator.RemoveService(this);
        }
    }
}