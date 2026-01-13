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
            // 增加整型配置
            GameEntry.Config.AddConfig("Mine_Int", false, 100, 0f, null);
            int Mine_Int = GameEntry.Config.GetInt("Mine_Int");
            // 增加字符串配置
            GameEntry.Config.AddConfig("Mine_String", false, 0, 0f, "Hello");
            string Mine_String = GameEntry.Config.GetString("Mine_String");
            // 增加浮点型配置
            GameEntry.Config.AddConfig("Mine_Float", false, 0, 3.14f, null);
            float Mine_Float = GameEntry.Config.GetFloat("Mine_Float");
            // 增加布尔型配置
            GameEntry.Config.AddConfig("Mine_Bool", true, 0, 0f, null);
            bool Mine_Bool = GameEntry.Config.GetBool("Mine_Bool");

            Debug.Log(PlayerName);
            Debug.Log(GameID);
            Debug.Log(Mine_Int);//输出100
            Debug.Log(Mine_String);//输出Hello
            Debug.Log(Mine_Float);//输出3.14
            Debug.Log(Mine_Bool);//true
            Debug.Log("内存中的个数：" + GameEntry.Config.Count);
            Debug.Log("删除内存中的Mine_Bool：");
            GameEntry.Config.RemoveConfig("Mine_Bool");
            Debug.Log("是否还存在Mine_Bool：" + GameEntry.Config.HasConfig("Mine_Bool"));

            // 保存配置到本地PlayerPref
            // 真的要增加用stringBuilder+AppendLine
            GameEntry.Setting.SetInt("Mine_Int_1",10);
            GameEntry.Setting.SetFloat("Mine_Float_1",10.5f);
            GameEntry.Setting.SetString("Mine_String_1","Mine_String_1");
            GameEntry.Setting.SetBool("Mine_Bool_1",false);
            // 保存到文件
            GameEntry.Setting.Save();
            // 读取配置
            int Mine_Int_1 = GameEntry.Setting.GetInt("Mine_Int_1", 0);
            float Mine_Float_1 = GameEntry.Setting.GetFloat("Mine_Float_1", 0);
            string Mine_String_1 = GameEntry.Setting.GetString("Mine_String_1", "N");
            bool Mine_Bool_1 = GameEntry.Setting.GetBool("Mine_Bool_1");
            Debug.Log(Mine_Int_1);
            Debug.Log(Mine_Float_1);
            Debug.Log(Mine_String_1);
            Debug.Log(Mine_Bool_1);

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
