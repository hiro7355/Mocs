﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Mocs.Utils;

namespace Mocs.Controls
{
    /// <summary>
    /// LedControl.xaml の相互作用ロジック
    /// </summary>
    public partial class LedControl : UserControl
    {
        public string LastMessage1 { get; set; }     //  LEDの最後のメッセージ(運転状況のCELLメッセージで使用）
        public string LastMessage2 { get; set; }     //  LEDの最後のメッセージ（運転状況のMUメッセージで使用）

        public LedControl()
        {
            InitializeComponent();

        }

        public void SetColorFromName(String name)
        {

            this.Foreground = ColorUtil.brushFromColorName(name);

        }


    }
}
