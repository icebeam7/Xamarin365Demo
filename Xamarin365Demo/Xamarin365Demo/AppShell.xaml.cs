﻿using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Xamarin365Demo
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
        }
    }
}