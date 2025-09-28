# DeclGUI 框架完整技术文档与用户引导

## 1. 框架总览

### 1.1 框架简介

DeclGUI是一个基于声明式编程范式的Unity GUI框架，专门为Unity Editor扩展开发设计。它借鉴了React等现代前端框架的设计理念，提供了声明式、组件化的UI开发方式。该框架旨在简化Unity Editor界面开发，提供更好的开发体验和性能表现。

### 1.2 核心设计思想

- **声明式UI范式**：开发者描述UI应该是什么样子，而不是如何创建和更新UI
- **数据驱动理念**：UI状态变化自动反映到界面更新
- **组件化架构**：通过组合简单组件构建复杂界面
- **类型安全**：编译时类型检查，减少运行时错误
- **性能优化**：使用值类型、对象池等技术减少GC压力

### 1.3 适用场景

- Unity Editor工具开发
- 复杂的编辑器界面
- 需要高度自定义的UI
- 性能敏感的编辑器扩展
- 需要状态管理的UI
- 需要上下文传递的UI

## 2. 核心机制深入解析

### 2.1 元素与渲染器分离设计

DeclGUI采用元素与渲染器分离的架构设计，实现了关注点分离：

**IElement接口**：
```csharp
public interface IElement
{
    IElement Render();
}
```

**IElementRenderer接口**：
```csharp
public interface IElementRenderer
{
    void Render(RenderManager mgr, in IElement element, in IDeclStyle style);
    Vector2 CalculateSize(RenderManager mgr, in IElement element, in IDeclStyle style);
}
```

这种设计的优势：
- **可扩展性**：可以轻松添加新的渲染目标（如不同的UI系统）
- **可测试性**：渲染逻辑与元素定义分离
- **性能优化**：渲染器可以针对特定元素类型进行优化

### 2.2 状态管理机制

DeclGUI提供了完整的状态管理解决方案，包括：

**状态管理器接口**：
```csharp
public interface IStateManager
{
    IElementState GetOrCreateState(in IElementWithKey element);
    void UpdateState(in IStatefulElement element, object state);
    void RemoveState(in IElementWithKey element);
    bool HasState(in IElementWithKey element);
    void ResetCounters();
    void CleanupUnusedStates(int framesToKeep = 2);
}
```

**状态栈管理**：
- `StateStackManager`：管理状态管理器的栈结构
- `StateManagerStorage`：管理状态存储器的层级关系
- `ContainerState`：容器状态实现，负责管理容器内子元素的状态

**状态生命周期**：
1. 元素首次渲染时创建状态
2. 状态在状态管理器中存储
3. 每帧检查状态使用情况
4. 未使用的状态在一定帧数后清理

### 2.3 上下文栈机制

上下文栈实现了数据的跨层级传递：

```csharp
public interface IContextProvider : IElement, IEnumerable<IElement>
{
    IElement Child { get; }
}

public interface IContextReader
{
    T Get<T>() where T : struct, IContextProvider;
    bool TryGet<T>(out T value) where T : struct, IContextProvider;
    bool Has<T>() where T : struct, IContextProvider;
}
```

**ContextStack**管理不同类型的上下文：
- 支持多类型上下文栈
- 类型安全的上下文访问
- 自动清理未使用的上下文

### 2.4 事件系统

事件系统通过`DeclEvent`结构和`EventDispatcher`实现：

```csharp
public struct DeclEvent
{
    public Action OnClick;
    public Action OnPressDown;
    public Action OnPressUp;
    // ... 其他事件
}
```

**事件处理流程**：
1. 渲染器在渲染时记录元素位置
2. 事件分发器接收Unity事件
3. 根据鼠标位置判断事件目标
4. 触发相应事件回调

### 2.5 样式与主题系统

#### 2.5.1 样式系统架构

**IDeclStyle接口**：
```csharp
public interface IDeclStyle
{
    IDeclStyle GetStyleForState(IElementState elementState);
    IDeclStyle Merge(IDeclStyle other);
    Color? Color { get; }
    float? Width { get; }
    float? Height { get; }
    // ... 其他样式属性
}
```

**DeclStyle结构体**：
- 支持颜色、尺寸、布局、文本、边框等多种样式属性
- 使用`StyleProperty<T>`统一管理属性值
- 支持直接值、属性引用和空值三种状态

**StyleProperty<T>**：
```csharp
public struct StyleProperty<T>
{
    private PropertyValueType _valueType;
    private T _directValue;
    private string _propertyRef;
    
    public static StyleProperty<T> Direct(T value);
    public static StyleProperty<T> Ref(string propertyRef);
    public static StyleProperty<T> None();
}
```

#### 2.5.2 主题系统

**DeclThemeManager**：
- 全局主题管理器
- 样式集解析和缓存
- 伪类样式处理

**DeclTheme**：
- ScriptableObject实现，支持可视化编辑
- 样式集管理和自动收集
- 主题属性系统

**DeclStyleSet**：
- 支持伪类样式的样式集
- 包含Normal、Hover、Active、Focus、Disabled等状态
- 支持过渡效果配置

## 3. 模块详解

### 3.1 运行时组件模块

#### 3.1.1 基础组件

**Button**：可点击按钮组件
```csharp
public struct Button : IEventfulElement, IStylefulElement
{
    public string Text { get; }
    public DeclStyle? Style { get; }
    public DeclEvent Events { get; set; }
}
```

**StatefulButton**：有状态按钮组件
```csharp
public struct StatefulButton : IElement<ButtonState>, IEventfulElement
{
    public string Text { get; }
    public ButtonState State { get; set; }
}
```

**Hor/Ver**：布局容器组件
```csharp
public struct Hor : IContainerElement, IEventfulElement, IStylefulElement
{
    private IElement[] _elements;
    private int _count;
    private int _capacity;
}
```

#### 3.1.2 高级组件

**ContextBatch**：批量上下文提供者
```csharp
public struct ContextBatch : IContextProvider, IElement, IEnumerable<IElement>
{
    private readonly IContextProvider[] _contexts;
    public IElement Child { get; }
}
```

**ContextConsumer**：上下文消费者
```csharp
public struct ContextConsumer : IContextConsumer
{
    public Func<IContextReader, IElement> Render { get; }
}
```

#### 3.1.3 实际组件列表

DeclGUI框架包含以下组件：

- `Runtime/Components/Hor.cs` - 水平布局容器
- `Runtime/Components/Ver.cs` - 垂直布局容器
- `Runtime/Components/Label.cs` - 标签组件
- `Runtime/Components/Button.cs` - 按钮组件
- `Runtime/Components/TextField.cs` - 文本输入框
- `Runtime/Components/Slider.cs` - 滑动条
- `Runtime/Components/StatefulButton.cs` - 有状态按钮
- `Runtime/Components/LongPressButton.cs` - 长按按钮
- `Runtime/Components/Spc.cs` - 间距组件
- `Runtime/Components/ObjectField.cs` - 对象选择字段
- `Runtime/Components/ContextBatch.cs` - 批量上下文容器
- `Runtime/Components/ContextConsumer.cs` - 上下文消费者
- `Runtime/Components/DisableContext.cs` - 禁用上下文
- `Runtime/Components/DisableGroup.cs` - 禁用组
- `Runtime/Components/UserName.cs` - 用户名上下文
- `Runtime/Components/ECanvas.cs` - 编辑器画布组件
- `Runtime/Components/AbsolutePanel.cs` - 绝对定位面板组件
- `Runtime/Components/FixableSpace.cs` - 可伸缩空白空间组件
- `Runtime/Components/ScrollRect.cs` - 滚动视图容器组件
- `Runtime/Components/ColorField.cs` - 颜色选择器组件
- `Runtime/Components/CurveField.cs` - 曲线编辑器组件
- `Runtime/Components/EnumPopup.cs` - 枚举下拉选择框组件
- `Runtime/Components/FloatField.cs` - 浮点数字段组件
- `Runtime/Components/IntField.cs` - 整数字段组件
- `Runtime/Components/MinMaxSlider.cs` - 最小-最大范围滑块组件
- `Runtime/Components/Popup.cs` - 下拉选择框组件
- `Runtime/Components/TagField.cs` - 标签选择器组件
- `Runtime/Components/Toggle.cs` - 开关/复选框组件
- `Runtime/Components/Vector2Field.cs` - Vector2输入字段组件
- `Runtime/Components/Vector3Field.cs` - Vector3输入字段组件
- `Runtime/Components/Vector4Field.cs` - Vector4输入字段组件
- `Runtime/Components/Advanced/Panel.cs` - 面板组件

### 3.2 核心系统模块

#### 3.2.1 渲染管理器

**RenderManager**：渲染管理器基类
- 管理渲染器注册和查找
- 状态栈和上下文栈管理
- 事件处理和样式解析
- 样式缓存管理

**EditorRenderManager**：编辑器渲染管理器
- 继承RenderManager，针对Unity Editor优化
- 实现具体的GUI渲染逻辑

#### 3.2.2 状态管理系统

**ElementState**：元素状态实现
- 管理悬停、聚焦、禁用等多种状态
- 支持状态标志组合
- 类型安全的状态管理

**ContainerState**：容器状态实现
- 管理容器内子元素的状态
- 类型安全检查
- 状态清理机制

### 3.3 编辑器集成模块

**DeclGUIWindow**：声明式GUI窗口基类
```csharp
public abstract class DeclGUIWindow : EditorWindow, IElement
{
    protected EditorRenderManager RenderManager { get; }
    public abstract IElement Render();
}
```

## 4. 实用指南

### 4.1 创建自定义声明式UI组件

创建自定义组件需要遵循以下步骤：

1. **定义元素结构**：
```csharp
public struct MyCustomComponent : IElement, IStylefulElement
{
    public string Text { get; }
    public DeclStyle? Style { get; }
    public DeclEvent Events { get; set; }
    
    public MyCustomComponent(string text, DeclStyle? style = null)
    {
        Text = text;
        Style = style;
        Events = new DeclEvent();
    }
    
    public IElement Render() => null;
}
```

2. **实现渲染器**：
```csharp
public class MyCustomComponentRenderer : EditorElementRenderer<MyCustomComponent>
{
    public override void Render(RenderManager mgr, in MyCustomComponent element, in IDeclStyle style)
    {
        var editorMgr = mgr as EditorRenderManager;
        var currentStyle = style ?? element.Style;
        var guiStyle = editorMgr.ApplyStyle(currentStyle, GUI.skin.label);
        
        GUILayout.Label(element.Text, guiStyle);
    }
    
    public override Vector2 CalculateSize(RenderManager mgr, in MyCustomComponent element, in IDeclStyle style)
    {
        var editorMgr = mgr as EditorRenderManager;
        var currentStyle = style ?? element.Style;
        var guiStyle = editorMgr.ApplyStyle(currentStyle, GUI.skin.label);
        
        return guiStyle.CalcSize(new GUIContent(element.Text));
    }
}
```

3. **注册渲染器**：确保渲染器被自动发现和注册

### 4.2 编辑器集成配置与调试

**窗口创建**：
```csharp
public class MyCustomWindow : DeclGUIWindow
{
    private string inputText = "";
    
    public override IElement Render()
    {
        return new Ver(
            new Label("自定义窗口"),
            new TextField(inputText, OnTextChanged),
            new Spc(10),
            new Button("提交", OnSubmit)
        );
    }
    
    private void OnTextChanged(string newText)
    {
        inputText = newText;
        Repaint();
    }
    
    private void OnSubmit()
    {
        Debug.Log($"提交内容: {inputText}");
    }
}
```

**调试技巧**：
- 使用`Repaint()`强制重绘窗口
- 通过`Debug.Log`输出状态变化信息
- 检查渲染器的异常处理机制

### 4.3 性能优化建议

1. **对象池使用**：
- `ArrayPoolHelper`管理数组内存
- 避免频繁创建和销毁对象
- 合理设置对象池大小

2. **渲染效率提升**：
- 使用值类型组件减少GC压力
- 合理使用样式缓存
- 避免在Render方法中创建新对象

3. **状态管理优化**：
- 及时清理未使用的状态
- 避免过度使用有状态组件
- 合理使用状态栈层级

## 5. 陷阱与规避

### 5.1 状态管理陷阱

**内存泄漏风险**：
- 问题：未及时清理的状态可能造成内存泄漏
- 解决：使用`CleanupUnusedStates`方法定期清理

**状态同步问题**：
- 问题：状态更新不同步可能导致UI显示异常
- 解决：确保状态更新后调用`Repaint()`

### 5.2 上下文滥用问题

**依赖混乱**：
- 问题：过度使用上下文可能导致组件依赖关系复杂
- 解决：合理设计上下文层级，避免不必要的上下文传递

**上下文生命周期**：
- 问题：上下文生命周期管理不当可能导致空引用
- 解决：使用上下文栈的压入/弹出机制

### 5.3 样式覆盖冲突

**优先级问题**：
- 问题：样式覆盖顺序不当可能导致意外的样式表现
- 解决：理解样式合并优先级：直接样式 > 伪类样式 > 默认样式

**缓存问题**：
- 问题：样式缓存可能导致样式更新不及时
- 解决：主题变更时清理样式缓存

## 6. 最佳实践总结

### 6.1 组件化开发原则

1. **单一职责原则**：每个组件只负责一个特定的UI功能
2. **组合优于继承**：通过组合简单组件构建复杂UI
3. **接口隔离**：清晰的接口定义，便于扩展和维护

### 6.2 状态管理最佳实践

1. **状态最小化**：只存储必要的状态信息
2. **状态集中管理**：使用统一的状态管理机制
3. **状态生命周期**：合理管理状态的创建和销毁

### 6.3 样式与逻辑分离

1. **样式与逻辑分离**：保持UI逻辑与样式定义分离
2. **主题化设计**：使用主题系统管理全局样式
3. **组件样式独立**：组件样式定义与全局样式解耦

### 6.4 性能优化策略

1. **值类型优先**：使用struct减少堆分配
2. **对象池管理**：合理使用对象池减少GC压力
3. **事件处理优化**：避免不必要的事件监听

通过遵循这些最佳实践，开发者可以构建出高效、可维护、可扩展的DeclGUI应用程序。DeclGUI框架为Unity Editor开发提供了现代化的声明式UI解决方案，具有良好的性能表现和开发体验。