﻿#pragma checksum "..\..\..\View\OptionConfigUI.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "37E69E3BFB13B040BEA9F2238F0B0226"
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
    /// OptionConfigUI
    /// </summary>
    public partial class OptionConfigUI : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 121 "..\..\..\View\OptionConfigUI.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.PasswordBox loginOldPassword;
        
        #line default
        #line hidden
        
        
        #line 123 "..\..\..\View\OptionConfigUI.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.PasswordBox loginNewPassword;
        
        #line default
        #line hidden
        
        
        #line 125 "..\..\..\View\OptionConfigUI.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.PasswordBox loginAckPassword;
        
        #line default
        #line hidden
        
        
        #line 150 "..\..\..\View\OptionConfigUI.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.PasswordBox operatOldPassword;
        
        #line default
        #line hidden
        
        
        #line 152 "..\..\..\View\OptionConfigUI.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.PasswordBox operatNewPassword;
        
        #line default
        #line hidden
        
        
        #line 154 "..\..\..\View\OptionConfigUI.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.PasswordBox operatAckPassword;
        
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
            System.Uri resourceLocater = new System.Uri("/ZFreeGo.ChoicePhase.ControlPlatform;component/view/optionconfigui.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\View\OptionConfigUI.xaml"
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
            this.loginOldPassword = ((System.Windows.Controls.PasswordBox)(target));
            
            #line 121 "..\..\..\View\OptionConfigUI.xaml"
            this.loginOldPassword.PasswordChanged += new System.Windows.RoutedEventHandler(this.passWordBox_PasswordChanged);
            
            #line default
            #line hidden
            return;
            case 2:
            this.loginNewPassword = ((System.Windows.Controls.PasswordBox)(target));
            
            #line 123 "..\..\..\View\OptionConfigUI.xaml"
            this.loginNewPassword.PasswordChanged += new System.Windows.RoutedEventHandler(this.passWordBox_PasswordChanged);
            
            #line default
            #line hidden
            return;
            case 3:
            this.loginAckPassword = ((System.Windows.Controls.PasswordBox)(target));
            
            #line 125 "..\..\..\View\OptionConfigUI.xaml"
            this.loginAckPassword.PasswordChanged += new System.Windows.RoutedEventHandler(this.passWordBox_PasswordChanged);
            
            #line default
            #line hidden
            return;
            case 4:
            this.operatOldPassword = ((System.Windows.Controls.PasswordBox)(target));
            
            #line 150 "..\..\..\View\OptionConfigUI.xaml"
            this.operatOldPassword.PasswordChanged += new System.Windows.RoutedEventHandler(this.passWordBox_PasswordChanged);
            
            #line default
            #line hidden
            return;
            case 5:
            this.operatNewPassword = ((System.Windows.Controls.PasswordBox)(target));
            
            #line 152 "..\..\..\View\OptionConfigUI.xaml"
            this.operatNewPassword.PasswordChanged += new System.Windows.RoutedEventHandler(this.passWordBox_PasswordChanged);
            
            #line default
            #line hidden
            return;
            case 6:
            this.operatAckPassword = ((System.Windows.Controls.PasswordBox)(target));
            
            #line 154 "..\..\..\View\OptionConfigUI.xaml"
            this.operatAckPassword.PasswordChanged += new System.Windows.RoutedEventHandler(this.passWordBox_PasswordChanged);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}

