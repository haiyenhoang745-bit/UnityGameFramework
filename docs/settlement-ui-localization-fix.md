# 结算界面本地化配置修复指南

## 问题说明

结算界面的 Title 和 BackToMenuButton 显示 `<NoKey>`，这是因为 Unity 预制体中的 Text 组件配置了本地化键，但键值不正确或未配置。

## 解决方案

### 方案 A：使用固定文本（推荐，快速解决）

在 Unity 编辑器中：

1. **打开预制体**
   - 在 Project 窗口中找到 `Assets/GameMain/UI/UIForms/SettlementForm.prefab`
   - 双击打开预制体编辑模式

2. **修改 Title 文本**
   - 在 Hierarchy 中选择 Title 对象
   - 在 Inspector 中找到 Text 组件
   - 将 Text 字段改为：`游戏结算`
   - 取消勾选任何本地化相关的选项

3. **修改按钮文本**
   - 在 Hierarchy 中选择 BackToMenuButton/Text 对象
   - 在 Inspector 中找到 Text 组件
   - 将 Text 字段改为：`返回大厅`
   - 取消勾选任何本地化相关的选项

4. **保存预制体**
   - Ctrl+S 或 File → Save

### 方案 B：正确配置本地化（完整方案）

如果您的 SettlementForm 预制体使用了 `LocalizationText` 组件：

1. **检查组件类型**
   - 选择 Title 对象
   - 查看是否有 `LocalizationText` 或类似组件

2. **配置本地化键**
   - 如果有 LocalizationText 组件：
     - 将 Key 字段设置为：`Settlement.Title`
   - 如果只有普通 Text 组件：
     - 直接输入文本：`游戏结算`

3. **配置按钮文本**
   - 选择 BackToMenuButton/Text 对象
   - 如果有 LocalizationText 组件：
     - 将 Key 字段设置为：`Settlement.BackToMenuButton`
   - 如果只有普通 Text 组件：
     - 直接输入文本：`返回大厅`

## 本地化键已添加

我已经在本地化文件中添加了以下键值对：

### 简体中文 (ChineseSimplified/Dictionaries/Default.xml)
```xml
<String Key="Settlement.Title" Value="游戏结算" />
<String Key="Settlement.BackToMenuButton" Value="返回大厅" />
```

### 英文 (English/Dictionaries/Default.xml)
```xml
<String Key="Settlement.Title" Value="GAME SETTLEMENT" />
<String Key="Settlement.BackToMenuButton" Value="BACK TO MENU" />
```

## 验证步骤

1. 保存预制体修改
2. 运行游戏
3. 进入关卡并死亡
4. 检查结算界面是否正确显示文本

## 常见问题

### Q: 为什么其他界面没有这个问题？
A: 其他界面（如 MenuForm、SettingForm）的预制体已经正确配置了文本或本地化键。

### Q: 如果我想支持多语言怎么办？
A: 使用方案 B，确保预制体使用 LocalizationText 组件，并正确配置本地化键。

### Q: 修改后还是显示 <NoKey>？
A: 
1. 检查是否保存了预制体
2. 检查 Text 组件是否有其他脚本覆盖了文本
3. 尝试重启 Unity 编辑器
4. 检查本地化文件是否正确加载（查看 Console 是否有错误）

## 参考

- 本地化文件位置：`Assets/GameMain/Localization/*/Dictionaries/Default.xml`
- UI 预制体位置：`Assets/GameMain/UI/UIForms/SettlementForm.prefab`