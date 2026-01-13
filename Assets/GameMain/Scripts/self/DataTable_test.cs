using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityGameFramework.Runtime;
using System.Data;
using System;

namespace StarForce
{
    public class DataTable_test : MonoBehaviour
    {
        DataTableComponent DataTable;
        string[] DataTableNames;

        private void Start()
        {
            DataTable = GameEntry.DataTable;
            DataTableNames = new string[2]{"Scene","Music"};
            // 预加载 data tables数据表
            foreach (string dataTableName in DataTableNames)
            {
                LoadDataTable(dataTableName);
            } 
        }
        /// <summary>
        /// 加载数据表
        /// </summary>
        /// <param name="dataTableName"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void LoadDataTable(string dataTableName)
        {
            string dataTableAssetName = AssetUtility.GetDataTableAsset(dataTableName, false);
            // 表名、 表名资源路径、用户自定义数据
            DataTable.LoadDataTable(dataTableName,dataTableAssetName,this);
        }

    }
}



