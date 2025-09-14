# DeclGUI 编辑器组件

本文档详细介绍了 DeclGUI 框架中可用的各种 UI 组件及其使用示例。

## 基础组件

### Label（标签）
用于显示文本内容的组件。

**参数：**
- `text` (string): 显示的文本内容
- `style` (DeclStyle?): 样式设置

**示例：**
```csharp
new Label("Hello World")
new Label("彩色文本", DeclStyle.WithColor(Color.red))
```

### Button（按钮）
可点击的按钮组件，支持点击事件处理。

**参数：**
- `text` (string): 按钮显示文本
- `onClick` (Action): 点击事件回调
- `style` (DeclStyle?): 样式设置

**示例：**
```csharp
new Button("Click Me", OnClick)
new Button("大按钮", OnClick, DeclStyle.WithSize(200, 40))
```

### TextField（文本输入框）
允许用户输入和编辑文本的组件。

**参数：**
- `value` (string): 当前文本值
- `onValueChanged` (Action<string>): 文本变化回调
- `style` (DeclStyle?): 样式设置

**示例：**
```csharp
new TextField("Initial Text")
new TextField(currentText, OnTextChanged, DeclStyle.WithWidth(200))
```

### Slider（滑动条）
允许用户通过拖动滑块来选择数值的组件。

**参数：**
- `value` (float): 当前值
- `minValue` (float): 最小值
- `maxValue` (float): 最大值
- `onValueChanged` (Action<float>): 值变化回调
- `style` (DeclStyle): 样式设置

**示例：**
```csharp
new Slider(0.5f, 0f, 1f)
new Slider(currentValue, 0f, 1f, OnSliderChanged, DeclStyle.WithWidth(200))
```

### Toggle（切换开关）
用于在两种状态之间切换的组件。

**参数：**
- `value` (bool): 当前状态
- `onValueChanged` (Action<bool>): 状态变化回调
- `style` (DeclStyle): 样式设置

**示例：**
```csharp
new Toggle(true)
new Toggle(isEnabled, OnToggleChanged)
```

### ObjectField（对象选择字段）
允许用户选择 Unity 对象的组件。

**参数：**
- `value` (T): 当前对象引用
- `onValueChanged` (Action<T>): 对象变化回调
- `allowSceneObjects` (bool): 是否允许场景对象
- `style` (DeclStyle): 样式设置

**示例：**
```csharp
new ObjectField<UnityEngine.Object>(null)
new ObjectField<Texture2D>(selectedTexture, OnTextureChanged, true)
```

## 布局组件

### Hor（水平布局容器）
水平排列子元素的容器组件。

**参数：**
- `children` (params IElement[]): 子元素
- `style` (DeclStyle?): 样式设置

**示例：**
```csharp
new Hor {
    new Label("Left"),
    new Label("Right")
}
new Hor(DeclStyle.WithColor(Color.gray), child1, child2)
```

### Ver（垂直布局容器）
垂直排列子元素的容器组件。

**参数：**
- `children` (params IElement[]): 子元素
- `style` (DeclStyle?): 样式设置

**示例：**
```csharp
new Ver {
    new Label("Top"),
    new Label("Bottom")
}
new Ver(DeclStyle.WithColor(Color.gray), child1, child2)
```

### Spc（间距组件）
用于在布局中添加空白间距的组件。

**参数：**
- `size` (float): 间距大小
- `style` (DeclStyle?): 样式设置

**示例：**
```csharp
new Spc(10) // 添加10像素的间距
```

## 状态管理组件

### StatefulButton（有状态按钮）
具有状态管理功能的按钮组件。

**参数：**
- `text` (string): 按钮文本
- `onClick` (Action<ButtonState>): 点击事件回调
- `key` (string): 元素唯一键

**示例：**
```csharp
new StatefulButton("Stateful", OnStatefulClick)
```

### LongPressButton（长按按钮）
支持长按事件的按钮组件。

**参数：**
- `text` (string): 按钮文本
- `onLongPress` (Action): 长按事件回调
- `longPressDuration` (float): 长按持续时间（秒）

**示例：**
```csharp
new LongPressButton("Long Press", OnLongPress)
new LongPressButton("长按2秒", OnLongPress, 2.0f)
```

## 上下文组件

### ContextBatch（上下文批处理容器）
用于批量提供多个上下文信息的容器组件。

**参数：**
- `contexts` (params IContextProvider[]): 上下文提供者数组
- `child` (IElement): 子元素

**示例：**
```csharp
new ContextBatch(
    new ReadOnly(true, childElement),
    new UserName("John", childElement)
)
```

### ContextConsumer（上下文消费者）
用于消费上下文信息的组件。

**参数：**
- `render` (Func<IContextReader, IElement>): 渲染函数

**示例：**
```csharp
new ContextConsumer(context => {
    bool isReadOnly = context.Get<ReadOnly>().Value;
    string userName = context.Get<UserName>().Value;
    
    return new Label($"Hello {userName}, ReadOnly: {isReadOnly}");
})
```

## 高级组件

### ECanvas（编辑器画布）
用于在自动布局系统中创建固定位置的子元素。

**参数：**
- `children` (params IElement[]): 子元素
- `style` (DeclStyle?): 样式设置

**示例：**
```csharp
new ECanvas(
    new AbsolutePanel(new Vector2(100, 50), 
        new Button("固定位置按钮", OnButtonClick)
    )
)
```

### AbsolutePanel（绝对定位面板）
不参与自动布局，但可以使用自动布局系统渲染内容物。

**参数：**
- `position` (Vector2): 面板位置
- `child` (IElement): 子元素
- `style` (DeclStyle?): 样式设置
- `minWidth` (float?): 最小宽度
- `minHeight` (float?): 最小高度
- `maxWidth` (float?): 最大宽度
- `maxHeight` (float?): 最大高度

**示例：**
```csharp
new AbsolutePanel(new Vector2(100, 50), 
    new Button("固定位置按钮", OnButtonClick)
)
```

### FixableSpace（可伸缩空白空间）
用于在布局中创建可伸缩的空白空间。

**参数：**
- `style` (DeclStyle?): 样式设置

**示例：**
```csharp
new FixableSpace()
```

### ScrollRect（滚动视图容器）
用于创建可滚动的UI区域。

**参数：**
- `scrollPosition` (Vector2): 滚动位置
- `content` (IElement): 内容元素
- `onScroll` (Action<Vector2>): 滚动位置变化回调
- `alwaysShowVertical` (bool): 是否总是显示垂直滚动条
- `alwaysShowHorizontal` (bool): 是否总是显示水平滚动条
- `style` (DeclStyle): 样式

**示例：**
```csharp
new ScrollRect(scrollPosition, content, OnScrollChanged)
```

## 输入字段组件

### IntField（整数字段）
用于输入整数值的组件。

**参数：**
- `value` (int): 当前值
- `onValueChanged` (Action<int>): 值变化回调
- `style` (DeclStyle): 样式设置

**示例：**
```csharp
new IntField(42, OnIntChanged)
```

### FloatField（浮点数字段）
用于输入浮点数值的组件。

**参数：**
- `value` (float): 当前值
- `onValueChanged` (Action<float>): 值变化回调
- `style` (DeclStyle): 样式设置

**示例：**
```csharp
new FloatField(3.14f, OnFloatChanged)
```

### Popup（下拉选择框）
用于从选项列表中选择值的组件。

**参数：**
- `selectedIndex` (int): 当前选中的索引
- `options` (string[]): 选项列表
- `onSelectionChanged` (Action<int>): 选择变化回调
- `style` (DeclStyle): 样式设置

**示例：**
```csharp
new Popup(0, new[] { "选项1", "选项2", "选项3" }, OnSelectionChanged)
```

### EnumPopup（枚举下拉选择框）
用于从枚举类型中选择值的组件。

**参数：**
- `value` (Enum): 当前选中的枚举值
- `onValueChanged` (Action<Enum>): 值变化回调
- `style` (DeclStyle): 样式设置

**示例：**
```csharp
new EnumPopup(KeyCode.A, OnEnumChanged)
```

### ColorField（颜色选择器）
用于选择颜色值的组件。

**参数：**
- `value` (Color): 当前颜色值
- `showAlpha` (bool): 是否显示Alpha通道
- `onValueChanged` (Action<Color>): 值变化回调
- `style` (DeclStyle): 样式设置

**示例：**
```csharp
new ColorField(Color.red, true, OnColorChanged)
```

### Vector2Field（Vector2输入字段）
用于输入二维向量值的组件。

**参数：**
- `value` (Vector2): 当前Vector2值
- `onValueChanged` (Action<Vector2>): 值变化回调
- `style` (DeclStyle): 样式设置

**示例：**
```csharp
new Vector2Field(new Vector2(1, 2), OnVector2Changed)
```

### Vector3Field（Vector3输入字段）
用于输入三维向量值的组件。

**参数：**
- `value` (Vector3): 当前Vector3值
- `onValueChanged` (Action<Vector3>): 值变化回调
- `style` (DeclStyle): 样式设置

**示例：**
```csharp
new Vector3Field(new Vector3(1, 2, 3), OnVector3Changed)
```

### Vector4Field（Vector4输入字段）
用于输入四维向量值的组件。

**参数：**
- `value` (Vector4): 当前Vector4值
- `onValueChanged` (Action<Vector4>): 值变化回调
- `style` (DeclStyle): 样式设置

**示例：**
```csharp
new Vector4Field(new Vector4(1, 2, 3, 4), OnVector4Changed)
```

### MinMaxSlider（最小-最大范围滑块）
用于选择数值范围的组件。

**参数：**
- `minValue` (float): 当前最小值
- `maxValue` (float): 当前最大值
- `minLimit` (float): 允许的最小值限制
- `maxLimit` (float): 允许的最大值限制
- `onValueChanged` (Action<float, float>): 值变化回调
- `style` (DeclStyle): 样式设置

**示例：**
```csharp
new MinMaxSlider(0.2f, 0.8f, 0, 1, OnRangeChanged)
```

### LayerField（图层选择器）
用于选择Unity图层的组件。

**参数：**
- `layerIndex` (int): 当前选中的图层索引
- `onValueChanged` (Action<int>): 值变化回调
- `style` (DeclStyle): 样式设置

**示例：**
```csharp
new LayerField(0, OnLayerChanged)
```

### TagField（标签选择器）
用于选择Unity标签的组件。

**参数：**
- `tag` (string): 当前选中的标签
- `onValueChanged` (Action<string>): 值变化回调
- `style` (DeclStyle): 样式设置

**示例：**
```csharp
new TagField("Untagged", OnTagChanged)
```

### CurveField（曲线编辑器）
用于编辑AnimationCurve的组件。

**参数：**
- `value` (AnimationCurve): 当前曲线值
- `onValueChanged` (Action<AnimationCurve>): 值变化回调
- `style` (DeclStyle): 样式设置

**示例：**
```csharp
new CurveField(AnimationCurve.Linear(0, 0, 1, 1), OnCurveChanged)
```

## 其他组件

### ReadOnly（只读上下文）
提供只读状态的上下文组件。

**参数：**
- `value` (bool): 只读状态
- `child` (IElement): 子元素

**示例：**
```csharp
new ReadOnly(true, childElement)
```

### UserName（用户名上下文）
提供用户名信息的上下文组件。

**参数：**
- `value` (string): 用户名
- `child` (IElement): 子元素

**示例：**
```csharp
new UserName("John Doe", childElement)
```