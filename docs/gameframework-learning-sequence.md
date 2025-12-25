# GameFramework 学习顺序指南

## 学习路径概述

本文档提供了一个系统的 GameFramework 学习路径，从基础到高级，帮助您循序渐进地掌握框架的核心概念和用法。

---

## 阶段一：框架基础入门（1-2周）

### 学习目标
- 理解 GameFramework 的整体架构
- 掌握框架入口和核心模块
- 学会基础的模块访问方式

### 学习顺序

#### 1.1 GameEntry - 框架入口
**文件：** [`Assets/GameMain/Scripts/Base/GameEntry.cs`](Assets/GameMain/Scripts/Base/GameEntry.cs)

**学习重点：**
- 理解 `GameEntry` 的静态属性：
  - `GameEntry.Procedure` - 流程管理器
  - `GameEntry.Entity` - 实体管理器
  - `GameEntry.UI` - UI 管理器
  - `GameEntry.DataTable` - 数据表管理器
  - `GameEntry.Sound` - 声音管理器
  - `GameEntry.Config` - 配置管理器
  - `GameEntry.HPBar` - HP 条管理器

**练习：**
- 查看 `GameEntry` 的实现
- 理解各个模块的初始化顺序
- 了解如何通过 `GameEntry` 访问框架功能

#### 1.2 基础概念理解
**核心概念：**
- **有限状态机（FSM）**：管理游戏流程
- **实体系统**：管理游戏对象
- **数据表系统**：从配置读取数据
- **引用池**：优化对象创建和销毁

**学习资源：**
- GameFramework 官方文档：https://gameframework.cn/
- 项目中的示例代码

---

## 阶段二：流程系统（2-3周）

### 学习目标
- 掌握游戏流程的管理
- 理解状态机的生命周期
- 学会流程间的数据传递

### 学习顺序

#### 2.1 ProcedureBase - 流程基类
**文件：** `Assets/GameFramework/Scripts/Runtime/Procedure/ProcedureBase.cs`（编译在 DLL 中）

**学习重点：**
- 流程生命周期方法：
  - `OnInit()` - 初始化
  - `OnEnter()` - 进入状态
  - `OnUpdate()` - 每帧更新
  - `OnLeave()` - 离开状态
  - `OnDestroy()` - 销毁

#### 2.2 ProcedureMain - 主游戏流程
**文件：** [`Assets/GameMain/Scripts/Procedure/ProcedureMain.cs`](Assets/GameMain/Scripts/Procedure/ProcedureMain.cs)

**学习重点：**
- 流程的创建和初始化
- 游戏实例的管理
- 流程切换逻辑
- FSM 数据的使用

**代码分析：**
```csharp
// 流程进入时初始化游戏
protected override void OnEnter(ProcedureOwner procedureOwner)
{
    m_CurrentGame = m_Games[gameMode];
    m_CurrentGame.Initialize();
}

// 每帧更新游戏
protected override void OnUpdate(ProcedureOwner procedureOwner, float elapseSeconds, float realElapseSeconds)
{
    m_CurrentGame.Update(elapseSeconds, realElapseSeconds);
}

// 流程离开时清理
protected override void OnLeave(ProcedureOwner procedureOwner, bool isShutdown)
{
    m_CurrentGame.Shutdown();
}
```

**练习：**
- 添加一个新的游戏模式
- 实现流程切换逻辑
- 在流程间传递数据

#### 2.3 ProcedureSettlement - 结算流程
**文件：** [`Assets/GameMain/Scripts/Procedure/ProcedureSettlement.cs`](Assets/GameMain/Scripts/Procedure/ProcedureSettlement.cs)

**学习重点：**
- UI 的打开和关闭
- 事件订阅和取消订阅
- FSM 数据的读取和设置

**代码分析：**
```csharp
// 进入时订阅事件并打开 UI
protected override void OnEnter(ProcedureOwner procedureOwner)
{
    GameEntry.Event.Subscribe(OpenUIFormSuccessEventArgs.EventId, OnOpenUIFormSuccess);
    GameEntry.UI.OpenUIForm(UIFormId.SettlementForm, settlementData);
}

// 离开时取消订阅并关闭 UI
protected override void OnLeave(ProcedureOwner procedureOwner, bool isShutdown)
{
    GameEntry.Event.Unsubscribe(OpenUIFormSuccessEventArgs.EventId, OnOpenUIFormSuccess);
    GameEntry.UI.CloseUIForm(m_SettlementForm);
}
```

**练习：**
- 创建一个新的流程
- 实现流程间的数据传递
- 管理事件订阅

---

## 阶段三：游戏逻辑（2-3周）

### 学习目标
- 理解游戏数据的收集和管理
- 掌握时间管理
- 学会游戏状态的判断

### 学习顺序

#### 3.1 GameBase - 游戏逻辑基类
**文件：** [`Assets/GameMain/Scripts/Game/GameBase.cs`](Assets/GameMain/Scripts/Game/GameBase.cs)

**学习重点：**
- 游戏数据的收集（分数、击杀、连击、存活时间）
- 游戏状态管理（GameOver 标志）
- 结算数据的生成

**核心方法：**
```csharp
// 累计存活时间
public virtual void Update(float elapseSeconds, float realElapseSeconds)
{
    if (!GameOver)
    {
        m_SurvivalTime += elapseSeconds;
    }
}

// 增加分数
public void AddScore(int score)
{
    m_Score += score;
}

// 获取结算数据
public GameSettlementData GetSettlementData()
{
    return new GameSettlementData(m_Score, m_KillCount, m_SurvivalTime, m_MaxCombo);
}
```

**练习：**
- 添加新的游戏数据类型
- 实现连击系统
- 创建自定义结算数据

#### 3.2 SurvivalGame - 生存模式游戏
**文件：** `Assets/GameMain/Scripts/Game/SurvivalGame.cs`（未打开，但被 ProcedureMain 引用）

**学习重点：**
- 继承 GameBase 实现具体游戏模式
- 游戏规则的实现
- 与实体系统的交互

---

## 阶段四：实体系统（3-4周）

### 学习目标
- 掌握实体的生命周期
- 理解实体数据的配置
- 学会实体的碰撞检测

### 学习顺序

#### 4.1 Entity - 实体基类
**文件：** `Assets/GameFramework/Scripts/Runtime/Entity/Entity.cs`（编译在 DLL 中）

**学习重点：**
- 实体生命周期：
  - `OnInit(object userData)` - 初始化
  - `OnShow(object userData)` - 显示
  - `OnUpdate(float elapseSeconds, float realElapseSeconds)` - 更新
  - `OnHide()` - 隐藏
  - `OnRecycle()` - 回收
- 实体属性（Id、Priority、CachedTransform）

#### 4.2 EntityData - 实体数据基类
**文件：** `Assets/GameFramework/Scripts/Runtime/Entity/EntityData.cs`（编译在 DLL 中）

**学习重点：**
- 实体基础属性（EntityId、TypeId、CampType）
- 数据表集成

#### 4.3 TargetableObject - 可目标实体
**文件：** [`Assets/GameMain/Scripts/Entity/EntityLogic/TargetableObject.cs`](Assets/GameMain/Scripts/Entity/EntityLogic/TargetableObject.cs)

**学习重点：**
- HP 管理和伤害计算
- 死亡处理
- HP 条显示
- 碰撞检测

**核心方法：**
```csharp
// 应用伤害
public void ApplyDamage(Entity attacker, int damageHP)
{
    m_TargetableObjectData.HP -= damageHP;
    
    // HP 耗尽时死亡
    if (m_TargetableObjectData.HP <= 0)
    {
        OnDead(attacker);
    }
}

// 死亡处理
protected virtual void OnDead(Entity attacker)
{
    GameEntry.Entity.HideEntity(this);
}
```

**练习：**
- 创建自定义的可目标实体
- 实现不同的伤害计算逻辑
- 添加死亡特效和音效

#### 4.4 Asteroid - 小行星实体
**文件：** [`Assets/GameMain/Scripts/Entity/EntityLogic/Asteroid.cs`](Assets/GameMain/Scripts/Entity/EntityLogic/Asteroid.cs)

**学习重点：**
- 实体移动和旋转
- 死亡时更新游戏数据
- 特效和音效的播放

**代码分析：**
```csharp
// 移动和旋转
protected override void OnUpdate(float elapseSeconds, float realElapseSeconds)
{
    CachedTransform.Translate(Vector3.back * m_AsteroidData.Speed * elapseSeconds, Space.World);
    CachedTransform.Rotate(m_RotateSphere * m_AsteroidData.AngularSpeed * elapseSeconds, Space.Self);
}

// 死亡时记录数据
protected override void OnDead(Entity attacker)
{
    base.OnDead(attacker);
    
    GameBase currentGame = ProcedureMain.CurrentGame;
    if (currentGame != null)
    {
        currentGame.AddScore(100);  // 击杀小行星加 100 分
        currentGame.AddKill();      // 增加击杀数
    }
}
```

**练习：**
- 创建不同类型的小行星
- 实现小行星的生成逻辑
- 添加不同的死亡效果

#### 4.5 MyAircraft - 玩家飞船
**文件：** [`Assets/GameMain/Scripts/Entity/EntityLogic/MyAircraft.cs`](Assets/GameMain/Scripts/Entity/EntityLogic/MyAircraft.cs)

**学习重点：**
- 玩家控制（鼠标跟随）
- 武器发射
- 受伤时重置连击

**代码分析：**
```csharp
// 鼠标控制移动
protected override void OnUpdate(float elapseSeconds, float realElapseSeconds)
{
    if (Input.GetMouseButton(0))
    {
        Vector3 point = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        m_TargetPosition = new Vector3(point.x, 0f, point.z);
        
        // 发射武器
        for (int i = 0; i < m_Weapons.Count; i++)
        {
            m_Weapons[i].TryAttack();
        }
    }
}

// 受伤时重置连击
public new void ApplyDamage(Entity attacker, int damageHP)
{
    base.ApplyDamage(attacker, damageHP);
    
    GameBase currentGame = ProcedureMain.CurrentGame;
    if (currentGame != null)
    {
        currentGame.ResetCombo();
    }
}
```

**练习：**
- 实现键盘控制
- 添加武器升级系统
- 实现无敌时间

---

## 阶段五：数据系统（1-2周）

### 学习目标
- 掌握数据表的配置
- 理解实体数据的读取
- 学会数据表的使用

### 学习顺序

#### 5.1 EntityData - 实体数据基类
**文件：** `Assets/GameFramework/Scripts/Runtime/Entity/EntityData.cs`（编译在 DLL 中）

**学习重点：**
- 实体 ID 和类型 ID
- 阵营类型
- 数据表集成

#### 5.2 TargetableObjectData - 可目标实体数据
**文件：** `Assets/GameFramework/Scripts/Runtime/Entity/TargetableObjectData.cs`（编译在 DLL 中）

**学习重点：**
- HP 和 HP 比例
- 最大 HP 属性

#### 5.3 AsteroidData - 小行星数据
**文件：** [`Assets/GameMain/Scripts/Entity/EntityData/AsteroidData.cs`](Assets/GameMain/Scripts/Entity/EntityData/AsteroidData.cs)

**学习重点：**
- 从数据表读取配置
- 实体属性的封装
- 特效和音效 ID 的使用

**代码分析：**
```csharp
public AsteroidData(int entityId, int typeId) : base(entityId, typeId, CampType.Neutral)
{
    IDataTable<DRAsteroid> dtAsteroid = GameEntry.DataTable.GetDataTable<DRAsteroid>();
    DRAsteroid drAsteroid = dtAsteroid.GetDataRow(TypeId);
    
    HP = m_MaxHP = drAsteroid.MaxHP;
    m_Attack = drAsteroid.Attack;
    m_Speed = drAsteroid.Speed;
    m_AngularSpeed = drAsteroid.AngularSpeed;
    m_DeadEffectId = drAsteroid.DeadEffectId;
    m_DeadSoundId = drAsteroid.DeadSoundId;
}
```

**练习：**
- 查看数据表配置文件
- 创建新的实体数据类型
- 添加新的数据表行

---

## 阶段六：UI 系统（2-3周）

### 学习目标
- 掌握 UI 表单的生命周期
- 理解 UI 的打开和关闭
- 学会 UI 与流程的交互

### 学习顺序

#### 6.1 UIForm - UI 表单基类
**文件：** `Assets/GameFramework/Scripts/Runtime/UI/UIForm.cs`（编译在 DLL 中）

**学习重点：**
- UI 表单生命周期：
  - `OnInit(object userData)` - 初始化
  - `OnRecycle()` - 回收
  - `OnOpen(object userData)` - 打开
  - `OnClose(bool isShutdown, object userData)` - 关闭
- UI 表单 ID

#### 6.2 UGuiForm - Unity GUI 表单
**文件：** [`Assets/GameMain/Scripts/UI/UGuiForm.cs`](Assets/GameMain/Scripts/UI/UGuiForm.cs)

**学习重点：**
- Unity UI 的封装
- 与 GameFramework 的集成

#### 6.3 SettlementForm - 结算界面
**文件：** [`Assets/GameMain/Scripts/UI/SettlementForm.cs`](Assets/GameMain/Scripts/UI/SettlementForm.cs)

**学习重点：**
- UI 组件的引用（Text、Button）
- 数据的显示和格式化
- 按钮事件的处理
- 与流程的交互

**代码分析：**
```csharp
// 打开时显示数据
protected override void OnOpen(object userData)
{
    GameSettlementData settlementData = (GameSettlementData)userData;
    
    m_ScoreText.text = Utility.Text.Format("得分: {0}", settlementData.Score);
    m_KillCountText.text = Utility.Text.Format("击杀: {0}", settlementData.KillCount);
    m_SurvivalTimeText.text = Utility.Text.Format("存活时间: {0:F1}秒", settlementData.SurvivalTime);
    m_MaxComboText.text = Utility.Text.Format("最大连击: {0}", settlementData.MaxCombo);
}

// 按钮点击事件
private void OnBackToMenuButtonClick()
{
    if (m_ProcedureSettlement != null)
    {
        m_ProcedureSettlement.GotoMenu();
    }
}
```

**练习：**
- 创建自定义 UI 表单
- 实现动态数据更新
- 添加动画效果

---

## 阶段七：高级主题（持续学习）

### 7.1 事件系统
**学习重点：**
- 事件的订阅和取消订阅
- 自定义事件的创建
- 事件参数的传递

**相关文件：**
- `OpenUIFormSuccessEventArgs.cs`

### 7.2 引用池（ReferencePool）
**学习重点：**
- 对象的获取和释放
- 减少垃圾回收
- 提高性能

**相关代码：**
```csharp
// 从引用池获取对象
VarObject varObject = ReferencePool.Acquire<VarObject>();
varObject.Value = settlementData;

// 使用后释放
ReferencePool.Release(varObject);
```

### 7.3 变量系统
**学习重点：**
- 变量包装类（VarInt32, VarByte, VarObject）
- 类型安全的数据传递
- FSM 数据的使用

**相关文件：**
- `VarObject.cs`

### 7.4 场景管理
**学习重点：**
- 场景的加载和卸载
- 场景切换流程
- 场景配置

**相关流程：**
- `ProcedureChangeScene`

---

## 学习检查清单

### 基础阶段
- [ ] 理解 GameEntry 的所有模块
- [ ] 知道如何访问各个管理器
- [ ] 理解引用池的作用

### 流程阶段
- [ ] 掌握流程的生命周期
- [ ] 会创建自定义流程
- [ ] 理解 FSM 数据的传递
- [ ] 会切换流程

### 游戏逻辑阶段
- [ ] 理解游戏数据的收集
- [ ] 会实现连击系统
- [ ] 会生成结算数据

### 实体阶段
- [ ] 掌握实体的生命周期
- [ ] 会创建自定义实体
- [ ] 理解碰撞检测
- [ ] 会使用实体数据

### 数据阶段
- [ ] 会查看数据表配置
- [ ] 会创建实体数据类
- [ ] 理解数据表的读取

### UI 阶段
- [ ] 会创建 UI 表单
- [ ] 理解 UI 的生命周期
- [ ] 会实现 UI 与流程的交互

---

## 实战项目建议

### 项目一：简单的计数器
**目标：**
- 创建一个简单的计数流程
- 显示计数结果
- 实现返回菜单功能

**涉及模块：**
- Procedure
- UIForm
- GameEntry

### 项目二：简单的射击游戏
**目标：**
- 实现玩家控制
- 创建敌人实体
- 实现射击和碰撞检测
- 添加分数系统

**涉及模块：**
- Procedure
- Entity
- EntityData
- GameBase
- UIForm

### 项目三：完整的生存游戏
**目标：**
- 实现完整的游戏流程
- 添加多种敌人类型
- 实现连击系统
- 添加结算界面

**涉及模块：**
- 所有模块

---

## 学习资源

### 官方资源
- **官方网站：** https://gameframework.cn/
- **GitHub 仓库：** https://github.com/GameFramework/GameFramework
- **在线文档：** https://gameframework.cn/document

### 项目资源
- **示例项目：** StarForce（当前项目）
- **数据表配置：** `Assets/GameMain/DataTables/`
- **实体预制体：** `Assets/GameMain/Entities/`

---

## 学习时间表

| 阶段 | 模块 | 预计时间 | 难度 |
|------|------|-----------|------|
| 一 | 框架基础 | 1-2 周 | ⭐ |
| 二 | 流程系统 | 2-3 周 | ⭐⭐ |
| 三 | 游戏逻辑 | 2-3 周 | ⭐⭐ |
| 四 | 实体系统 | 3-4 周 | ⭐⭐⭐ |
| 五 | 数据系统 | 1-2 周 | ⭐⭐ |
| 六 | UI 系统 | 2-3 周 | ⭐⭐ |
| 七 | 高级主题 | 持续 | ⭐⭐⭐⭐ |

**总计：** 约 11-17 周完成基础学习

---

## 常见问题解答

### Q1: 什么时候使用 Procedure，什么时候使用 Entity？
**A:** 
- **Procedure** 用于管理游戏的整体流程（菜单、游戏、结算）
- **Entity** 用于管理游戏中的具体对象（玩家、敌人、子弹）

### Q2: 如何在流程间传递数据？
**A:** 使用 FSM 的 `SetData` 和 `GetData` 方法：
```csharp
// 设置数据
procedureOwner.SetData<VarInt32>("Score", 100);

// 获取数据
VarInt32 scoreVar = procedureOwner.GetData<VarInt32>("Score");
int score = scoreVar.Value;
```

### Q3: 实体数据为什么要从数据表读取？
**A:** 
- 便于配置调整（修改数据表即可）
- 支持多种配置（不同类型的实体）
- 便于扩展（添加新实体类型）

### Q4: 什么时候需要重写 OnLeave？
**A:** 
- 当流程需要清理资源时
- 当需要取消事件订阅时
- 当需要保存状态时

---

**文档版本：** 1.0  
**创建日期：** 2025-12-25  
**适用框架版本：** GameFramework 2021
