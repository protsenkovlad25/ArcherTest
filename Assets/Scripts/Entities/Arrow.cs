using ArcherTest.Core.Interfaces;
using Spine;
using Spine.Unity;
using UnityEngine;

namespace ArcherTest.Core.Entities
{
    [RequireComponent(typeof(Rigidbody2D), typeof(BoxCollider2D))]
    public class Arrow : MonoBehaviour, IPoolable
    {
        #region Actions
        public event IPoolable.Disabled OnDisabled;
        #endregion

        #region Fields
        private SkeletonAnimation m_SkeletonAnimation;
        private Rigidbody2D m_Rigidbody;
        #endregion

        #region Properties
        public Rigidbody2D Rigidbody => m_Rigidbody;
        #endregion

        #region Methods
        public void Init()
        {
            m_Rigidbody = GetComponent<Rigidbody2D>();

            m_SkeletonAnimation = GetComponent<SkeletonAnimation>();
            m_SkeletonAnimation.AnimationState.Complete += Disable;
        }

        #region Poolable Methods
        public void OnActivate()
        {
            m_SkeletonAnimation.AnimationState.SetAnimation(0, "idle", false);
        }
        public void OnDeactivate()
        {
        }
        public void Disable()
        {
            OnDisabled?.Invoke(this);
        }
        public void Disable(TrackEntry trackEntry)
        {
            if (trackEntry.Animation.Name == "attack")
            {
                Disable();
            }
        }
        #endregion

        private void UpdateRotation()
        {
            if (m_Rigidbody.velocity.sqrMagnitude > 0.01f)
            {
                float angle = Mathf.Atan2(m_Rigidbody.velocity.y, m_Rigidbody.velocity.x) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.Euler(0, 0, angle);
            }
        }

        private void CheckCollision()
        {
            m_Rigidbody.velocity = Vector2.zero;

            TrackEntry current = m_SkeletonAnimation.AnimationState.GetCurrent(0);

            if (current == null || current.Animation.Name != "attack")
            {
                m_SkeletonAnimation.AnimationState.SetAnimation(0, "attack", false);
            }
        }

        #region Unity Methods
        private void FixedUpdate()
        {
            UpdateRotation();
        }
        private void OnCollisionEnter2D(Collision2D collision)
        {
            CheckCollision();
        }
        #endregion
        #endregion
    }
}
