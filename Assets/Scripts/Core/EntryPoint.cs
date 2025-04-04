using ArcherTest.Core.Controllers;
using UnityEngine;

namespace ArcherTest.Core
{
    public class EntryPoint : MonoBehaviour
    {
        [SerializeField] private GameController m_GameController;

        private void Awake()
        {
            m_GameController.Init();
        }
    }
}
