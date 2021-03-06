﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SamplePage
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PropertyPage : ContentPage
    {
        public PropertyPage()
        {
            InitializeComponent();
        }

        async void Common(object sender, EventArgs e)
        {
            bool x = await DisplayAlert("警告", "登録されている書籍を消去してもよろしいですか？", "OK", "CANCEL");
            if (x == true)
            {
                UserModel.dropUser();
            }
        }
        private void Backup(object sender, EventArgs e)
        {
            Navigation.PushAsync(new BackupPage());
        }
        private void Help(object sender, EventArgs e)
        {
            Navigation.PushAsync(new HelpPage());
        }
    }
}