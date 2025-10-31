# DeclGUI 缩进功能技术实现文档

## 1. 概述

本文档详细描述了 DeclGUI 框架中缩进功能的实现方案。该功能允许开发者通过声明式语法轻松实现元素的缩进效果，并支持嵌套缩进、动态缩进变化等高级特性。

## 2. 核心组件

### 2.1 IndentContext 结构体

```csharp
public struct IndentContext
{
    public int Level { get; }
    public float Size { get; }
    public IElement Child { get; private set; }
    
    public IndentContext(int level, float size, IElement child = null);
}
```

- **作用**: 存储当前的缩进级别和缩进大小
- **Level**: 当前缩进的层级（0表示无缩进）
- **Size**: 每个缩进级别的像素大小

### 2.2 Indent 和 DeIndent 组件

#### Indent 组件
```csharp
public struct Indent : IContextProvider, ISpecialContext
{
    public float? CustomSize { get; }
    public IElement Child { get; private set; }
}
```

- **作用**: 增加一层缩进
- **CustomSize**: 可选的自定义缩进大小，如果不设置则使用当前上下文中的默认大小

#### DeIndent 组件
```csharp
public struct DeIndent : IContextProvider, ISpecialContext
{
    public int Levels { get; }
    public IElement Child { get; private set; }
}
```

- **作用**: 减少指定层级的缩进
- **Levels**: 要减少的缩进层级数（默认为1）

### 2.3 ContextParam 系统

在 `IContainerElement` 接口中添加了对上下文参数的支持：

```csharp
public interface IContainerElement : IElement, IEnumerable<IElement>
{
    // ... 其他成员
    IReadOnlyList<IContextProvider> ContextParams { get; }
    IContainerElement WithContext(params IContextProvider[] contextParams);
}
```

## 3. 实现机制

### 3.1 RenderManager 的修改

1. **特殊上下文处理**: `RenderManager` 现在能够识别 `ISpecialContext` 接口的实现，并对 `Indent` 和 `DeIndent` 进行特殊处理

2. **上下文栈管理**: 在处理特殊上下文时，会生成实际的 `IndentContext` 对象并推入上下文栈

3. **容器上下文参数**: 在渲染容器时，会先处理容器的 `ContextParams`，然后渲染子元素

### 3.2 EditorRenderManager 的修改

`EditorRenderManager` 重写了 `RenderElement` 方法，使用 `EditorGUI.indentLevel` 来应用缩进效果：

```csharp
public override void RenderElement(in IElement element)
{
    if (ContextStack.TryGet<IndentContext>(out var indentContext) && indentContext.Level > 0)
    {
        EditorGUI.indentLevel += indentContext.Level;
        
        try
        {
            base.RenderElement(element);
        }
        finally
        {
            EditorGUI.indentLevel -= indentContext.Level;
        }
    }
    else
    {
        base.RenderElement(element);
    }
}
```

## 4. 使用示例

### 4.1 基本用法

```csharp
// 使用 Indent 组件增加缩进
var element = new Indent().WithChild(
    new Label("这是一个缩进的文本")
);

// 使用 DeIndent 组件减少缩进
var element2 = new DeIndent(2).WithChild(
    new Label("这个文本的缩进减少了2级")
);
```

### 4.2 嵌套缩进

```csharp
var nestedElement = new Indent().WithChild(
    new Ver(
        new Label("第一级缩进"),
        new Indent().WithChild(
            new Label("第二级缩进"),
            new Indent().WithChild(
                new Label("第三级缩进")
            )
        )
    )
);
```

### 4.3 使用容器的 ContextParam

```csharp
var container = new Ver()
    .WithContext(new IndentContext(2, 20f))  // 为容器设置上下文参数
    .Add(new Label("这些元素都会有2级缩进"));
```

### 4.4 响应式缩进

```csharp
// 缩进级别可以基于数据状态动态变化
var reactiveIndent = new Indent(dataModel.CurrentLevel).WithChild(
    new Label("缩进级别根据数据模型动态变化")
);
```

## 5. 性能优化

1. **上下文栈复用**: 通过上下文栈管理缩进状态，避免重复计算
2. **缓存机制**: 在渲染管理器中缓存样式和计算结果
3. **最小化重绘**: 只有在缩进级别发生变化时才更新渲染状态

## 6. API 设计原则

1. **声明式语法**: 使用声明式方式定义缩进，代码更清晰易懂
2. **一致性**: 与 DeclGUI 的其他组件保持一致的 API 设计
3. **可组合性**: Indent 和 DeIndent 组件可以任意组合使用
4. **响应式**: 缩进级别可以根据数据状态自动更新

## 7. 注意事项

1. **性能考虑**: 大量嵌套缩进可能影响渲染性能
2. **兼容性**: 确保在不同的编辑器环境和运行时环境中行为一致
3. **调试**: 提供了足够的调试信息以帮助排查缩进相关问题

## 8. 扩展性

该设计允许轻松扩展其他类型的上下文组件，例如颜色主题、字体大小等，为框架提供了良好的扩展性。