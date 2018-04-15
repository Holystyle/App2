using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.Core;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x804 上介绍了“空白页”项模板

namespace App2
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        private async void button_Click(object sender, RoutedEventArgs e)
        {
            FileOpenPicker openPicker = new FileOpenPicker();
            openPicker.ViewMode = PickerViewMode.Thumbnail;
            openPicker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
            openPicker.FileTypeFilter.Add(".mp3");
            openPicker.FileTypeFilter.Add(".mp4");

            StorageFile file = await openPicker.PickSingleFileAsync();
            if (file != null)
            {
                mediaPlayer.Source = MediaSource.CreateFromStorageFile(file);
                Tips.Text = file.DisplayName; //显示文件名字
            }
            else
            {
                Tips.Text = "Invalid File!";
            }
        }

        private void playOnline_Click(object sender, RoutedEventArgs e)       
        {  
            if (Uri.IsWellFormedUriString(source.Text,UriKind.Absolute))           
            {
                mediaPlayer.Source = MediaSource.CreateFromUri(new Uri(source.Text));
                Tips.Text = "正在播放：";
            }
            else
            {
                Tips.Text = "Invalid url!";
            }
        }

        private async void downLoad_Click(object sender, RoutedEventArgs e)
        {
            if (Uri.IsWellFormedUriString(source.Text, UriKind.Absolute))
            {

                Uri uri = new Uri(source.Text);
                var myMusics = await StorageLibrary.GetLibraryAsync(KnownLibraryId.Music);
                var myMusic = myMusics.SaveFolder;
                var fileName = Path.GetFileName(uri.LocalPath);
                try
                {
                    StorageFile musicFile = await myMusic.CreateFileAsync(fileName, Windows.Storage.CreationCollisionOption.FailIfExists);
                    if (musicFile != null)
                    {
                        Windows.Web.Http.HttpClient httpClient = new Windows.Web.Http.HttpClient();
                        IBuffer buffer;
                        try
                        {
                            buffer = await httpClient.GetBufferAsync(uri);
                        }
                        catch (Exception ex)
                        {
                            Tips.Text = "download fail!";
                            return;
                        }
                        await FileIO.WriteBufferAsync(musicFile, buffer);
                        Tips.Text = "download complete!";
                        mediaPlayer.Source = MediaSource.CreateFromUri(new Uri(source.Text));
                    }
                }
                catch (Exception ex)
                {
                    Tips.Text = "file exist";
                    mediaPlayer.Source = MediaSource.CreateFromUri(new Uri(source.Text));
                }
            }
            else
            {
                Tips.Text = "Invalid url!";
            }
        }
    }
 }

