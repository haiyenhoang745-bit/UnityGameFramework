using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityGameFramework.Runtime;
using ProcedureOwner = GameFramework.Fsm.IFsm<GameFramework.Procedure.IProcedureManager>;

namespace StarForce
{
    //使用流程管理
    public class self_config : ProcedureBase
    {
        public override bool UseNativeDialog
        {
            get
            {
                return false;
            }
        }

        protected override void OnEnter(ProcedureOwner procedureOwner)
        {
            base.OnEnter(procedureOwner);
            LoadConfig();
        }
        protected override void OnLeave(ProcedureOwner procedureOwner, bool isShutdown)
        {
            base.OnLeave(procedureOwner, isShutdown);
        }
        protected override void OnUpdate(ProcedureOwner procedureOwner, float elapseSeconds, float realElapseSeconds)
        {
            base.OnUpdate(procedureOwner, elapseSeconds, realElapseSeconds);

            int GameID = GameEntry.Config.GetInt("Scene.Main");
            string PlayerName = GameEntry.Config.GetString("Player.Name");
            Debug.Log(PlayerName);
            Debug.Log(GameID);
        }
        private void LoadConfig()
        {
            //全局配置表路径
            string configAssetName = "Assets/GameMain/Configs/DefaultConfig.txt";
            //读取
            GameEntry.Config.ReadData(configAssetName, this);
        }

        // //使用mono的效果
        // void Awake()
        // {
        //     LoadConfig();
        // }

        // private void Start() 
        // {
        //     int GameID = GameEntry.Config.GetInt("Scene.Main");
        //     Debug.Log(GameID);
        // }


    }
}
