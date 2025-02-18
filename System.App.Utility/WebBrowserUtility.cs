﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace System.App.Utility
{
    /// <summary>
    /// WebBrowser.Source is not a DependencyProperty. One workaround would be to use some AttachedProperty magic to enable this ability.
    /// </summary>
    /// <remarks>
    /// http://stackoverflow.com/questions/263551/databind-the-source-property-of-the-webbrowser-in-wpf
    /// </remarks>
    public static class WebBrowserUtility
    {
        public static readonly DependencyProperty BindableSourceProperty =
            DependencyProperty.RegisterAttached("BindableSource", typeof(string), typeof(WebBrowserUtility), new UIPropertyMetadata(null, BindableSourcePropertyChanged));

        public static string GetBindableSource(DependencyObject obj)
        {
            return (string)obj.GetValue(BindableSourceProperty);
        }

        public static void SetBindableSource(DependencyObject obj, string value)
        {
            obj.SetValue(BindableSourceProperty, value);
        }

        public static void BindableSourcePropertyChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            WebBrowser browser = o as WebBrowser;
            if (browser != null)
            {
                string uri = e.NewValue as string;
                browser.Source = uri != null ? new Uri(uri) : null;
            }
        }
    }
}
