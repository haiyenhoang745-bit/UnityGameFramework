//------------------------------------------------------------
// Game Framework
// Copyright © 2013-2021 Jiang Yin. All rights reserved.
// Homepage: https://gameframework.cn/
// Feedback: mailto:ellan@gameframework.cn
//------------------------------------------------------------

using System.Runtime.InteropServices;

namespace StarForce
{
    /// <summary>
    /// 游戏结算数据。
    /// </summary>
    [StructLayout(LayoutKind.Auto)]
    public struct GameSettlementData
    {
        private readonly int m_Score;
        private readonly int m_KillCount;
        private readonly float m_SurvivalTime;
        private readonly int m_MaxCombo;

        /// <summary>
        /// 初始化游戏结算数据的新实例。
        /// </summary>
        /// <param name="score">本局得分。</param>
        /// <param name="killCount">击杀数量。</param>
        /// <param name="survivalTime">存活时间（秒）。</param>
        /// <param name="maxCombo">最大连击数。</param>
        public GameSettlementData(int score, int killCount, float survivalTime, int maxCombo)
        {
            m_Score = score;
            m_KillCount = killCount;
            m_SurvivalTime = survivalTime;
            m_MaxCombo = maxCombo;
        }

        /// <summary>
        /// 获取本局得分。
        /// </summary>
        public int Score
        {
            get
            {
                return m_Score;
            }
        }

        /// <summary>
        /// 获取击杀数量。
        /// </summary>
        public int KillCount
        {
            get
            {
                return m_KillCount;
            }
        }

        /// <summary>
        /// 获取存活时间（秒）。
        /// </summary>
        public float SurvivalTime
        {
            get
            {
                return m_SurvivalTime;
            }
        }

        /// <summary>
        /// 获取最大连击数。
        /// </summary>
        public int MaxCombo
        {
            get
            {
                return m_MaxCombo;
            }
        }
    }
}