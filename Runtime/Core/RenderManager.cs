using System;
using System.Collections.Generic;
using UnityEngine;
using DeclGUI.Core;

namespace DeclGUI.Core
{
    /// <summary>
    /// 渲染管理器基类
    /// 负责管理渲染器的注册和元素的渲染
    /// </summary>
    public abstract class RenderManager
    {
        private readonly Dictionary<Type, IElementRenderer> _renderers = new Dictionary<Type, IElementRenderer>();
        private bool _initialized = false;

        /// <summary>
        /// 状态栈管理器
        /// </summary>
        public StateStackManager StateStack { get; } = new StateStackManager();

        /// <summary>
        /// 上下文栈管理器
        /// </summary>
        public ContextStack ContextStack { get; } = new ContextStack();

        /// <summary>
        /// 过渡效果处理器
        /// </summary>
        public TransitionProcessor TransitionProcessor { get; } = new TransitionProcessor();

        /// <summary>
        /// 根状态存储器存储
        /// </summary>
        private IStateManagerStorage _rootStorage;

        /// <summary>
        /// 当前正在处理的事件
        /// </summary>
        private Event _currentEvent;

        /// <summary>
        /// 当前正在处理的事件
        /// </summary>
        public Event CurrentEvent => _currentEvent;

        /// <summary>
        /// 确保管理器已初始化
        /// </summary>
        protected void EnsureInitialized()
        {
            if (!_initialized)
            {
                DiscoverRenderers();
                _initialized = true;
            }
        }

        /// <summary>
        /// 发现和注册渲染器
        /// 子类需要重写此方法来注册特定的渲染器
        /// </summary>
        protected abstract void DiscoverRenderers();

        /// <summary>
        /// 注册渲染器
        /// </summary>
        /// <typeparam name="T">元素类型</typeparam>
        /// <param name="renderer">渲染器实例</param>
        protected void RegisterRenderer<T>(IElementRenderer<T> renderer) where T : IElement
        {
            _renderers[typeof(T)] = renderer;
        }

        /// <summary>
        /// 注册渲染器（基于类型）
        /// </summary>
        /// <param name="elementType">元素类型</param>
        /// <param name="renderer">渲染器实例</param>
        protected void RegisterRenderer(Type elementType, IElementRenderer renderer)
        {
            _renderers[elementType] = renderer;
        }

        /// <summary>
        /// 渲染DOM树
        /// </summary>
        /// <param name="element">根元素</param>
        /// <param name="currentEvent">当前事件（可选）</param>
        public void RenderDOM(in IElement element, Event currentEvent = null)
        {
            EnsureInitialized();
            _currentEvent = currentEvent ?? Event.current;

            if (element == null)
                return;

            try
            {
                // 递归渲染元素（事件处理在渲染过程中进行）
                RenderElement(element);
            }
            finally
            {
                ResetAndCleanupUnuseState();
                _currentEvent = null;
            }
        }

        /// <summary>
        /// 设置当前事件
        /// </summary>
        /// <param name="currentEvent">当前事件</param>
        public void SetCurrentEvent(Event currentEvent)
        {
            _currentEvent = currentEvent;
        }

        /// <summary>
        /// 清除当前事件
        /// </summary>
        public void ClearCurrentEvent()
        {
            _currentEvent = null;
        }

        /// <summary>
        /// 渲染单个元素（基于状态栈的新架构）
        /// </summary>
        /// <param name="element">要渲染的元素</param>
        public void RenderElement(in IElement element)
        {
            if (element == null)
                return;

            // 根据元素类型处理不同的渲染逻辑
            if (element is IContextProvider contextProvider)
            {
                // 上下文提供者：压入上下文栈并渲染子元素
                RenderContextElement(contextProvider);
            }
            else if (element is ContextConsumer contextConsumer)
            {
                // 上下文消费者：使用当前上下文栈渲染
                RenderContextConsumer(contextConsumer);
            }
            else
            {
                // 统一处理容器、状态和普通元素
                RenderElementInternal(element);
            }

            ProcessDisableState(element);

            // 处理事件（如果有当前事件）- 在渲染之后进行
            // 在Editor环境下，即使事件类型是Repaint也需要处理Hover事件
            if (_currentEvent != null && IsNeedProcessEevent(element))
            {
                ProcessElementEvents(element);
            }
        }

        private bool IsNeedProcessEevent(IElement element)
        {
            if (element is IEventfulElement eventfulElement)
            {
                return eventfulElement.Events.OnClick != null ||
                        eventfulElement.Events.OnPressDown != null ||
                        eventfulElement.Events.OnPressUp != null ||
                        eventfulElement.Events.OnScroll != null ||
                        eventfulElement.Events.OnHoverEnter != null ||
                        eventfulElement.Events.OnHoverExit != null ||
                        eventfulElement.Events.OnFocus != null ||
                        eventfulElement.Events.OnBlur != null;
            }
            return false;
        }

        private void ProcessDisableState(IElement element)
        {
            if (element is IElementWithKey elementWithKey)
            {
                IElementState elementState = GetElementState(elementWithKey);
                if (elementState == null)
                    return;

                if (ContextStack.Has<DisableContext>())
                {
                    var disabledContext = ContextStack.Get<DisableContext>();
                    elementState.SetDisabled(disabledContext.Value);
                }
                else
                {
                    elementState.SetDisabled(false);
                }
            }
        }

        public IElementState GetElementState(IElementWithKey element)
        {
            if (element == null || StateStack.IsEmpty())
                return null;

            var stateManager = StateStack.CurrentStateManager;
            return stateManager?.GetOrCreateState(element);
        }

        /// <summary>
        /// 处理元素的事件
        /// 只对实现了IEventfulElement接口的元素处理事件
        /// </summary>
        private void ProcessElementEvents(IElement element)
        {
            IElementState elementState = null;
            if (element is IElementWithKey elementWithKey)
            {
                elementState = GetElementState(elementWithKey);
            }

            // 只处理可处理事件的元素
            if (element is IEventfulElement eventfulElement)
            {
                // 获取元素的屏幕区域（需要渲染器提供）
                Rect elementRect = GetElementScreenRect(element);


                // 检查事件是否发生在元素区域内
                if (elementRect.Contains(_currentEvent.mousePosition))
                {
                    switch (_currentEvent.type)
                    {
                        case EventType.MouseDown:
                            // 处理焦点获取：当鼠标点击在元素上时，设置该元素为聚焦状态
                            if (elementState != null)
                            {
                                if (!elementState.HasState(ElementStateFlags.Disabled))
                                {
                                    // 检查是否当前元素没有焦点，如果是则触发Focus事件
                                    bool wasFocused = elementState.HasState(ElementStateFlags.Focus);

                                    elementState.SetFocused(true);

                                    // 仅在元素之前没有焦点时触发Focus事件
                                    if (!wasFocused)
                                    {
                                        eventfulElement.Events.OnFocus?.Invoke();
                                    }
                                }
                            }
                            eventfulElement.Events.OnPressDown?.Invoke();
                            break;
                        case EventType.MouseUp:
                            eventfulElement.Events.OnPressUp?.Invoke();
                            // 点击事件在释放时触发
                            if (_currentEvent.button == 0) // 左键
                            {
                                eventfulElement.Events.OnClick?.Invoke();
                            }
                            break;
                        case EventType.MouseMove:
                        case EventType.Repaint:
                            // 在Repaint事件中也处理Hover事件，因为Editor中MouseMove很少触发
                            HandleHoverEvents(eventfulElement, true);
                            break;
                        case EventType.ScrollWheel:
                            eventfulElement.Events.OnScroll?.Invoke();
                            break;
                    }
                }
                else
                {
                    switch (_currentEvent.type)
                    {
                        case EventType.MouseDown:
                            {
                                // 检查当前是否有其他元素获得了焦点
                                // 这里简单地认为只要鼠标不在当前元素上，就触发Blur事件
                                bool wasFocused = elementState.HasState(ElementStateFlags.Focus);
                                // 检查失焦事件：如果元素之前有焦点，但现在鼠标不在其上，则触发Blur事件
                                if (elementState != null && wasFocused)
                                {
                                    elementState.SetFocused(false);

                                    // 仅在元素之前有焦点时触发Blur事件
                                    if (wasFocused)
                                    {
                                        eventfulElement.Events.OnBlur?.Invoke();
                                    }
                                }
                                break;
                            }
                        case EventType.Repaint:
                            HandleHoverEvents(eventfulElement, false);
                            break;
                    }


                }
            }
        }

        /// <summary>
        /// 处理悬停事件（基于IElementState状态管理）
        /// </summary>
        private void HandleHoverEvents(IEventfulElement element, bool isMouseOver)
        {

            // 对于有状态元素，使用状态管理系统处理悬停事件
            if (StateStack.IsEmpty() || (element.Events.OnHoverEnter == null && element.Events.OnHoverExit == null))
            {
                return;
            }

            var stateManager = StateStack.CurrentStateManager;
            var elementState = stateManager.GetOrCreateState(element);

            if (elementState != null)
            {
                var hoverState = elementState.HoverState;
                var currentTime = Time.time;

                if (isMouseOver)
                {
                    if (hoverState.ShouldTriggerEnter)
                    {
                        // 触发Enter事件：从非悬停状态进入悬停状态
                        hoverState.IsHovering = true;
                        hoverState.HoverStartTime = currentTime;
                        element.Events.OnHoverEnter?.Invoke();

                        // 设置LastHoverTime为HoverStartTime，确保下一次不会重复触发Enter
                        hoverState.LastHoverTime = hoverState.HoverStartTime;
                    }
                    else
                    {
                        // 鼠标在元素上，更新悬停时间（仅在非Enter触发时）
                        hoverState.LastHoverTime = currentTime;
                    }
                }
                else
                {
                    // 鼠标不在元素上
                    if (hoverState.ShouldTriggerLeave)
                    {
                        // 触发Leave事件：从悬停状态离开
                        hoverState.LastLeaveTime = currentTime;
                        element.Events.OnHoverExit?.Invoke();
                        hoverState.IsHovering = false;

                        // 重置LastHoverTime，确保下一次进入时可以触发Enter
                        hoverState.LastHoverTime = 0f;
                    }
                }

                // 更新元素状态
                elementState.HoverState = hoverState;

                // 更新状态标志（确保状态同步）
                elementState.UpdateStateFlags();
            }
        }

        /// <summary>
        /// 获取元素的屏幕区域
        /// 需要渲染器在渲染时提供元素的位置和大小信息
        /// 子类需要实现此方法来提供元素的位置检测
        /// </summary>
        /// <param name="element">要获取位置的元素</param>
        /// <returns>元素的屏幕矩形</returns>
        protected abstract Rect GetElementScreenRect(IElement element);

        /// <summary>
        /// 内部渲染方法（统一处理容器、状态和普通元素）
        /// </summary>
        private void RenderElementInternal(IElement element)
        {
            IStateManagerStorage containerStateManager = null;
            bool isContainer = element is IContainerElement;

            // 如果是容器元素，压入状态栈
            if (isContainer)
            {
                containerStateManager = GetOrCreateStateManagerStorage((IContainerElement)element);
                StateStack.Push(containerStateManager);
            }

            try
            {
                IElementState state = null;
                // 统一渲染逻辑：为所有IElementWithKey的元素创建状态
                if (element is IElementWithKey elementWithKey)
                {
                    if (!StateStack.IsEmpty())
                    {
                        // 使用状态栈的当前状态管理器
                        state = StateStack.CurrentStateManager.GetOrCreateState(elementWithKey);

                        // 如果元素是IStatefulElement，调用其CreateState方法
                        if (element is IStatefulElement statefulElement)
                        {
                            var customState = statefulElement.CreateState();
                            if (customState != null)
                            {
                                // 将自定义状态设置到ElementState的State属性中
                                if (state is ElementState concreteState)
                                {
                                    concreteState.State = customState;
                                }
                            }
                        }
                    }
                }

                // 尝试使用渲染器渲染
                if (TryRenderWithRenderer(element, state))
                {
                    return;
                }

                try
                {
                    // 如果没有找到渲染器，调用元素的Render方法
                    var renderTarget = element.Render();
                    if (renderTarget != null && renderTarget != element)
                    {
                        RenderElement(renderTarget);
                    }
                }
                catch (Exception ex)
                {
                    // 如果没有渲染器，使用管理器的Fallback方法
                    RenderFallback(ex, element);
                }
            }
            finally
            {
                // 如果是容器元素，弹出状态栈
                if (isContainer)
                {
                    StateStack.Pop();
                }
            }
        }

        /// <summary>
        /// 渲染上下文元素
        /// </summary>
        private void RenderContextElement(IContextProvider context)
        {
            // 使用运行时类型管理上下文
            Type contextType = context.GetType();
            ContextStack.Push(contextType, context);

            try
            {
                RenderElement(context.Child);
            }
            finally
            {
                ContextStack.Pop(contextType);
            }
        }

        /// <summary>
        /// 渲染上下文消费者
        /// </summary>
        private void RenderContextConsumer(IContextConsumer contextConsumer)
        {
            RenderElement(contextConsumer.Render(ContextStack));
        }

        /// <summary>
        /// 尝试使用渲染器渲染元素
        /// </summary>
        /// <param name="element">要渲染的元素</param>
        /// <param name="state">状态参数（可选）</param>
        /// <returns>是否成功使用渲染器渲染</returns>
        protected bool TryRenderWithRenderer(IElement element, IElementState elementState = null)
        {
            var elementType = element.GetType();

            IDeclStyle style = null;

            // 处理样式化元素
            if (element is IStylefulElement stylefulElement && elementState != null)
            {
                // 解析样式（处理样式集引用）
                IDeclStyle resolvedStyle = DeclThemeManager.ResolveStyle(stylefulElement.Style, elementState);

                // 获取过渡配置
                var transitionConfig = GetTransitionConfig(resolvedStyle);

                // 获取当前样式（如果有过渡，则使用过渡样式；否则使用解析后的样式）
                IDeclStyle currentStyle = GetCurrentStyle(elementState, resolvedStyle);

                // 应用过渡效果
                IDeclStyle finalStyle = TransitionProcessor.ProcessTransition(
                    currentStyle, resolvedStyle, elementState, transitionConfig);

                style = finalStyle;
            }

            // 首先尝试精确匹配
            if (_renderers.TryGetValue(elementType, out var renderer))
            {
                return TryInvokeRenderer(renderer, element, elementState?.State, style);
            }

            // 如果是泛型类型，尝试匹配泛型定义
            if (elementType.IsGenericType)
            {
                var genericDefinition = elementType.GetGenericTypeDefinition();
                if (_renderers.TryGetValue(genericDefinition, out renderer))
                {
                    return TryInvokeRenderer(renderer, element, elementState?.State, style);
                }
            }

            return false;
        }

        /// <summary>
        /// 尝试调用渲染器
        /// </summary>
        private bool TryInvokeRenderer(IElementRenderer renderer, IElement element, object state, IDeclStyle style)
        {
            try
            {
                if (state != null)
                {
                    // 有状态元素必须使用状态感知渲染器
                    if (renderer is IStatefulElementRenderer statefulRenderer)
                    {
                        statefulRenderer.Render(this, element as IStatefulElement, state, style);
                    }
                    else
                    {
                        throw new InvalidOperationException(
                            $"Stateful element {element.GetType().Name} requires a state-aware renderer. " +
                            $"Implement IStatefulElementRenderer for {element.GetType().Name} or use a compatible renderer.");
                    }
                }
                else
                {
                    renderer.Render(this, element, style);
                }
                return true;
            }
            catch (System.Exception ex)
            {
                renderer.RenderFallback(ex, element);
                Debug.LogError(ex);
                return true; // 仍然算作使用了渲染器（即使失败了）
            }
        }

        /// <summary>
        /// 渲染普通元素
        /// </summary>
        private void RenderRegularElement(IElement element)
        {
            // 尝试使用渲染器渲染
            if (TryRenderWithRenderer(element, null))
            {
                return;
            }

            try
            {
                // 如果没有找到渲染器，调用元素的Render方法
                var renderTarget = element.Render();
                if (renderTarget != null && renderTarget != element)
                {
                    RenderElement(renderTarget);
                }
            }
            catch (Exception ex)
            {
                // 如果没有渲染器，使用管理器的Fallback方法
                RenderFallback(ex, element);
            }
        }

        // /// <summary>
        // /// 渲染有状态元素（带状态参数）
        // /// </summary>
        // /// <param name="element">有状态元素</param>
        // /// <param name="state">元素状态</param>
        // public void RenderElement(in IStatefulElement element, object state)
        // {
        //     if (element == null)
        //         return;

        //     // 尝试使用渲染器渲染（带状态）
        //     if (TryRenderWithRenderer(element, state))
        //     {
        //         return;
        //     }

        //     try
        //     {
        //         // 如果没有找到渲染器，调用元素的Render方法
        //         var renderTarget = element.Render(state);
        //         if (renderTarget != null && renderTarget != element)
        //         {
        //             RenderElement(renderTarget);
        //         }
        //     }
        //     catch (Exception ex)
        //     {
        //         // 如果没有渲染器，使用管理器的Fallback方法
        //         RenderFallback(ex, element);
        //     }
        // }

        /// <summary>
        /// 获取指定类型的渲染器
        /// </summary>
        /// <typeparam name="T">元素类型</typeparam>
        /// <returns>渲染器实例，如果未找到则返回null</returns>
        public IElementRenderer<T> GetRenderer<T>() where T : IElement
        {
            _renderers.TryGetValue(typeof(T), out var renderer);
            return renderer as IElementRenderer<T>;
        }

        /// <summary>
        /// 获取元素的渲染器
        /// </summary>
        /// <param name="element">要获取渲染器的元素</param>
        /// <returns>渲染器实例，如果未找到则返回null</returns>
        public IElementRenderer GetRenderer(IElement element)
        {
            if (element == null)
                return null;

            var elementType = element.GetType();

            // 首先尝试精确匹配
            if (_renderers.TryGetValue(elementType, out var renderer))
            {
                return renderer;
            }

            // 如果是泛型类型，尝试匹配泛型定义
            if (elementType.IsGenericType)
            {
                var genericDefinition = elementType.GetGenericTypeDefinition();
                if (_renderers.TryGetValue(genericDefinition, out renderer))
                {
                    return renderer;
                }
            }

            return null;
        }

        /// <summary>
        /// 尝试获取元素的渲染器
        /// </summary>
        /// <param name="element">要获取渲染器的元素</param>
        /// <param name="renderer">返回的渲染器</param>
        /// <returns>是否成功获取到渲染器</returns>
        private bool TryGetRenderer(IElement element, out IElementRenderer renderer)
        {
            renderer = null;

            // 查找对应的渲染器
            var elementType = element.GetType();

            // 首先尝试精确匹配
            if (_renderers.TryGetValue(elementType, out renderer))
            {
                return true;
            }

            // 如果是泛型类型，尝试匹配泛型定义
            if (elementType.IsGenericType)
            {
                var genericDefinition = elementType.GetGenericTypeDefinition();
                if (_renderers.TryGetValue(genericDefinition, out renderer))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 计算元素的期望大小
        /// </summary>
        /// <param name="element">要计算大小的元素</param>
        /// <param name="parentStyle">父元素的样式（可选）</param>
        /// <returns>元素的期望大小</returns>
        public Vector2 CalculateElementSize(IElement element, IDeclStyle parentStyle = null)
        {
            EnsureInitialized();

            if (element == null)
                return Vector2.zero;

            // 特殊处理IContextProvider类型
            if (element is IContextProvider contextProvider)
            {
                // 对于上下文提供者，计算其子元素的大小
                if (contextProvider.Child != null)
                {
                    return CalculateElementSize(contextProvider.Child, parentStyle);
                }
                return Vector2.zero;
            }

            // 特殊处理IContextConsumer类型
            if (element is IContextConsumer contextConsumer)
            {
                // 对于上下文消费者，尝试获取其渲染结果并计算大小
                try
                {
                    var renderedElement = contextConsumer.Render(ContextStack);
                    if (renderedElement != null)
                    {
                        return CalculateElementSize(renderedElement, parentStyle);
                    }
                }
                catch (Exception ex)
                {
                    Debug.LogWarning($"Error calculating size for ContextConsumer: {ex.Message}");
                }
                return Vector2.zero;
            }

            // 尝试直接获取元素的渲染器并计算大小
            if (TryGetRenderer(element, out var directRenderer))
            {
                return directRenderer.CalculateSize(this, element, parentStyle);
            }

            // 获取元素的实际渲染目标
            var renderTarget = element.Render();
            if (renderTarget == null || renderTarget == element)
                return Vector2.zero;

            // 尝试获取渲染器并计算大小
            return CalculateElementSize(renderTarget, parentStyle);
        }

        /// <summary>
        /// 获取或创建容器状态管理器（优雅的无键拼接实现）
        /// </summary>
        public IStateManagerStorage GetOrCreateStateManagerStorage(IContainerElement container)
        {
            if (!StateStack.IsEmpty())
            {
                string key = GetContainerStateKey(container);
                container.Key = key; // 保持Key，使其在接下来的渲染状态都不变
                return StateStack.CurrentStorage.GetOrCreateChildStateManagerStorage(key, () =>
                {
                    return new StateManagerStorage(new ContainerState());
                });
            }

            if (_rootStorage == null)
                _rootStorage = new StateManagerStorage(new ContainerState());

            return _rootStorage;
        }

        private string GetContainerStateKey(IContainerElement container)
        {
            return !string.IsNullOrEmpty(container.Key)
                ? container.Key
                : StateStack.CurrentStorage.GenerateContainerStateKey(container.GetType());
        }

        /// <summary>
        /// 清理和重置所有状态管理器（优化版本）
        /// </summary>
        private void ResetAndCleanupUnuseState()
        {
            _rootStorage?.ResetAndCleanupUnuseState(10);
        }

        /// <summary>
        /// 统一的Fallback渲染方法（由继承者实现）
        /// </summary>
        /// <param name="exception">发生的异常</param>
        /// <param name="element">发生异常的元素</param>
        protected abstract void RenderFallback(Exception exception, IElement element);

        /// <summary>
        /// 获取过渡配置
        /// </summary>
        private TransitionConfig? GetTransitionConfig(IDeclStyle style)
        {
            // 如果样式是DeclStyleSet类型，尝试获取其过渡配置
            if (style is DeclStyleSet styleSet)
            {
                return styleSet.Transition;
            }

            return null;
        }

        /// <summary>
        /// 获取当前样式
        /// </summary>
        private IDeclStyle GetCurrentStyle(IElementState elementState, IDeclStyle targetStyle)
        {
            // 如果有活跃的过渡，返回过渡样式
            if (elementState.TransitionState != null &&
                elementState.TransitionState.FromStyle != null &&
                !elementState.TransitionState.IsCompleted)
            {
                return elementState.TransitionState.ToStyle == targetStyle
                    ? elementState.TransitionState.FromStyle
                    : targetStyle;
            }

            return targetStyle;
        }

        /// <summary>
        /// 推送上下文到上下文栈
        /// </summary>
        /// <typeparam name="T">上下文类型</typeparam>
        /// <param name="context">上下文实例</param>
        public void PushContext<T>(T context) where T : IContextProvider
        {
            ContextStack.Push(typeof(T), context);
        }

        /// <summary>
        /// 从上下文栈弹出指定类型的上下文
        /// </summary>
        /// <typeparam name="T">上下文类型</typeparam>
        public void PopContext<T>() where T : IContextProvider
        {
            ContextStack.Pop(typeof(T));
        }
    }
}