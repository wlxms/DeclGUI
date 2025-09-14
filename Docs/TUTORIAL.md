# DeclGUI 新手入门指南

本文档将引导您快速上手使用 DeclGUI 框架，通过实际示例帮助您理解框架的核心概念和使用方法。

## 目录
1. [简介](#简介)
2. [安装和设置](#安装和设置)
3. [创建第一个 DeclGUI 窗口](#创建第一个-declgui-窗口)
4. [基础组件使用](#基础组件使用)
5. [布局系统](#布局系统)
6. [状态管理](#状态管理)
7. [上下文机制](#上下文机制)
8. [高级组件使用](#高级组件使用)
9. [样式定制](#样式定制)
10. [最佳实践](#最佳实践)
11. [常见问题](#常见问题)
12. [下一步](#下一步)

## 简介

DeclGUI 是一个基于声明式编程范式的 Unity GUI 框架，专门为 Unity Editor 扩展开发设计。它借鉴了 React 等现代前端框架的设计理念，提供了声明式、组件化的 UI 开发方式。

## 安装和设置

1. 将 DeclGUI 框架导入到您的 Unity 项目中
2. 确保项目中包含 `DeclGUI` 文件夹及其子目录
3. 在您的 Editor 脚本中添加 `using DeclGUI.Components;` 和 `using DeclGUI.Core;` 引用

## 创建第一个 DeclGUI 窗口

让我们从创建一个简单的 Editor 窗口开始：

```csharp
using DeclGUI.Core;
using DeclGUI.Components;
using DeclGUI.Editor.Renderers;
using UnityEditor;
using UnityEngine;

public class MyFirstDeclGUIWindow : EditorWindow
{
    private EditorRenderManager _renderManager;
    
    [MenuItem("Tools/My First DeclGUI Window")]
    public static void ShowWindow()
    {
        GetWindow<MyFirstDeclGUIWindow>("My First DeclGUI Window");
    }
    
    void OnGUI()
    {
        _renderManager ??= new EditorRenderManager();
        
        // 创建一个简单的 UI
        var ui = new Ver(
            new Label("欢迎使用 DeclGUI!"),
            new Spc(10),
            new Button("点击我", OnButtonClicked)
        );
        
        // 渲染 UI
        _renderManager.RenderDOM(ui);
    }
    
    private void OnButtonClicked()
    {
        Debug.Log("按钮被点击了!");
    }
}
```

## 基础组件使用

### 文本输入框和状态管理

```csharp
private string _inputText = "初始文本";

void OnGUI()
{
    _renderManager ??= new EditorRenderManager();
    
    var ui = new Ver(
        new TextField(_inputText, OnInputChanged),
        new Spc(10),
        new Label($"当前输入: {_inputText}")
    );
    
    _renderManager.RenderDOM(ui);
}

private void OnInputChanged(string newValue)
{
    _inputText = newValue;
    Repaint(); // 重新绘制窗口以更新 UI
}
```

### 滑动条组件

```csharp
private float _sliderValue = 0.5f;

void OnGUI()
{
    _renderManager ??= new EditorRenderManager();
    
    var ui = new Hor(
        new Label("滑动条:"),
        new Spc(5),
        new Slider(_sliderValue, 0f, 1f, OnSliderChanged),
        new Spc(5),
        new Label($"{_sliderValue:F2}")
    );
    
    _renderManager.RenderDOM(ui);
}

private void OnSliderChanged(float newValue)
{
    _sliderValue = newValue;
    Repaint();
}
```

## 布局系统

DeclGUI 提供了两种主要的布局容器：

### 垂直布局 (Ver)

```csharp
var ui = new Ver {
    new Label("第一个元素"),
    new Label("第二个元素"),
    new Label("第三个元素")
};
```

### 水平布局 (Hor)

```csharp
var ui = new Hor {
    new Label("左侧"),
    new Spc(10), // 添加间距
    new Label("右侧")
};
```

### 嵌套布局

```csharp
var ui = new Ver {
    new Label("顶部"),
    new Hor {
        new Label("左下"),
        new Label("右下")
    }
};
```

## 状态管理

DeclGUI 提供了内置的状态管理机制，通过 `StatefulButton` 等组件实现：

```csharp
var ui = new Ver {
    new StatefulButton("有状态按钮", state => {
        Debug.Log($"按钮被点击! 点击次数: {state.ClickCount}");
        Repaint();
    })
};
```

## 上下文机制

DeclGUI 的上下文机制允许您在组件树中传递数据，类似于 React 的 Context：

### 提供上下文

```csharp
var ui = new ContextBatch(new ReadOnly(true), new UserName("John")) {
    // 子元素可以消费这些上下文
    BuildUI()
};
```

### 消费上下文

```csharp
var ui = new ContextConsumer(context => {
    bool isReadOnly = context.Get<ReadOnly>().Value;
    string userName = context.Get<UserName>().Value;
    
    return new Label($"用户: {userName}, 只读: {isReadOnly}");
});
```

## 高级组件使用

### ECanvas 和 AbsolutePanel 组件

ECanvas 组件允许您在自动布局系统中创建固定位置的子元素：

```csharp
var ui = new ECanvas(
    new AbsolutePanel(new Vector2(100, 50), 
        new Button("固定位置按钮", OnButtonClick)
    ),
    new AbsolutePanel(new Vector2(200, 100),
        new Label("另一个固定位置元素")
    )
);
```

### ScrollRect 组件

ScrollRect 组件用于创建可滚动的 UI 区域：

```csharp
private Vector2 _scrollPosition = Vector2.zero;

var ui = new ScrollRect(
    _scrollPosition,
    new Ver {
        new Label("很长的内容..."),
        new Label("需要滚动才能看到..."),
        new Label("内容结束")
    },
    newPos => {
        _scrollPosition = newPos;
        Repaint();
    }
);
```

### ObjectField 组件

ObjectField 组件用于选择 Unity 对象：

```csharp
private UnityEngine.Object _selectedObject;

var ui = new ObjectField<UnityEngine.Object>(
    _selectedObject,
    newObj => {
        _selectedObject = newObj;
        Repaint();
    },
    true // 允许场景对象
);
```

## 样式定制

DeclGUI 支持通过 `DeclStyle` 对组件进行样式定制：

```csharp
new Label("彩色文本", DeclStyle.WithColor(Color.red))
new Button("大按钮", OnClick, DeclStyle.WithSize(200, 40))
```

### 高级样式定制

```csharp
// 创建自定义样式
var customStyle = DeclStyle.WithColor(Color.blue).SetSize(150, 30);

new Button("自定义样式按钮", OnClick, customStyle)
```

## 最佳实践

1. **保持组件简单**：每个组件应该只负责一个特定的功能
2. **使用纯函数**：Render 方法应该是纯函数，避免副作用
3. **合理使用状态**：只在必要时使用有状态组件
4. **善用上下文**：在需要跨层级传递数据时使用上下文机制
5. **及时重绘**：在状态改变后调用 `Repaint()` 更新 UI
6. **合理使用布局组件**：根据需要选择合适的布局组件
7. **避免过度嵌套**：保持 UI 结构清晰易懂

## 常见问题

### 为什么 UI 没有更新？

确保在状态改变后调用了 `Repaint()` 方法。

### 如何处理复杂的 UI 逻辑？

将 UI 拆分为多个小的组件函数，每个函数负责渲染一部分 UI。

### 如何优化性能？

1. 避免在 Render 方法中创建新对象
2. 合理使用状态管理，避免不必要的状态更新
3. 使用对象池管理大量元素

### 如何调试 DeclGUI 应用？

1. 使用 Debug.Log 输出调试信息
2. 检查组件的 Render 方法是否正确实现
3. 确保状态更新逻辑正确

## 下一步

- 查看 [EDITOR-COMPONENTS.md](./EDITOR-COMPONENTS.md) 了解所有可用组件
- 阅读 [DeclGUI_Framework_Analysis.md](./DeclGUI_Framework_Analysis.md) 深入理解框架设计
- 查看示例代码了解实际应用