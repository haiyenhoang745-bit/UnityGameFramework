//------------------------------------------------------------
// Game Framework
// Copyright © 2013-2021 Jiang Yin. All rights reserved.
// Homepage: https://gameframework.cn/
// Feedback: mailto:ellan@gameframework.cn
//------------------------------------------------------------

using GameFramework.Event;
using UnityGameFramework.Runtime;
using ProcedureOwner = GameFramework.Fsm.IFsm<GameFramework.Procedure.IProcedureManager>;

namespace StarForce
{
    /// <summary>
    /// 游戏结算流程。
    /// </summary>
    public class ProcedureSettlement : ProcedureBase
    {
        private bool m_GotoMenu = false;
        private SettlementForm m_SettlementForm = null;

        public override bool UseNativeDialog
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// 返回主菜单。
        /// </summary>
        public void GotoMenu()
        {
            m_GotoMenu = true;
        }

        protected override void OnInit(ProcedureOwner procedureOwner)
        {
            base.OnInit(procedureOwner);
        }

        protected override void OnEnter(ProcedureOwner procedureOwner)
        {
            base.OnEnter(procedureOwner);

            // 订阅 UI 打开成功事件
            GameEntry.Event.Subscribe(OpenUIFormSuccessEventArgs.EventId, OnOpenUIFormSuccess);

            m_GotoMenu = false;

            // 从 FSM 中获取结算数据
            VarObject varObject = procedureOwner.GetData<VarObject>("SettlementData");
            GameSettlementData settlementData;

            if (varObject == null || varObject.Value == null)
            {
                // 如果数据为空，记录警告并使用默认值
                Log.Warning("Settlement data is invalid, using default values.");
                settlementData = new GameSettlementData(0, 0, 0f, 0);
            }
            else
            {
                settlementData = (GameSettlementData)varObject.Value;
            }

            // 打开结算界面，传递数据
            GameEntry.UI.OpenUIForm(UIFormId.SettlementForm, settlementData);
        }

        protected override void OnUpdate(ProcedureOwner procedureOwner, float elapseSeconds, float realElapseSeconds)
        {
            base.OnUpdate(procedureOwner, elapseSeconds, realElapseSeconds);

            // 检测是否需要返回菜单
            if (m_GotoMenu)
            {
                // 设置下一个场景为菜单场景
                procedureOwner.SetData<VarInt32>("NextSceneId", GameEntry.Config.GetInt("Scene.Menu"));
                
                // 切换到场景切换流程
                ChangeState<ProcedureChangeScene>(procedureOwner);
            }
        }

        protected override void OnLeave(ProcedureOwner procedureOwner, bool isShutdown)
        {
            // 取消订阅事件
            GameEntry.Event.Unsubscribe(OpenUIFormSuccessEventArgs.EventId, OnOpenUIFormSuccess);

            // 关闭结算界面 - 使用 UI 组件的 CloseUIForm 方法
            if (m_SettlementForm != null)
            {
                GameEntry.UI.CloseUIForm(m_SettlementForm);
                m_SettlementForm = null;
            }

            // 清理 FSM 中的结算数据
            if (procedureOwner.HasData("SettlementData"))
            {
                procedureOwner.RemoveData("SettlementData");
            }

            base.OnLeave(procedureOwner, isShutdown);
        }

        /// <summary>
        /// 处理 UI 打开成功事件。
        /// </summary>
        /// <param name="sender">事件源。</param>
        /// <param name="e">事件参数。</param>
        private void OnOpenUIFormSuccess(object sender, GameEventArgs e)
        {
            OpenUIFormSuccessEventArgs ne = (OpenUIFormSuccessEventArgs)e;
            if (ne.UIForm.UIFormAssetName != AssetUtility.GetUIFormAsset("SettlementForm"))
            {
                return;
            }

            m_SettlementForm = (SettlementForm)ne.UIForm.Logic;
        }
    }
}