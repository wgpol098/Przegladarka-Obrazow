﻿#pragma checksum "..\..\..\AppWindow\Slideshow.xaml" "{8829d00f-11b8-4213-878b-770e8597ac16}" "D3A6548CAA60FAB1B077B1DAACD7B931F023D746817703B666A643DE97433CB7"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using Przegladarka_obazow.AppWindow;
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


namespace Przegladarka_obazow.AppWindow {
    
    
    /// <summary>
    /// Slideshow
    /// </summary>
    public partial class Slideshow : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 13 "..\..\..\AppWindow\Slideshow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Image ImageSlide;
        
        #line default
        #line hidden
        
        
        #line 17 "..\..\..\AppWindow\Slideshow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Label PopupAlert;
        
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
            System.Uri resourceLocater = new System.Uri("/Przegladarka obazow;component/appwindow/slideshow.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\AppWindow\Slideshow.xaml"
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
            
            #line 9 "..\..\..\AppWindow\Slideshow.xaml"
            ((Przegladarka_obazow.AppWindow.Slideshow)(target)).KeyDown += new System.Windows.Input.KeyEventHandler(this.myControl_KeyDown);
            
            #line default
            #line hidden
            
            #line 11 "..\..\..\AppWindow\Slideshow.xaml"
            ((Przegladarka_obazow.AppWindow.Slideshow)(target)).Closing += new System.ComponentModel.CancelEventHandler(this.SlideshowClosing);
            
            #line default
            #line hidden
            return;
            case 2:
            this.ImageSlide = ((System.Windows.Controls.Image)(target));
            return;
            case 3:
            this.PopupAlert = ((System.Windows.Controls.Label)(target));
            return;
            case 4:
            
            #line 22 "..\..\..\AppWindow\Slideshow.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.ClickNext);
            
            #line default
            #line hidden
            return;
            case 5:
            
            #line 25 "..\..\..\AppWindow\Slideshow.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.ClickBack);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}
