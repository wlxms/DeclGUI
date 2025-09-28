using DeclGUI.Components.Advanced;
using DeclGUI.Core;
using UnityEditor;
using UnityEngine;

namespace DeclGUI.Components
{
    /// <summary>
    /// Inspector专用的标签字段控件
    /// 封装Label，使用EditorGUIUtility.labelWidth作为宽度
    /// </summary>
    public struct LabelField : IElement
    {
        public string Text { get; }

        public LabelField(string text)
        {
            Text = text;
        }

        public IElement Render() => new Label(Text, new DeclStyle(width: UnityEditor.EditorGUIUtility.labelWidth));
    }

    /// <summary>
    /// Inspector专用的标题控件
    /// 使用H1-H6样式
    /// </summary>
    public struct Header : IElement
    {
        public string Text { get; }
        public int Level { get; }

        public Header(string text, int level = 1)
        {
            Text = text;
            Level = Mathf.Clamp(level, 1, 6);
        }

        public IElement Render() => new Label(Text, new DeclStyle(styleSetId: $"H{Level}"));
    }

   /// <summary>
   /// Inspector专用的帮助框控件
   /// </summary>
   public struct HelpBox : IElement
   {
       public string Text { get; }
       public MessageType MessageType { get; }

       public HelpBox(string text, MessageType messageType = MessageType.Info)
       {
           Text = text;
           MessageType = messageType;
       }

       public IElement Render()
       {
           // 根据消息类型设置不同的样式集
           string styleSetId = MessageType switch
           {
               MessageType.Warning => "WarningButton",
               MessageType.Error => "DangerButton",
               MessageType.Info => "InfoButton",
               _ => "HelpBoxText"
           };
           
           // 获取Unity内置图标纹理
           Texture iconTexture = null;
           switch (MessageType)
           {
               case MessageType.Info:
                   iconTexture = EditorGUIUtility.IconContent("d_console.infoicon").image;
                   break;
               case MessageType.Warning:
                   iconTexture = EditorGUIUtility.IconContent("d_console.warnicon").image;
                   break;
               case MessageType.Error:
                   iconTexture = EditorGUIUtility.IconContent("d_console.erroricon").image;
                   break;
           }
           
           if (iconTexture != null)
           {
               // 如果有图标纹理，则使用水平布局显示图标和文本
               return new Ver(BoxSkin.HelpBox)
               {
                   new Hor(
                       new Image(iconTexture, ScaleMode.ScaleToFit).WithStyle(DeclStyle.WithSize(20, 20)),
                       new Label(Text, new DeclStyle(styleSetId: styleSetId))
                   ).WithStyle(new DeclStyle(padding: new RectOffset(5, 5, 5, 5)))
               };
           }
           else
           {
               // 没有图标时，保持原有布局
               return new Ver(BoxSkin.HelpBox)
               {
                   new Label(Text, new DeclStyle(styleSetId: styleSetId))
               };
           }
       }
   }

    /// <summary>
    /// Inspector专用的折叠标题控件
    /// </summary>
    public struct FoldoutHeader : IElement
    {
        public string Text { get; }
        public bool IsExpanded { get; }

        public FoldoutHeader(string text, bool isExpanded = true)
        {
            Text = text;
            IsExpanded = isExpanded;
        }

        public IElement Render() => new Label(Text, new DeclStyle(styleSetId: "FoldoutHeader"));
    }

    /// <summary>
    /// Inspector专用的属性字段容器
    /// 水平布局，左侧标签，右侧控件
    /// </summary>
    public struct PropertyField : IElement
    {
        public string Label { get; }
        public IElement Control { get; }

        public PropertyField(string label, IElement control)
        {
            Label = label;
            Control = control;
        }

        public IElement Render() => new Hor(
            new LabelField(Label),
            Control
        );
    }

    /// <summary>
    /// Inspector专用的分隔线控件
    /// 使用Panel实现分隔线效果
    /// </summary>
    public struct Separator : IElement
    {
        public float Height { get; }

        public Separator(float height = 1f)
        {
            Height = height;
        }

        public IElement Render() => new Panel(new DeclStyle(
            height: Height,
            backgroundColor: new Color(0.3f, 0.3f, 0.3f, 0.3f)
        ));
    }

    /// <summary>
    /// Inspector专用的间距控件
    /// 封装Spc，提供更符合Inspector标准的间距
    /// </summary>
    public struct EditorSpc : IElement
    {
        public float Size { get; }

        public EditorSpc(float size = 2f)
        {
            Size = size;
        }

        public IElement Render() => new Spc(Size);
    }
}