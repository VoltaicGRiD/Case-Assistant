﻿#pragma checksum "..\..\CaseStarter.xaml" "{8829d00f-11b8-4213-878b-770e8597ac16}" "779587201E5F523B47721CD1D8BEC52B33D31D51F6694584DEDB0FBAD40C0240"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using Case_Assistant;
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


namespace Case_Assistant {
    
    
    /// <summary>
    /// CaseStarter
    /// </summary>
    public partial class CaseStarter : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 12 "..\..\CaseStarter.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox NewSR;
        
        #line default
        #line hidden
        
        
        #line 14 "..\..\CaseStarter.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox NewName;
        
        #line default
        #line hidden
        
        
        #line 18 "..\..\CaseStarter.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox CMETBox;
        
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
            System.Uri resourceLocater = new System.Uri("/Case Assistant;component/casestarter.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\CaseStarter.xaml"
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
            this.NewSR = ((System.Windows.Controls.TextBox)(target));
            return;
            case 2:
            this.NewName = ((System.Windows.Controls.TextBox)(target));
            
            #line 14 "..\..\CaseStarter.xaml"
            this.NewName.KeyDown += new System.Windows.Input.KeyEventHandler(this.BusinessNameEnter);
            
            #line default
            #line hidden
            return;
            case 3:
            
            #line 15 "..\..\CaseStarter.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.CreateClicked);
            
            #line default
            #line hidden
            return;
            case 4:
            
            #line 16 "..\..\CaseStarter.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.Button_Click);
            
            #line default
            #line hidden
            return;
            case 5:
            this.CMETBox = ((System.Windows.Controls.TextBox)(target));
            
            #line 18 "..\..\CaseStarter.xaml"
            this.CMETBox.KeyDown += new System.Windows.Input.KeyEventHandler(this.BusinessNameEnter);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}

