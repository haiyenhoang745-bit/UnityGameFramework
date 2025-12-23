//------------------------------------------------------------
// Game Framework
// Copyright © 2013-2021 Jiang Yin. All rights reserved.
// Homepage: https://gameframework.cn/
// Feedback: mailto:ellan@gameframework.cn
//------------------------------------------------------------

using UnityEngine;
using UnityGameFramework.Runtime;

namespace StarForce
{
    /// <summary>
    /// 暂停界面。
    /// </summary>
    public class PauseForm : UGuiForm
    {
        private ProcedureMain m_ProcedureMain = null;

        /// <summary>
        /// 继续游戏按钮点击事件。
        /// </summary>
        public void OnResumeButtonClick()
        {
            Close();
        }

        /// <summary>
        /// 返回大厅按钮点击事件。
        /// </summary>
        public void OnQuitButtonClick()
        {
            if (m_ProcedureMain == null)
            {
                Log.Warning("ProcedureMain is invalid when quit from PauseForm.");
                return;
            }

            m_ProcedureMain.GotoMenu();
            Close();
        }

#if UNITY_2017_3_OR_NEWER
        protected override void OnOpen(object userData)
#else
        protected internal override void OnOpen(object userData)
#endif
        {
            base.OnOpen(userData);

            // 暂停游戏
            Time.timeScale = 0f;

            m_ProcedureMain = (ProcedureMain)userData;
            if (m_ProcedureMain == null)
            {
                Log.Warning("ProcedureMain is invalid when open PauseForm.");
                return;
            }
        }

#if UNITY_2017_3_OR_NEWER
        protected override void OnClose(bool isShutdown, object userData)
#else
        protected internal override void OnClose(bool isShutdown, object userData)
#endif
        {
            // 恢复游戏时间流速
            Time.timeScale = 1f;

            m_ProcedureMain = null;
            base.OnClose(isShutdown, userData);
        }
    }
}