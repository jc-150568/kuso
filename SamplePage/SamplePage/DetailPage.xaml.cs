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
    public partial class DetailPage : ContentPage
    {
        string Isbn;
        string title;
        string Date;
        string type;
        string itemcaption;
        string category;
        string author;
        string publisher;

        int read;
        int redstar;
        int bluebook;

        public DetailPage(string x)
        {
            InitializeComponent();

            Isbn = x;

            if (UserModel.isbnSelect(x) != null)
            {
                var query = UserModel.isbnSelect(x);

                foreach (var book in query)
                {
                    title = book.Title;
                    Date = book.SalesDate;
                    type = book.Type;
                    publisher = book.Publisher;
                    category = book.booksGenreId;
                    bluebook = book.BlueBook;
                    redstar = book.RedStar;
                    read = book.Read;
                    if (book.ItemCaption != null)
                    {
                        itemcaption = book.ItemCaption;
                    }
                    /*bluebook = book.BlueBook;
                    redstar = book.RedStar;*/
                }


            }
            else
            {
                DisplayAlert("表なし", "表なし", "OK");
            }

            title2.Text = title;
            Type2.Text = "タイプ:" + type;
            SalesDate2.Text = "発売日:" + Date;
            Publisher2.Text = "出版社:" + publisher;
            ItemCaption2.Text = "説明:" + itemcaption;
            if (bluebook == 1)
            {
                this.image1.Image = "blue_book_72.png";
            }

            else
            {
                this.image1.Image = "gray_book_72.png";
            }

            if (redstar == 1)
            {
                this.image2.Image = "red_star_72.png";
            }

            else
            {
                this.image2.Image = "gray_star_72.png";
            }

            if (read == 0)
            {
                this.unread2.Text = "未読";
                unread1.IsToggled = false;
            }

            if (read == 1)
            {
                this.unread2.Text = "既読";
                unread1.IsToggled = true;
            }
        }

        // 読みたいボタンを点滅させる
        private void OnImageClicked1(object sender, EventArgs e)
        {
            if (bluebook == 1)
            {
                UserModel.Gray_Book(Isbn);
                this.image1.Image = "gray_book_72.png";
                bluebook = 0;
            }

            else
            {
                UserModel.Blue_Book(Isbn);
                this.image1.Image = "blue_book_72.png";
                bluebook = 1;
            }
        }

        // お気にいりボタンを点滅させる
        private void OnImageClicked2(object sender, EventArgs e)
        {
            if (redstar == 1)
            {
                UserModel.Gray_Star(Isbn);
                this.image2.Image = "gray_star_72.png";
                redstar = 0;
            }

            else
            {
                UserModel.Red_Star(Isbn);
                this.image2.Image = "red_star_72.png";
                redstar = 1;
            }
        }

        // ラベルを未読⇔既読にする
        private void OnToggled(object sender, ToggledEventArgs e)
        {
            if (unread1.IsToggled == true)
            {
                UserModel.ReadBook(Isbn);
                this.unread2.Text = "既読";
            }
            if (unread1.IsToggled == false)
            {
                UserModel.UnreadBook(Isbn);
                this.unread2.Text = "未読";
            }
        }

        private async void OnDeleteButtonClicked(object sender, EventArgs e)
        {
            bool x = await DisplayAlert("警告", "削除してもよろしいですか？", "はい", "いいえ");
            if (x == true)
            {
                UserModel.DeleteBook(Isbn);
                Navigation.PushAsync(new BookPage());
            }
        }
    }
}