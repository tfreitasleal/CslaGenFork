﻿using WeifenLuo.WinFormsUI.Docking;

namespace CslaGenerator.Controls
{
    public partial class StartPage : DockContent
    {
        public StartPage()
        {
            InitializeComponent();
        }

        internal void GetState()
        {
            GeneratorController.Current.CurrentUnitLayout.StartPageMainTabHidden = false;
        }
    }
}
