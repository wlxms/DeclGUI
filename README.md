# DeclGUI 框架

[![Unity Version](https://img.shields.io/badge/Unity-2020.3%2B-blue.svg)](https://unity.com/)
[![License](https://img.shields.io/badge/license-MIT-green.svg)](LICENSE)

## 简介

DeclGUI 是一个基于声明式编程范式的 Unity GUI 框架，专门为 Unity Editor 扩展开发设计。它借鉴了 React 等现代前端框架的设计理念，提供了声明式、组件化的 UI 开发方式。

## 核心特性

- **声明式编程**：描述 UI 应该是什么样子，而不是如何创建和更新 UI
- **组件化架构**：通过组合简单的组件构建复杂的 UI
- **状态管理**：内置完善的状态管理机制
- **上下文传递**：优雅的上下文传递和消费方式
- **高性能**：精心设计的数据结构和算法，减少内存分配
- **类型安全**：编译时类型检查，减少运行时错误

## 文档导航

- [新手入门指南](./Docs/TUTORIAL.md) - 快速上手使用 DeclGUI
- [编辑器组件](./Docs/EDITOR-COMPONENTS.md) - 了解所有可用的 UI 组件及其用例
- [框架分析报告](./Docs/DeclGUI_Framework_Analysis.md) - 深入理解框架设计思想和架构
- [上下文机制设计](./Docs/ContextBatch_Design_Complete.md) - 详细了解上下文机制的设计和实现

## 快速开始

### 安装

将 DeclGUI 框架导入到您的 Unity 项目中，确保包含以下目录结构：

```
DeclGUI/
├── Runtime/           # 运行时核心代码
├── Editor/            # 编辑器实现
└── Docs/              # 文档
```

### 创建一个简单的窗口

```csharp
using DeclGUI.Core;
using DeclGUI.Components;
using DeclGUI.Editor.Renderers;
using UnityEditor;
using UnityEngine;

public class MyDeclGUIWindow : EditorWindow
{
    private EditorRenderManager _renderManager;
    
    [MenuItem("Tools/My DeclGUI Window")]
    public static void ShowWindow()
    {
        GetWindow<MyDeclGUIWindow>("My DeclGUI Window");
    }
    
    void OnGUI()
    {
        _renderManager ??= new EditorRenderManager();
        
        var ui = new Ver(
            new Label("欢迎使用 DeclGUI!"),
            new Spc(10),
            new Button("点击我", OnButtonClicked)
        );
        
        _renderManager.RenderDOM(ui);
    }
    
    private void OnButtonClicked()
    {
        Debug.Log("按钮被点击了!");
    }
}
```

## 核心概念

### 1. 声明式编程

DeclGUI 采用声明式编程范式，开发者只需描述 UI 应该是什么样子，框架会自动处理 UI 的创建和更新。

```csharp
// 声明式写法
new Ver {
    new Label("Hello"),
    new Button("Click", OnClick)
}
```

### 2. 组件化架构

所有 UI 元素都是组件，可以通过组合简单的组件构建复杂的 UI。

```csharp
new Hor {
    new Label("用户名:"),
    new TextField(_username, OnUsernameChanged)
}
```

### 3. 状态管理

DeclGUI 提供了内置的状态管理机制，支持有状态组件。

```csharp
new StatefulButton("点击计数", state => {
    Debug.Log($"点击次数: {state.ClickCount}");
})
```

### 4. 上下文机制

通过 ContextBatch 和 ContextConsumer 实现上下文的传递和消费。

```csharp
// 提供上下文
new ContextBatch(new ReadOnly(true)) {
    BuildUI()
}

// 消费上下文
new ContextConsumer(context => {
    bool isReadOnly = context.Get<ReadOnly>().Value;
    return new Label($"只读模式: {isReadOnly}");
})
```

## 架构设计

### 分层架构

```
DeclGUI
├── Runtime (运行时核心)
│   ├── Core (核心接口和基础组件)
│   └── Components (UI组件)
└── Editor (编辑器实现)
    ├── Renderers (具体渲染器)
    └── Examples (示例代码)
```

### 核心接口

- `IElement` - 基础元素接口
- `IContainerElement` - 容器元素接口
- `IStatefulElement` - 有状态元素接口
- `IElementRenderer` - 元素渲染器接口
- `IContextProvider` - 上下文提供者接口
- `IContextConsumer` - 上下文消费者接口

## 性能优化

- **值类型组件**：使用 struct 减少堆分配
- **对象池管理**：使用 ArrayPool 管理数组内存
- **最小化GC**：精心设计的数据结构减少垃圾回收
- **状态帧管理**：基于帧数的状态清理机制

## 适用场景

1. **Unity Editor工具开发** - 主要设计目标
2. **复杂的编辑器界面** - 声明式语法适合复杂UI
3. **需要高度自定义的UI** - 易于扩展和定制
4. **性能敏感的编辑器扩展** - 优化的内存管理
5. **需要状态管理的UI** - 内置完善的状态管理机制

## 贡献

欢迎提交 Issue 和 Pull Request 来改进 DeclGUI 框架。

## 许可证

MIT License