# DeclGUI 缩进功能测试和验证计划

## 1. 概述

本文档详细描述了 DeclGUI 框架中缩进功能的测试和验证计划，确保新实现的缩进功能在各种使用场景下都能正常工作。

## 2. 测试范围

### 2.1 功能测试
- Indent 组件的基本功能
- DeIndent 组件的基本功能
- 嵌套缩进功能
- 动态缩进变化
- 容器 ContextParam 支持
- 与现有布局组件的兼容性

### 2.2 性能测试
- 大量嵌套缩进的性能
- 频繁缩进变化的性能
- 内存使用情况

### 2.3 兼容性测试
- 不同 Unity 版本的兼容性
- 编辑器与运行时环境的兼容性
- 不同操作系统下的表现

## 3. 测试用例

### 3.1 基础功能测试

#### 测试用例 1: 基本缩进功能
- **目的**: 验证 Indent 组件能够正确增加缩进
- **步骤**:
  1. 创建一个 Indent 组件
  2. 添加一个 Label 子元素
  3. 渲染并验证缩进效果
- **预期结果**: Label 元素应显示缩进效果

#### 测试用例 2: 基本反缩进功能
- **目的**: 验证 DeIndent 组件能够正确减少缩进
- **步骤**:
  1. 在缩进环境中创建一个 DeIndent(1) 组件
  2. 添加一个 Label 子元素
  3. 渲染并验证缩进减少效果
- **预期结果**: Label 元素的缩进应比父环境减少1级

#### 测试用例 3: 嵌套缩进
- **目的**: 验证多层嵌套缩进的正确性
- **步骤**:
  1. 创建多层 Indent 嵌套结构
  2. 在不同层级添加 Label 元素
  3. 渲染并验证各层级的缩进
- **预期结果**: 每层缩进应正确叠加

#### 测试用例 4: 自定义缩进大小
- **目的**: 验证自定义缩进大小功能
- **步骤**:
  1. 使用 Indent(30f) 创建缩进组件
 2. 添加子元素
  3. 渲染并测量缩进大小
- **预期结果**: 缩进大小应为30像素

### 3.2 容器 ContextParam 测试

#### 测试用例 5: 容器上下文参数
- **目的**: 验证容器的 WithContext 方法
- **步骤**:
  1. 创建 Ver 容器并使用 WithContext 添加 IndentContext
 2. 向容器添加多个子元素
  3. 渲染并验证所有子元素都有缩进
- **预期结果**: 容器内的所有子元素都应有相应的缩进

#### 测试用例 6: 多个 ContextParam
- **目的**: 验证容器支持多个上下文参数
- **步骤**:
  1. 创建容器并添加多个不同的 ContextParam
  2. 验证所有上下文参数都正确应用
- **预期结果**: 所有上下文参数都应正确生效

### 3.3 边界条件测试

#### 测试用例 7: 负缩进测试
- **目的**: 验证 DeIndent 在没有足够缩进时的行为
- **步骤**:
  1. 在无缩进环境中使用 DeIndent
  2. 渲染并观察行为
- **预期结果**: 不应出现负缩进，应保持最小缩进级别0

#### 测试用例 8: 大量嵌套测试
- **目的**: 验证大量嵌套缩进的性能和正确性
- **步骤**:
  1. 创建50层嵌套缩进
 2. 测量渲染性能和内存使用
- **预期结果**: 应能正常渲染，性能在可接受范围内

### 3.4 响应式测试

#### 测试用例 9: 动态缩进变化
- **目的**: 验证缩进级别能够根据数据状态动态变化
- **步骤**:
 1. 创建基于数据模型的缩进组件
  2. 修改数据模型的缩进级别
  3. 验证 UI 缩进是否相应变化
- **预期结果**: UI 缩进应随数据变化而更新

#### 测试用例 10: 事件处理
- **目的**: 验证缩进组件不影响事件处理
- **步骤**:
  1. 在缩进组件中添加有事件处理的元素
  2. 触发事件并验证处理
- **预期结果**: 事件应能正常处理

## 4. 性能基准测试

### 4.1 渲染性能测试
- **测试场景**: 渲染1000个嵌套缩进元素
- **基准指标**: 渲染时间应小于50ms
- **内存指标**: 内存增长应在合理范围内

### 4.2 上下文栈性能测试
- **测试场景**: 频繁创建和销毁缩进上下文
- **基准指标**: 上下文栈操作应高效

## 5. 兼容性测试

### 5.1 Unity 版本兼容性
- Unity 2021.3 LTS
- Unity 2022.3 LTS
- Unity 2023.2+

### 5.2 平台兼容性
- Windows Editor
- macOS Editor
- Linux Editor

## 6. 验证方法

### 6.1 自动化测试
- 单元测试验证核心逻辑
- 集成测试验证组件交互
- 性能测试脚本验证性能指标

### 6.2 手动测试
- UI 可视化验证
- 交互功能验证
- 边界条件验证

## 7. 测试代码示例

```csharp
// 示例测试代码
[Test]
public void TestBasicIndent()
{
    // 创建缩进组件
    var indent = new Indent();
    var label = new Label("Indented Text");
    indent.Child = label;

    // 渲染并验证
    var renderer = new EditorRenderManager();
    renderer.RenderElement(indent);

    // 验证缩进级别已正确设置
    Assert.IsTrue(renderer.ContextStack.Has<IndentContext>());
    var indentContext = renderer.ContextStack.Get<IndentContext>();
    Assert.AreEqual(1, indentContext.Level);
}

[Test]
public void TestNestedIndent()
{
    var nestedElement = new Indent().WithChild(
        new Ver(
            new Label("Level 1"),
            new Indent().WithChild(
                new Label("Level 2"),
                new Indent().WithChild(
                    new Label("Level 3")
                )
            )
        )
    );

    // 渲染并验证嵌套缩进
    var renderer = new EditorRenderManager();
    renderer.RenderElement(nestedElement);

    // 验证多层缩进正确
    // ...
}
```

## 8. 验收标准

- 所有功能测试用例通过率: 10%
- 性能指标满足基准要求
- 兼容性测试在所有目标平台上通过
- 无内存泄漏
- API 使用符合设计预期