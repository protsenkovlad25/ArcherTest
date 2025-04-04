using UnityEngine;

namespace ArcherTest.Core.Controllers
{
    public class GameController : MonoBehaviour
    {
        [SerializeField] private InputController m_InputController;
        [SerializeField] private ArcherController m_ArcherController;

        public void Init()
        {
            m_InputController.Init();
            m_ArcherController.Init();
        }
    }
}
