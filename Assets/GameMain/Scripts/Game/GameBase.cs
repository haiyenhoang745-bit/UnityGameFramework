//------------------------------------------------------------
// Game Framework
// Copyright © 2013-2021 Jiang Yin. All rights reserved.
// Homepage: https://gameframework.cn/
// Feedback: mailto:ellan@gameframework.cn
//------------------------------------------------------------

using GameFramework.Event;
using UnityEngine;
using UnityGameFramework.Runtime;

namespace StarForce
{
    public abstract class GameBase
    {
        public abstract GameMode GameMode
        {
            get;
        }

        protected ScrollableBackground SceneBackground
        {
            get;
            private set;
        }

        public bool GameOver
        {
            get;
            protected set;
        }

        private MyAircraft m_MyAircraft = null;

        // 游戏数据收集字段
        private int m_Score = 0;
        private int m_KillCount = 0;
        private float m_SurvivalTime = 0f;
        private int m_CurrentCombo = 0;
        private int m_MaxCombo = 0;

        /// <summary>
        /// 获取当前分数。
        /// </summary>
        public int Score
        {
            get { return m_Score; }
        }

        /// <summary>
        /// 获取击杀数量。
        /// </summary>
        public int KillCount
        {
            get { return m_KillCount; }
        }

        /// <summary>
        /// 获取存活时间。
        /// </summary>
        public float SurvivalTime
        {
            get { return m_SurvivalTime; }
        }

        /// <summary>
        /// 获取最大连击数。
        /// </summary>
        public int MaxCombo
        {
            get { return m_MaxCombo; }
        }

        public virtual void Initialize()
        {
            GameEntry.Event.Subscribe(ShowEntitySuccessEventArgs.EventId, OnShowEntitySuccess);
            GameEntry.Event.Subscribe(ShowEntityFailureEventArgs.EventId, OnShowEntityFailure);

            SceneBackground = Object.FindObjectOfType<ScrollableBackground>();
            if (SceneBackground == null)
            {
                Log.Warning("Can not find scene background.");
                return;
            }

            SceneBackground.VisibleBoundary.gameObject.GetOrAddComponent<HideByBoundary>();
            GameEntry.Entity.ShowMyAircraft(new MyAircraftData(GameEntry.Entity.GenerateSerialId(), 10000)
            {
                Name = "My Aircraft",
                Position = Vector3.zero,
            });

            GameOver = false;
            m_MyAircraft = null;
        }

        public virtual void Shutdown()
        {
            GameEntry.Event.Unsubscribe(ShowEntitySuccessEventArgs.EventId, OnShowEntitySuccess);
            GameEntry.Event.Unsubscribe(ShowEntityFailureEventArgs.EventId, OnShowEntityFailure);
        }

        public virtual void Update(float elapseSeconds, float realElapseSeconds)
        {
            if (m_MyAircraft != null && m_MyAircraft.IsDead)
            {
                GameOver = true;
                return;
            }

            // 累加存活时间
            if (!GameOver)
            {
                m_SurvivalTime += elapseSeconds;
            }
        }

        /// <summary>
        /// 增加分数。
        /// </summary>
        public void AddScore(int score)
        {
            m_Score += score;
        }

        /// <summary>
        /// 增加击杀数。
        /// </summary>
        public void AddKill()
        {
            m_KillCount++;
            m_CurrentCombo++;
            if (m_CurrentCombo > m_MaxCombo)
            {
                m_MaxCombo = m_CurrentCombo;
            }
        }

        /// <summary>
        /// 重置连击。
        /// </summary>
        public void ResetCombo()
        {
            m_CurrentCombo = 0;
        }

        /// <summary>
        /// 获取游戏结算数据。
        /// </summary>
        public GameSettlementData GetSettlementData()
        {
            return new GameSettlementData(m_Score, m_KillCount, m_SurvivalTime, m_MaxCombo);
        }

        protected virtual void OnShowEntitySuccess(object sender, GameEventArgs e)
        {
            ShowEntitySuccessEventArgs ne = (ShowEntitySuccessEventArgs)e;
            if (ne.EntityLogicType == typeof(MyAircraft))
            {
                m_MyAircraft = (MyAircraft)ne.Entity.Logic;
            }
        }

        protected virtual void OnShowEntityFailure(object sender, GameEventArgs e)
        {
            ShowEntityFailureEventArgs ne = (ShowEntityFailureEventArgs)e;
            Log.Warning("Show entity failure with error message '{0}'.", ne.ErrorMessage);
        }
    }
}
