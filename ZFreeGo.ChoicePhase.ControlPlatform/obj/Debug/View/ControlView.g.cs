﻿#pragma checksum "..\..\..\View\ControlView.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "A8AA777278D035BD77CC95782B775046"
//------------------------------------------------------------------------------
// <auto-generated>
//     此代码由工具生成。
//     运行时版本:4.0.30319.42000
//
//     对此文件的更改可能会导致不正确的行为，并且如果
//     重新生成代码，这些更改将会丢失。
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Shell;


namespace ZFreeGo.ChoicePhase.ControlPlatform.View {
    
    
    /// <summary>
    /// ControlView
    /// </summary>
    public partial class ControlView : System.Windows.Controls.Page, System.Windows.Markup.IComponentConnector {
        
        
        #line 63 "..\..\..\View\ControlView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox setHezhaPhase;
        
        #line default
        #line hidden
        
        
        #line 64 "..\..\..\View\ControlView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.RadioButton jiaoDuRadio;
        
        #line default
        #line hidden
        
        
        #line 65 "..\..\..\View\ControlView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.RadioButton huduRadio;
        
        #line default
        #line hidden
        
        
        #line 73 "..\..\..\View\ControlView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Canvas cavas;
        
        #line default
        #line hidden
        
        
        #line 75 "..\..\..\View\ControlView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Shapes.Polyline sinWave;
        
        #line default
        #line hidden
        
        
        #line 84 "..\..\..\View\ControlView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Shapes.Line lineCursor;
        
        #line default
        #line hidden
        
        
        #line 86 "..\..\..\View\ControlView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Canvas cavasRight;
        
        #line default
        #line hidden
        
        
        #line 99 "..\..\..\View\ControlView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Shapes.Line roundCursor;
        
        #line default
        #line hidden
        
        
        #line 104 "..\..\..\View\ControlView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox realHezhaTime;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/ZFreeGo.ChoicePhase.ControlPlatform;component/view/controlview.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\View\ControlView.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            this.setHezhaPhase = ((System.Windows.Controls.TextBox)(target));
            return;
            case 2:
            this.jiaoDuRadio = ((System.Windows.Controls.RadioButton)(target));
            return;
            case 3:
            this.huduRadio = ((System.Windows.Controls.RadioButton)(target));
            return;
            case 4:
            this.cavas = ((System.Windows.Controls.Canvas)(target));
            return;
            case 5:
            this.sinWave = ((System.Windows.Shapes.Polyline)(target));
            return;
            case 6:
            this.lineCursor = ((System.Windows.Shapes.Line)(target));
            return;
            case 7:
            this.cavasRight = ((System.Windows.Controls.Canvas)(target));
            return;
            case 8:
            this.roundCursor = ((System.Windows.Shapes.Line)(target));
            return;
            case 9:
            this.realHezhaTime = ((System.Windows.Controls.TextBox)(target));
            return;
            }
            this._contentLoaded = true;
        }
    }
}

