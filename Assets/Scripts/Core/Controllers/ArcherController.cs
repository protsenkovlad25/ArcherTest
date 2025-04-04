using ArcherTest.Core.Entities;
using UnityEngine;

namespace ArcherTest.Core.Controllers
{
    public class ArcherController : MonoBehaviour
    {
        [SerializeField] private Archer m_Archer;

        public void Init()
        {
            m_Archer.Init();

            InputController.OnButton.AddListener(CheckTrajectory);
            InputController.OnButtonUp.AddListener(CheckRelease);
            InputController.OnButtonDown.AddListener(CheckAiming);
        }

        private void CheckTrajectory(Vector2 point)
        {
            if (m_Archer.IsAiming)
            {
                m_Archer.UpdateAiming(point);
            }
        }

        private void CheckRelease(Vector2 point)
        {
            if (m_Archer.IsAiming)
            {
                m_Archer.ReleaseArrow(point);
            }
        }

        private void CheckAiming(Vector2 point)
        {
            if (m_Archer.IsTouched(point))
            {
                m_Archer.StartAiming(point);
            }
        }
    }
}
