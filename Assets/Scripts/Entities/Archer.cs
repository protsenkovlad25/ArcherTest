using Spine;
using Spine.Unity;
using System.Collections.Generic;
using UnityEngine;

namespace ArcherTest.Core.Entities
{
    [RequireComponent(typeof(Rigidbody2D), typeof(BoxCollider2D))]
    public class Archer : MonoBehaviour
    {
        #region Serialize Fields
        [Header("Ancher Data")]
        [SerializeField] private Transform m_CenterPoint;
        [SerializeField] private string m_AimBoneName;
        [SerializeField] private float m_TouchRadius = 2f;
        [SerializeField] private float m_MaxTiltAngle = 60f;
        [SerializeField] private float m_MinTiltAngle = -45f;

        [Header("Arrows Data")]
        [SerializeField] private Transform m_ArrowsParent;
        [SerializeField] private Transform m_ShootPoint;
        [SerializeField] private Arrow m_ArrowPrefab;
        [SerializeField] private float m_ForceMultiplier = 10f;

        [Header("Trajectory Data")]
        [SerializeField] private Transform m_DotsParent;
        [SerializeField] private TrajectoryDot m_DotPrefab;
        [SerializeField] private int m_DotsCount = 10;
        [SerializeField] private float m_DotSpacing = 0.1f;
        [SerializeField] private float m_DotMinScale = 0.3f;

        [Header("Tension Data")]
        [SerializeField] private LineRenderer m_TensionLine;
        [SerializeField] private float m_MaxTensionDistance = 1.5f;
        #endregion

        #region Fields
        private bool m_IsAiming;
        private Vector2 m_StartPoint;

        private Bone m_AimBone;
        private SkeletonAnimation m_SkeletonAnimation;
        
        private Pool<Arrow> m_ArrowsPool;
        private Pool<TrajectoryDot> m_DotsPool;

        private List<TrajectoryDot> m_ActiveDots;
        #endregion

        #region Properties
        public bool IsAiming => m_IsAiming;
        #endregion

        #region Methods
        #region Init Methods
        public void Init()
        {
            m_SkeletonAnimation = GetComponent<SkeletonAnimation>();

            m_ActiveDots = new List<TrajectoryDot>();

            m_AimBone = m_SkeletonAnimation.Skeleton.FindBone(m_AimBoneName);
            m_SkeletonAnimation.AnimationState.SetAnimation(0, "idle", true);

            InitArrowsPool();
            InitDotsPool();
        }
        private void InitArrowsPool()
        {
            m_ArrowsPool = new Pool<Arrow>(m_ArrowPrefab, 10, m_ArrowsParent);

            foreach (var arrow in m_ArrowsPool.ObjectsList)
                arrow.Init();
        }
        private void InitDotsPool()
        {
            m_DotsPool = new Pool<TrajectoryDot>(m_DotPrefab, m_DotsCount, m_DotsParent);

            foreach (var dot in m_DotsPool.ObjectsList)
                dot.Init();
        }
        #endregion

        public bool IsTouched(Vector2 point)
        {
            return Vector2.Distance(m_CenterPoint.position, point) <= m_TouchRadius;
        }

        private void RotateBone(Vector2 direction)
        {
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            m_AimBone.Rotation = angle;
        }

        #region Aiming
        public void StartAiming(Vector2 point)
        {
            m_IsAiming = true;
            m_StartPoint = point;

            m_SkeletonAnimation.AnimationState.SetAnimation(0, "attack_start", false);

            ActivateTrajectory();
        }
        public void UpdateAiming(Vector2 currentPoint)
        {
            Vector2 dragVector = ClampTensionStrength(ClampVectorAngle(m_StartPoint - currentPoint));
            
            ShowTrajectory(dragVector);
            RotateBone(dragVector);
            DrawTensionLine(currentPoint);
        }
        #endregion

        #region Arrow
        public void ReleaseArrow(Vector2 releasePoint)
        {
            Vector2 dragVector = ClampTensionStrength(ClampVectorAngle(m_StartPoint - releasePoint));

            Shoot(dragVector);
            HideTrajectory();
            HideTensionLine();

            m_IsAiming = false;
            m_SkeletonAnimation.AnimationState.SetAnimation(0, "attack_finish", false);
        }
        private void Shoot(Vector2 force)
        {
            Arrow arrow = m_ArrowsPool.Take();

            arrow.transform.position = m_ShootPoint.position;
            arrow.Rigidbody.velocity = force * m_ForceMultiplier;
        }
        #endregion

        #region Calc Vector and Tension
        private Vector2 ClampVectorAngle(Vector2 vector)
        {
            float angle = Mathf.Atan2(vector.y, vector.x) * Mathf.Rad2Deg;
            float clampedAngle = Mathf.Clamp(angle, m_MinTiltAngle, m_MaxTiltAngle);
            float rad = clampedAngle * Mathf.Deg2Rad;

            return new Vector2(Mathf.Cos(rad), Mathf.Sin(rad)) * vector.magnitude;
        }
        private Vector2 ClampTensionStrength(Vector2 vector)
        {
            if (vector.magnitude > m_MaxTensionDistance)
            {
                return vector.normalized * m_MaxTensionDistance;
            }

            return vector;
        }
        #endregion

        #region Trajectory
        private void ActivateTrajectory()
        {
            for (int i = 0; i < m_DotsCount; i++)
            {
                m_ActiveDots.Add(m_DotsPool.Take());
            }
        }
        private void ShowTrajectory(Vector2 force)
        {
            Vector2 velocity = force * m_ForceMultiplier;

            for (int i = 0; i < m_ActiveDots.Count; i++)
            {
                float time = i * m_DotSpacing;
                Vector3 pos = m_ShootPoint.position + (Vector3)(velocity * time + 0.5f * Physics2D.gravity * time * time);
                m_ActiveDots[i].transform.position = pos;

                float scale = Mathf.Lerp(1f, m_DotMinScale, (float)i / m_DotsCount);
                m_ActiveDots[i].transform.localScale = new Vector3(scale, scale, 1f);
                m_ActiveDots[i].gameObject.SetActive(true);
            }
        }
        private void HideTrajectory()
        {
            m_DotsPool.ReturnAll();

            m_ActiveDots.Clear();
        }
        #endregion

        #region Tension Line
        private void DrawTensionLine(Vector2 currentPoint)
        {;
            Vector2 endpoint = m_StartPoint - ClampTensionStrength(m_StartPoint - currentPoint);

            m_TensionLine.SetPosition(0, m_StartPoint);
            m_TensionLine.SetPosition(1, endpoint);
            
            m_TensionLine.enabled = true;
        }
        private void HideTensionLine()
        {
            m_TensionLine.enabled = false;
        }
        #endregion
        #endregion
    }
}
