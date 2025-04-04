using UnityEngine;
using UnityEngine.Events;

namespace ArcherTest.Core.Controllers
{
    public class InputController : MonoBehaviour
    {
        public static UnityEvent<Vector2> OnButtonDown = new();
        public static UnityEvent<Vector2> OnButtonUp = new();
        public static UnityEvent<Vector2> OnButton = new();

        public void Init()
        {
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                Vector2 mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                OnButtonDown?.Invoke(mouse);
            }

            if (Input.GetMouseButton(0))
            {
                Vector2 mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                OnButton?.Invoke(mouse);
            }

            if (Input.GetMouseButtonUp(0))
            {
                Vector2 mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                OnButtonUp?.Invoke(mouse);
            }
        }
    }
}
