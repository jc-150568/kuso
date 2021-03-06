﻿/*using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

using ZXing.Mobile;
using ZXing.Net.Mobile.Forms;

namespace SamplePage
{

    public partial class CameraPage : ContentPage
    {
        private String sd2;
        private int no=0;
        // ObservableCollection<string> scanedData;
        public CameraPage()
        {
            InitializeComponent();
            //  scanedData = new ObservableCollection<string>();
            // this.BindingContext = scanedData;
        }

        async void ScanButtonClicked(object sender, EventArgs s)
        {
            var scanPage = new ZXingScannerPage()
            {
                DefaultOverlayTopText = "バーコードを読み取ります",
                DefaultOverlayBottomText = "",
            };

            // スキャナページを表示
            await Navigation.PushAsync(scanPage);

            scanPage.OnScanResult += (result) =>
            {
                // スキャン停止
                scanPage.IsScanning = false;

                // PopAsyncで元のページに戻り、結果をダイアログで表示
                Device.BeginInvokeOnMainThread(async () =>
                {
                    await Navigation.PopAsync();
                    // await DisplayAlert("スキャン完了", result.Text, "OK");
                });

                // scanedData.Add(result.Text);
                no += 1;
                sd2 = result.Text;
                // LOL.Text = sd2;

                var InsertName = sd2;
                //Userテーブルに適当なデータを追加する
                UserModel.insertUser(1, InsertName,no);
            };

        }

        void SelectClicked(object sender, EventArgs e)
        {
           
            
            //Userテーブルの行データを取得
            var query = UserModel.selectUser(); //中身はSELECT * FROM [User]
            var layout = new StackLayout { HorizontalOptions = LayoutOptions.Center, Margin = new Thickness { Top = 100 } };
            foreach (var user in query)
            {

                //Userテーブルの名前列をLabelに書き出す
              layout.Children.Add(new Label { Text = user.Id.ToString() });
              layout.Children.Add(new Label { Text = user.Name });
              //layout.Children.Add(new Label { Text = user.No.ToString() });
              // LOL.Text = user.Name;

            }
           Content = layout;
        }
    }
}*/

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using ZXing.Mobile;
using ZXing.Net.Mobile.Forms;
using Newtonsoft.Json;
using System.IO;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using Newtonsoft.Json.Linq;

namespace SamplePage

{
    public partial class CameraPage : ContentPage
    {
        private string url;
        static string requestUrl;
        ObservableCollection<string> scanedData;

        public CameraPage()
        {
            InitializeComponent();
            url = "https://app.rakuten.co.jp/services/api/BooksBook/Search/20170404?format=json&applicationId=1051637750796067320&formatVersion=2"; //formatVersion=2にした
            scanedData = new ObservableCollection<string>();
            this.BindingContext = scanedData;
        }

        async void ScanButtonClicked(object sender, EventArgs s)
        {
            try
            {
                var layout2 = new StackLayout { HorizontalOptions = LayoutOptions.CenterAndExpand, VerticalOptions = LayoutOptions.CenterAndExpand };
                var scroll = new ScrollView { Orientation = ScrollOrientation.Vertical };
                layout2.Children.Add(scroll);
                var layout = new StackLayout { HorizontalOptions = LayoutOptions.CenterAndExpand, VerticalOptions = LayoutOptions.CenterAndExpand };
                scroll.Content = layout;
                var scanPage = new ZXingScannerPage()
                {
                    DefaultOverlayTopText = "バーコードを読み取ります",
                    DefaultOverlayBottomText = "",
                };

                // スキャナページを表示
                await Navigation.PushAsync(scanPage);

                scanPage.OnScanResult += async (result) =>
                {
                    // スキャン停止
                    scanPage.IsScanning = false;

                    // PopAsyncで元のページに戻り、結果をダイアログで表示
                    Device.BeginInvokeOnMainThread(async () =>
                    {
                        await Navigation.PopAsync();
                        // await DisplayAlert("スキャン完了", result.Text, "OK");
                    });

                    string isbncode = result.Text;

                    requestUrl = url + "&isbn=" + isbncode; //URLにISBNコードを挿入



                    //HTTPアクセスメソッドを呼び出す
                    string APIdata = await GetApiAsync(); //jsonをstringで受け取る

                    //HTTPアクセス失敗処理(404エラーとか名前解決失敗とかタイムアウトとか)
                    if (APIdata is null)
                    {
                        await DisplayAlert("接続エラー", "接続に失敗しました", "OK");
                    }



                    /*
                    //レスポンス(JSON)をstringに変換-------------->しなくていい
                    Stream s = GetMemoryStream(APIdata); //GetMemoryStreamメソッド呼び出し
                    StreamReader sr = new StreamReader(s);
                    string json = sr.ReadToEnd();
                    */
                    /*
                    //デシリアライズ------------------>しなくていい
                    var rakutenBooks = JsonConvert.DeserializeObject<RakutenBooks>(json.ToString());
                    */

                    //パースする *重要*   パースとは、文法に従って分析する、品詞を記述する、構文解析する、などの意味を持つ英単語。
                    var json = JObject.Parse(APIdata); //stringのAPIdataをJObjectにパース
                    var Items = JArray.Parse(json["Items"].ToString()); //Itemsは配列なのでJArrayにパース

                    //ここまで来てる---------------------
                    //結果を出力
                    foreach (JObject jobj in Items)
                    {
                        //↓のように取り出す
                        JValue titleValue = (JValue)jobj["title"];
                        string title = (string)titleValue.Value;

                        JValue titleKanaValue = (JValue)jobj["titleKana"];
                        string titleKana = (string)titleKanaValue.Value;

                        JValue subTiv = (JValue)jobj["subTitle"];
                        string subTitle = (string)subTiv.Value;

                        JValue subTiKv = (JValue)jobj["subTitleKana"];
                        string subTitleKana = (string)subTiKv.Value;

                        JValue authorValue = (JValue)jobj["author"];
                        string author = (string)authorValue.Value;

                        JValue authorKanaValue = (JValue)jobj["authorKana"];
                        string authorKana = (string)authorKanaValue.Value;

                        JValue pubV = (JValue)jobj["publisherName"];
                        string publisher = (string)pubV.Value;

                        JValue sizeV = (JValue)jobj["size"];
                        string size = (string)sizeV.Value;

                        JValue isbnValue = (JValue)jobj["isbn"];
                        string isbn = (string)isbnValue.Value;

                        JValue itemCaptionValue = (JValue)jobj["itemCaption"];
                        string itemCaption = (string)itemCaptionValue.Value;

                        JValue salesDateV = (JValue)jobj["salesDate"];
                        string salesDate = (string)salesDateV.Value;

                        JValue priceV = (JValue)jobj["itemPrice"];
                        var priceVV = priceV.ToString();
                        int price = int.Parse(priceVV);

                        JValue gazoValue = (JValue)jobj["largeImageUrl"];
                        string gazo = (string)gazoValue.Value;

                        JValue genreV = (JValue)jobj["booksGenreId"];
                        string genreId = (string)genreV.Value;


                        bool x = await DisplayAlert("この内容で登録してよろしいですか？", "タイトル:" + title + "\r\n著者:" + author, "OK", "CANCEL");
                        if (x == true)
                        {
                            UserModel.insertUser(isbn, title, titleKana, subTitle, subTitleKana, author, authorKana, publisher, size, itemCaption, salesDate, price, gazo, genreId,0,0,0);
                        }
                    };
                };

            }
            catch (Exception e)
            {
                await DisplayAlert("Error", e.ToString(), "ok");
            }


        }

        //HTTPアクセスメソッド
        public static async Task<string> GetApiAsync()
        {
            string APIurl = requestUrl;

            using (HttpClient client = new HttpClient())
                try
                {
                    string urlContents = await client.GetStringAsync(APIurl);
                    await Task.Delay(1000); //1秒待つ(楽天API規約に違反するため)
                    return urlContents;
                }
                catch (Exception e)
                {
                    string a = e.ToString();
                    return null;
                }
        }

        async void SerchClicked(object sender, EventArgs e)
        {
            try
            {
                requestUrl = url + "&isbn=" + LOL.Text; //URLにISBNコードを挿入

                //HTTPアクセスメソッドを呼び出す
                string APIdata = await GetApiAsync(); //jsonをstringで受け取る

                //HTTPアクセス失敗処理(404エラーとか名前解決失敗とかタイムアウトとか)
                if (APIdata is null)
                {
                    await DisplayAlert("接続エラー", "接続に失敗しました", "OK");
                }

                /*
                //レスポンス(JSON)をstringに変換-------------->しなくていい
                Stream s = GetMemoryStream(APIdata); //GetMemoryStreamメソッド呼び出し
                StreamReader sr = new StreamReader(s);
                string json = sr.ReadToEnd();
                */
                /*
                //デシリアライズ------------------>しなくていい
                var rakutenBooks = JsonConvert.DeserializeObject<RakutenBooks>(json.ToString());
                */

                //パースする *重要*   パースとは、文法に従って分析する、品詞を記述する、構文解析する、などの意味を持つ英単語。
                var json = JObject.Parse(APIdata); //stringのAPIdataをJObjectにパース
                var Items = JArray.Parse(json["Items"].ToString()); //Itemsは配列なのでJArrayにパース

                //結果を出力
                foreach (JObject jobj in Items)
                {
                    //↓のように取り出す
                    JValue titleValue = (JValue)jobj["title"];
                    string title = (string)titleValue.Value;

                    JValue titleKanaValue = (JValue)jobj["titleKana"];
                    string titleKana = (string)titleKanaValue.Value;

                    JValue subTiv = (JValue)jobj["subTitle"];
                    string subTitle = (string)subTiv.Value;

                    JValue subTiKv = (JValue)jobj["subTitleKana"];
                    string subTitleKana = (string)subTiKv.Value;

                    JValue authorValue = (JValue)jobj["author"];
                    string author = (string)authorValue.Value;

                    JValue authorKanaValue = (JValue)jobj["authorKana"];
                    string authorKana = (string)authorKanaValue.Value;

                    JValue pubV = (JValue)jobj["publisherName"];
                    string publisher = (string)pubV.Value;

                    JValue sizeV = (JValue)jobj["size"];
                    string size = (string)sizeV.Value;

                    JValue isbnValue = (JValue)jobj["isbn"];
                    string isbn = (string)isbnValue.Value;

                    JValue itemCaptionValue = (JValue)jobj["itemCaption"];
                    string itemCaption = (string)itemCaptionValue.Value;

                    JValue salesDateV = (JValue)jobj["salesDate"];
                    string salesDate = (string)salesDateV.Value;

                    JValue priceV = (JValue)jobj["itemPrice"];
                    var priceVV = priceV.ToString();
                    int price = int.Parse(priceVV);

                    JValue gazoValue = (JValue)jobj["largeImageUrl"];
                    string gazo = (string)gazoValue.Value;

                    JValue genreV = (JValue)jobj["booksGenreId"];
                    string genreId = (string)genreV.Value;


                    bool x = await DisplayAlert("この内容で登録してよろしいですか？", "タイトル:" + title + "\r\n著者:" + author, "OK", "CANCEL");
                    if (x == true)
                    {
                        UserModel.insertUser(isbn, title, titleKana, subTitle, subTitleKana, author, authorKana, publisher, size, itemCaption, salesDate, price, gazo, genreId,0,0,0);
                    }


                };
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", ex.ToString(), "ok");
            }



        }

    }
}