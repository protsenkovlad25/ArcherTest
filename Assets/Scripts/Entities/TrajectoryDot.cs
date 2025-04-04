using ArcherTest.Core.Interfaces;
using UnityEngine;

namespace ArcherTest.Core.Entities
{
    public class TrajectoryDot : MonoBehaviour, IPoolable
    {
        public event IPoolable.Disabled OnDisabled;

        public void Init()
        {
        }

        #region Poolable Methods
        public void OnActivate()
        {
        }
        public void OnDeactivate()
        {
        }
        public void Disable()
        {
            OnDisabled?.Invoke(this);
        }
        #endregion
    }
}
