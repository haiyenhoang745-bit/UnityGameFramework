//------------------------------------------------------------
// Game Framework
// Copyright © 2013-2021 Jiang Yin. All rights reserved.
// Homepage: https://gameframework.cn/
// Feedback: mailto:ellan@gameframework.cn
//------------------------------------------------------------

using GameFramework;
using UnityEngine;
using UnityEngine.UI;
using UnityGameFramework.Runtime;

namespace StarForce
{
    /// <summary>
    /// 游戏结算界面。
    /// </summary>
    public class SettlementForm : UGuiForm
    {
        [SerializeField]
        private Text m_ScoreText = null;

        [SerializeField]
        private Text m_KillCountText = null;

        [SerializeField]
        private Text m_SurvivalTimeText = null;

        [SerializeField]
        private Text m_MaxComboText = null;

        [SerializeField]
        private Button m_BackToMenuButton = null;

        private ProcedureSettlement m_ProcedureSettlement = null;

        protected override void OnInit(object userData)
        {
            base.OnInit(userData);

            // 绑定按钮点击事件
            if (m_BackToMenuButton != null)
            {
                m_BackToMenuButton.onClick.AddListener(OnBackToMenuButtonClick);
            }
        }

        protected override void OnOpen(object userData)
        {
            base.OnOpen(userData);

            // 获取结算数据
            GameSettlementData settlementData = (GameSettlementData)userData;
            if (userData == null)
            {
                Log.Warning("Settlement data is invalid.");
                return;
            }

            // 获取流程引用
            m_ProcedureSettlement = GameEntry.Procedure.CurrentProcedure as ProcedureSettlement;
            if (m_ProcedureSettlement == null)
            {
                Log.Warning("Current procedure is not ProcedureSettlement.");
                return;
            }

            // 显示结算数据
            if (m_ScoreText != null)
            {
                m_ScoreText.text = Utility.Text.Format("得分: {0}", settlementData.Score);
            }

            if (m_KillCountText != null)
            {
                m_KillCountText.text = Utility.Text.Format("击杀: {0}", settlementData.KillCount);
            }

            if (m_SurvivalTimeText != null)
            {
                m_SurvivalTimeText.text = Utility.Text.Format("存活时间: {0:F1}秒", settlementData.SurvivalTime);
            }

            if (m_MaxComboText != null)
            {
                m_MaxComboText.text = Utility.Text.Format("最大连击: {0}", settlementData.MaxCombo);
            }
        }

        protected override void OnClose(bool isShutdown, object userData)
        {
            m_ProcedureSettlement = null;
            base.OnClose(isShutdown, userData);
        }

        /// <summary>
        /// 返回菜单按钮点击事件。
        /// </summary>
        private void OnBackToMenuButtonClick()
        {
            if (m_ProcedureSettlement != null)
            {
                m_ProcedureSettlement.GotoMenu();
            }
        }
    }
}