# StarForce 结算页面功能技术复盘文档

## 📋 文档概述

本文档对 StarForce 项目新增的结算页面功能进行全面的技术复盘，分析代码变更、架构设计和实现细节。

---

## 一、新增功能逻辑流程

### 功能运行流程（分步描述）

**第一步：游戏数据收集阶段**
- 在游戏运行过程中，[`GameBase`](Assets/GameMain/Scripts/Game/GameBase.cs:14) 类持续收集游戏数据
- 通过 [`m_Score`](Assets/GameMain/Scripts/Game/GameBase.cs:36)、[`m_KillCount`](Assets/GameMain/Scripts/Game/GameBase.cs:37)、[`m_SurvivalTime`](Assets/GameMain/Scripts/Game/GameBase.cs:38)、[`m_MaxCombo`](Assets/GameMain/Scripts/Game/GameBase.cs:40) 等字段记录玩家表现
- 在 [`Update()`](Assets/GameMain/Scripts/Game/GameBase.cs:103) 方法中累加存活时间

**第二步：击杀数据统计**
- 当小行星被击毁时，[`Asteroid.OnDead()`](Assets/GameMain/Scripts/Entity/EntityLogic/Asteroid.cs:62) 方法被调用
- 通过 [`ProcedureMain.CurrentGame`](Assets/GameMain/Scripts/Procedure/ProcedureMain.cs:28) 获取当前游戏实例
- 调用 [`AddScore(100)`](Assets/GameMain/Scripts/Game/GameBase.cs:121) 增加分数
- 调用 [`AddKill()`](Assets/GameMain/Scripts/Game/GameBase.cs:129) 增加击杀数并更新连击数

**第三步：玩家受伤处理**
- 当玩家飞机受到伤害时，[`MyAircraft.ApplyDamage()`](Assets/GameMain/Scripts/Entity/EntityLogic/MyAircraft.cs:93) 被调用
- 调用 [`ResetCombo()`](Assets/GameMain/Scripts/Game/GameBase.cs:142) 重置连击计数

**第四步：游戏结束检测**
- [`ProcedureMain.OnUpdate()`](Assets/GameMain/Scripts/Procedure/ProcedureMain.cs:85) 每帧检测游戏状态
- 当 [`m_CurrentGame.GameOver`](Assets/GameMain/Scripts/Procedure/ProcedureMain.cs:89) 为 true 时，开始结算倒计时
- 等待 [`GameOverDelayedSeconds`](Assets/GameMain/Scripts/Procedure/ProcedureMain.cs:18)（2秒）后进入结算流程

**第五步：结算数据封装**
- 调用 [`m_CurrentGame.GetSettlementData()`](Assets/GameMain/Scripts/Procedure/ProcedureMain.cs:107) 获取结算数据
- 创建 [`GameSettlementData`](Assets/GameMain/Scripts/Definition/DataStruct/GameSettlementData.cs:16) 结构体实例
- 使用 [`ReferencePool.Acquire<VarObject>()`](Assets/GameMain/Scripts/Procedure/ProcedureMain.cs:110) 从引用池获取变量对象
- 将结算数据包装到 VarObject 中

**第六步：流程状态切换**
- 通过 [`procedureOwner.SetData<VarObject>("SettlementData", varObject)`](Assets/GameMain/Scripts/Procedure/ProcedureMain.cs:112) 将数据存入 FSM
- 调用 [`ChangeState<ProcedureSettlement>(procedureOwner)`](Assets/GameMain/Scripts/Procedure/ProcedureSettlement.cs:114) 切换到结算流程

**第七步：结算流程初始化**
- [`ProcedureSettlement.OnEnter()`](Assets/GameMain/Scripts/Procedure/ProcedureSettlement.cs:43) 被调用
- 订阅 [`OpenUIFormSuccessEventArgs`](Assets/GameMain/Scripts/Procedure/ProcedureSettlement.cs:48) 事件
- 从 FSM 中获取 [`"SettlementData"`](Assets/GameMain/Scripts/Procedure/ProcedureSettlement.cs:53) 数据

**第八步：打开结算界面**
- 调用 [`GameEntry.UI.OpenUIForm(UIFormId.SettlementForm, settlementData)`](Assets/GameMain/Scripts/Procedure/ProcedureSettlement.cs:68) 打开结算界面
- 将结算数据作为 userData 参数传递给 UI

**第九步：UI 数据显示**
- [`SettlementForm.OnOpen()`](Assets/GameMain/Scripts/UI/SettlementForm.cs:53) 接收结算数据
- 将 userData 转换为 [`GameSettlementData`](Assets/GameMain/Scripts/UI/SettlementForm.cs:61) 类型
- 使用 [`Utility.Text.Format()`](Assets/GameMain/Scripts/UI/SettlementForm.cs:79) 格式化并显示各项数据到 UI 组件

**第十步：用户交互处理**
- 用户点击"返回大厅"按钮触发 [`OnBackToMenuButtonClick()`](Assets/GameMain/Scripts/UI/SettlementForm.cs:111)
- 调用 [`m_ProcedureSettlement.GotoMenu()`](Assets/GameMain/Scripts/UI/SettlementForm.cs:115) 设置返回标记

**第十一步：返回主菜单**
- [`ProcedureSettlement.OnUpdate()`](Assets/GameMain/Scripts/Procedure/ProcedureSettlement.cs:71) 检测到 [`m_GotoMenu`](Assets/GameMain/Scripts/Procedure/ProcedureSettlement.cs:76) 标记
- 设置下一个场景 ID 为菜单场景
- 切换到 [`ProcedureChangeScene`](Assets/GameMain/Scripts/Procedure/ProcedureSettlement.cs:82) 流程

**第十二步：清理资源**
- [`ProcedureSettlement.OnLeave()`](Assets/GameMain/Scripts/Procedure/ProcedureSettlement.cs:86) 被调用
- 取消事件订阅，关闭结算界面
- 清理 FSM 中的结算数据

---

## 二、架构设计意图

### 2.1 为什么在 GameBase 中添加数据收集字段？

**设计意图：**

1. **职责归属合理**
   - [`GameBase`](Assets/GameMain/Scripts/Game/GameBase.cs:14) 是游戏逻辑的基类，负责管理游戏运行状态
   - 游戏数据（分数、击杀数等）是游戏逻辑的一部分，应该由游戏类管理
   - 这符合"谁产生数据，谁负责管理"的原则

2. **数据收集的实时性**
   - 游戏数据需要在游戏运行过程中实时更新
   - [`GameBase.Update()`](Assets/GameMain/Scripts/Game/GameBase.cs:103) 每帧被调用，可以持续累加存活时间
   - 击杀事件发生时可以立即更新击杀数和分数

3. **数据访问的便利性**
   - 通过 [`ProcedureMain.CurrentGame`](Assets/GameMain/Scripts/Procedure/ProcedureMain.cs:28) 静态属性可以全局访问当前游戏实例
   - 实体类（如 [`Asteroid`](Assets/GameMain/Scripts/Entity/EntityLogic/Asteroid.cs:16)、[`MyAircraft`](Assets/GameMain/Scripts/Entity/EntityLogic/MyAircraft.cs:13)）可以方便地调用游戏类的方法更新数据

4. **封装性和扩展性**
   - 提供 [`AddScore()`](Assets/GameMain/Scripts/Game/GameBase.cs:121)、[`AddKill()`](Assets/GameMain/Scripts/Game/GameBase.cs:129)、[`ResetCombo()`](Assets/GameMain/Scripts/Game/GameBase.cs:142) 等公共方法
   - 内部实现细节被隐藏，外部只需调用方法即可
   - 未来可以轻松扩展更多数据收集逻辑

### 2.2 为什么创建独立的 ProcedureSettlement 流程？

**设计意图：**

1. **符合 FSM 设计模式**
   - GameFramework 使用有限状态机（FSM）管理游戏流程
   - 每个流程（Procedure）代表游戏的一个独立状态
   - 结算是游戏的一个明确状态，应该有独立的流程

2. **单一职责原则**
   - [`ProcedureMain`](Assets/GameMain/Scripts/Procedure/ProcedureMain.cs:16) 专注于游戏逻辑的运行
   - [`ProcedureSettlement`](Assets/GameMain/Scripts/Procedure/ProcedureSettlement.cs:17) 专注于结算数据的展示
   - 职责分离使代码更清晰、更易维护

3. **生命周期管理**
   - 结算界面有独立的生命周期（打开、显示、关闭）
   - 通过 [`OnEnter()`](Assets/GameMain/Scripts/Procedure/ProcedureSettlement.cs:43)、[`OnUpdate()`](Assets/GameMain/Scripts/Procedure/ProcedureSettlement.cs:71)、[`OnLeave()`](Assets/GameMain/Scripts/Procedure/ProcedureSettlement.cs:86) 管理
   - 便于控制资源的加载和释放

4. **状态隔离**
   - 游戏运行状态和结算展示状态完全分离
   - 避免在游戏逻辑中混入 UI 展示代码
   - 便于后续扩展（如添加动画、音效等）

### 2.3 为什么使用 GameSettlementData 结构体？

**设计意图：**

1. **数据封装**
   - [`GameSettlementData`](Assets/GameMain/Scripts/Definition/DataStruct/GameSettlementData.cs:16) 将所有结算相关数据封装在一起
   - 使用 `struct` 而非 `class`，因为它是简单的数据容器，不需要继承
   - 使用 `readonly` 字段确保数据不可变，避免意外修改

2. **类型安全**
   - 明确定义了每个数据字段的类型和含义
   - 编译时就能发现类型错误，而不是运行时
   - 提供了清晰的 API（属性访问器）

3. **便于传递**
   - 结构体可以直接作为值类型传递
   - 通过 FSM 的 [`VarObject`](Assets/GameMain/Scripts/Procedure/ProcedureMain.cs:110) 包装后可以在流程间传递
   - 避免了使用字典或多个独立变量的复杂性

4. **可维护性**
   - 所有结算数据集中在一个地方定义
   - 添加新字段时只需修改这一个文件
   - 代码自文档化，通过字段名就能理解数据含义

### 2.4 新代码如何与原有代码交互？

**交互方式分析：**

1. **继承关系**
   - [`ProcedureSettlement`](Assets/GameMain/Scripts/Procedure/ProcedureSettlement.cs:17) 继承自 `ProcedureBase`
   - [`SettlementForm`](Assets/GameMain/Scripts/UI/SettlementForm.cs:18) 继承自 `UGuiForm`
   - 遵循 GameFramework 的框架规范，复用框架提供的基础功能

2. **事件系统交互**
   - 订阅 [`OpenUIFormSuccessEventArgs.EventId`](Assets/GameMain/Scripts/Procedure/ProcedureSettlement.cs:48) 事件
   - 通过事件系统解耦流程和 UI 模块
   - 当 UI 打开成功时，流程能够获得通知并保存 UI 引用

3. **FSM 数据共享**
   - 通过 [`procedureOwner.SetData()`](Assets/GameMain/Scripts/Procedure/ProcedureMain.cs:112) 和 [`GetData()`](Assets/GameMain/Scripts/Procedure/ProcedureSettlement.cs:53) 在流程间传递数据
   - FSM 作为数据中转站，实现流程间的数据共享
   - 使用 [`ReferencePool`](Assets/GameMain/Scripts/Procedure/ProcedureMain.cs:110) 管理对象生命周期

4. **静态属性访问**
   - 通过 [`ProcedureMain.CurrentGame`](Assets/GameMain/Scripts/Procedure/ProcedureMain.cs:28) 静态属性访问当前游戏实例
   - 实体类可以直接调用游戏类的方法更新数据
   - 这是一种便捷的全局访问方式，但需要注意生命周期管理

5. **UI 组件系统**
   - 通过 [`GameEntry.UI.OpenUIForm()`](Assets/GameMain/Scripts/Procedure/ProcedureSettlement.cs:68) 打开界面
   - 通过 [`GameEntry.UI.CloseUIForm()`](Assets/GameMain/Scripts/Procedure/ProcedureSettlement.cs:94) 关闭界面
   - 使用框架的 UI 管理系统，自动处理 UI 的生命周期

6. **流程切换机制**
   - 通过 [`ChangeState<T>()`](Assets/GameMain/Scripts/Procedure/ProcedureSettlement.cs:82) 方法切换流程
   - FSM 自动调用当前流程的 `OnLeave()` 和下一个流程的 `OnEnter()`
   - 确保流程切换的原子性和一致性

---

## 三、代码变更点详解

### 3.1 新增文件

#### 文件：GameSettlementData.cs

**位置：** [`Assets/GameMain/Scripts/Definition/DataStruct/GameSettlementData.cs`](Assets/GameMain/Scripts/Definition/DataStruct/GameSettlementData.cs:1)

**关键代码及逐行注释：**

```csharp
// 第 8 行：引入 System.Runtime.InteropServices 命名空间
// 用于使用 StructLayout 特性优化结构体内存布局
using System.Runtime.InteropServices;

// 第 15-16 行：使用 StructLayout 特性，让编译器自动选择最优的内存布局
// LayoutKind.Auto 允许运行时重新排列字段以获得最佳性能
[StructLayout(LayoutKind.Auto)]
public struct GameSettlementData
{
    // 第 18-21 行：使用 readonly 修饰符定义私有字段
    // readonly 确保字段只能在构造函数中赋值，之后不可修改，保证数据不可变性
    private readonly int m_Score;          // 本局得分
    private readonly int m_KillCount;      // 击杀数量
    private readonly float m_SurvivalTime; // 存活时间（秒）
    private readonly int m_MaxCombo;       // 最大连击数

    // 第 30-36 行：构造函数，初始化所有字段
    // 使用构造函数确保数据完整性，避免部分初始化的问题
    public GameSettlementData(int score, int killCount, float survivalTime, int maxCombo)
    {
        m_Score = score;
        m_KillCount = killCount;
        m_SurvivalTime = survivalTime;
        m_MaxCombo = maxCombo;
    }

    // 第 41-47 行：提供只读属性访问器
    // 外部只能读取数据，不能修改，保证数据安全性
    public int Score
    {
        get { return m_Score; }
    }
    
    // 其他属性类似，都是只读访问器...
}
```

**设计亮点：**
- 使用 `struct` 而非 `class`，因为这是简单的数据传输对象（DTO）
- 使用 `readonly` 确保数据不可变性
- 使用属性而非公共字段，符合封装原则
- 提供完整的构造函数，避免部分初始化

---

#### 文件：ProcedureSettlement.cs

**位置：** [`Assets/GameMain/Scripts/Procedure/ProcedureSettlement.cs`](Assets/GameMain/Scripts/Procedure/ProcedureSettlement.cs:1)

**关键代码及逐行注释：**

```csharp
// 第 17 行：继承 ProcedureBase，成为 GameFramework 流程系统的一部分
public class ProcedureSettlement : ProcedureBase
{
    // 第 19-20 行：私有字段，管理流程状态
    private bool m_GotoMenu = false;                // 标记是否需要返回菜单
    private SettlementForm m_SettlementForm = null; // 保存结算界面引用

    // 第 33-36 行：公共方法，供 UI 调用以触发返回菜单
    // 使用标记而非直接切换流程，因为流程切换只能在 OnUpdate 中进行
    public void GotoMenu()
    {
        m_GotoMenu = true;  // 设置标记，在 OnUpdate 中检测并执行切换
    }

    // 第 43-69 行：进入流程时的初始化逻辑
    protected override void OnEnter(ProcedureOwner procedureOwner)
    {
        base.OnEnter(procedureOwner);

        // 第 48 行：订阅 UI 打开成功事件
        // 当结算界面打开成功后，我们需要保存其引用以便后续管理
        GameEntry.Event.Subscribe(OpenUIFormSuccessEventArgs.EventId, OnOpenUIFormSuccess);

        m_GotoMenu = false;  // 重置返回菜单标记

        // 第 53 行：从 FSM 中获取结算数据
        // ProcedureMain 在切换流程前已经将数据存入 FSM
        VarObject varObject = procedureOwner.GetData<VarObject>("SettlementData");
        GameSettlementData settlementData;

        // 第 56-65 行：数据验证和容错处理
        // 如果数据无效，使用默认值并记录警告，避免程序崩溃
        if (varObject == null || varObject.Value == null)
        {
            Log.Warning("Settlement data is invalid, using default values.");
            settlementData = new GameSettlementData(0, 0, 0f, 0);
        }
        else
        {
            settlementData = (GameSettlementData)varObject.Value;
        }

        // 第 68 行：打开结算界面，将数据作为 userData 传递
        // UI 系统会自动调用 SettlementForm.OnOpen(userData)
        GameEntry.UI.OpenUIForm(UIFormId.SettlementForm, settlementData);
    }

    // 第 71-84 行：每帧更新逻辑
    protected override void OnUpdate(ProcedureOwner procedureOwner, float elapseSeconds, float realElapseSeconds)
    {
        base.OnUpdate(procedureOwner, elapseSeconds, realElapseSeconds);

        // 第 76 行：检测返回菜单标记
        if (m_GotoMenu)
        {
            // 第 79 行：设置下一个场景 ID
            // ProcedureChangeScene 会读取这个数据来加载对应场景
            procedureOwner.SetData<VarInt32>("NextSceneId", GameEntry.Config.GetInt("Scene.Menu"));
            
            // 第 82 行：切换到场景切换流程
            // 这会触发当前流程的 OnLeave() 和下一个流程的 OnEnter()
            ChangeState<ProcedureChangeScene>(procedureOwner);
        }
    }

    // 第 86-105 行：离开流程时的清理逻辑
    protected override void OnLeave(ProcedureOwner procedureOwner, bool isShutdown)
    {
        // 第 89 行：取消事件订阅，避免内存泄漏
        GameEntry.Event.Unsubscribe(OpenUIFormSuccessEventArgs.EventId, OnOpenUIFormSuccess);

        // 第 92-96 行：关闭结算界面
        // 使用 UI 组件的 CloseUIForm 方法，而不是直接调用 UI 的 Close 方法
        if (m_SettlementForm != null)
        {
            GameEntry.UI.CloseUIForm(m_SettlementForm);
            m_SettlementForm = null;
        }

        // 第 99-102 行：清理 FSM 中的结算数据
        // 避免数据残留影响下次游戏
        if (procedureOwner.HasData("SettlementData"))
        {
            procedureOwner.RemoveData("SettlementData");
        }

        base.OnLeave(procedureOwner, isShutdown);
    }

    // 第 112-121 行：处理 UI 打开成功事件
    private void OnOpenUIFormSuccess(object sender, GameEventArgs e)
    {
        OpenUIFormSuccessEventArgs ne = (OpenUIFormSuccessEventArgs)e;
        
        // 第 115-118 行：检查是否是结算界面
        // 因为可能有其他 UI 也在打开，需要过滤
        if (ne.UIForm.UIFormAssetName != AssetUtility.GetUIFormAsset("SettlementForm"))
        {
            return;
        }

        // 第 120 行：保存结算界面引用，用于后续关闭
        m_SettlementForm = (SettlementForm)ne.UIForm.Logic;
    }
}
```

**设计亮点：**
- 完整的生命周期管理（OnEnter、OnUpdate、OnLeave）
- 事件驱动的 UI 管理，解耦流程和 UI
- 完善的错误处理和数据验证
- 使用标记模式处理异步操作

---

#### 文件：SettlementForm.cs

**位置：** [`Assets/GameMain/Scripts/UI/SettlementForm.cs`](Assets/GameMain/Scripts/UI/SettlementForm.cs:1)

**关键代码及逐行注释：**

```csharp
// 第 18 行：继承 UGuiForm，成为 GameFramework UI 系统的一部分
public class SettlementForm : UGuiForm
{
    // 第 20-33 行：使用 SerializeField 标记的 UI 组件字段
    // 这些字段会在 Unity 编辑器中显示，可以拖拽绑定 UI 组件
    [SerializeField]
    private Text m_ScoreText = null;        // 显示分数的文本
    [SerializeField]
    private Text m_KillCountText = null;    // 显示击杀数的文本
    [SerializeField]
    private Text m_SurvivalTimeText = null; // 显示存活时间的文本
    [SerializeField]
    private Text m_MaxComboText = null;     // 显示最大连击的文本
    [SerializeField]
    private Button m_BackToMenuButton = null; // 返回菜单按钮

    // 第 35 行：保存流程引用，用于回调通知流程
    private ProcedureSettlement m_ProcedureSettlement = null;

    // 第 38-50 行：UI 初始化方法
    // 使用条件编译指令兼容不同 Unity 版本
    protected override void OnInit(object userData)
    {
        base.OnInit(userData);

        // 第 46-49 行：绑定按钮点击事件
        // 使用 AddListener 添加事件监听器
        if (m_BackToMenuButton != null)
        {
            m_BackToMenuButton.onClick.AddListener(OnBackToMenuButtonClick);
        }
    }

    // 第 53-96 行：UI 打开时的逻辑
    protected override void OnOpen(object userData)
    {
        base.OnOpen(userData);

        // 第 61 行：接收并转换结算数据
        // userData 是 OpenUIForm 时传递的参数
        GameSettlementData settlementData = (GameSettlementData)userData;
        
        // 第 62-66 行：数据验证，确保数据有效
        if (userData == null)
        {
            Log.Warning("Settlement data is invalid.");
            return;
        }

        // 第 69-74 行：获取流程引用
        // 通过 GameEntry.Procedure.CurrentProcedure 获取当前流程
        // 转换为 ProcedureSettlement 类型以调用其公共方法
        m_ProcedureSettlement = GameEntry.Procedure.CurrentProcedure as ProcedureSettlement;
        if (m_ProcedureSettlement == null)
        {
            Log.Warning("Current procedure is not ProcedureSettlement.");
            return;
        }

        // 第 77-95 行：显示结算数据
        // 使用 Utility.Text.Format 格式化文本，支持多语言
        if (m_ScoreText != null)
        {
            // 第 79 行：显示分数，使用格式化字符串
            m_ScoreText.text = Utility.Text.Format("得分: {0}", settlementData.Score);
        }

        if (m_KillCountText != null)
        {
            // 第 84 行：显示击杀数
            m_KillCountText.text = Utility.Text.Format("击杀: {0}", settlementData.KillCount);
        }

        if (m_SurvivalTimeText != null)
        {
            // 第 89 行：显示存活时间，格式化为 "X.X秒"
            // {0:F1} 表示保留一位小数
            m_SurvivalTimeText.text = Utility.Text.Format("存活时间: {0:F1}秒", settlementData.SurvivalTime);
        }

        if (m_MaxComboText != null)
        {
            // 第 94 行：显示最大连击数
            m_MaxComboText.text = Utility.Text.Format("最大连击: {0}", settlementData.MaxCombo);
        }
    }

    // 第 99-106 行：UI 关闭时的清理逻辑
    protected override void OnClose(bool isShutdown, object userData)
    {
        // 第 104 行：清空流程引用，避免悬挂引用
        m_ProcedureSettlement = null;
        base.OnClose(isShutdown, userData);
    }

    // 第 111-117 行：按钮点击事件处理
    private void OnBackToMenuButtonClick()
    {
        // 第 113-116 行：通知流程返回菜单
        // 不直接切换流程，而是通知流程，由流程决定何时切换
        if (m_ProcedureSettlement != null)
        {
            m_ProcedureSettlement.GotoMenu();
        }
    }
}
```

**设计亮点：**
- 使用 Unity 的序列化系统绑定 UI 组件
- 完整的生命周期管理（OnInit、OnOpen、OnClose）
- 数据验证和错误处理
- 使用回调模式与流程通信，保持解耦

---

### 3.2 修改的文件

#### 文件：GameBase.cs

**修改位置：** [`Assets/GameMain/Scripts/Game/GameBase.cs`](Assets/GameMain/Scripts/Game/GameBase.cs:1)

**新增代码及逐行注释：**

```csharp
// 第 35-40 行：新增游戏数据收集字段
private int m_Score = 0;           // 当前分数，初始化为 0
private int m_KillCount = 0;       // 击杀数量，初始化为 0
private float m_SurvivalTime = 0f; // 存活时间（秒），初始化为 0
private int m_CurrentCombo = 0;    // 当前连击数，初始化为 0
private int m_MaxCombo = 0;        // 最大连