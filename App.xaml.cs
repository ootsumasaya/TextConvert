﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using MaterialDesignColors;
using MaterialDesignThemes.Wpf;
using System.Windows.Media;

namespace TextConvert
{
    /// <summary>
    /// App.xaml の相互作用ロジック
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            Color primaryColor = SwatchHelper.Lookup[MaterialDesignColor.Pink];
            Color accentColor = SwatchHelper.Lookup[MaterialDesignColor.Pink];
            ITheme theme = Theme.Create(new MaterialDesignLightTheme(), primaryColor, accentColor);
            Resources.SetTheme(theme);
            base.OnStartup(e);
        }
    }
}
