using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace BitmapCropper
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        #region Declarations
        StorageFile imageFile; //Содержит выбранную фотографию 
        #endregion

        #region Constructors
        public MainPage()
        {
            this.InitializeComponent();
        }
        #endregion

        #region Events handlers
        private async void BtnBrowse_Click(object sender, RoutedEventArgs e)
        {
            // получаем файл фотографии
            if ((imageFile = await PickImage()) == null) return;

            await ImageHolderEx.LoadImageFromFile(imageFile);
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            ImageHolderEx.Reset();
        }

        private async void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            await WriteImageToFileAsync(imageFile);
        }

        private async void BtnSaveAs_Click(object sender, RoutedEventArgs e)
        {
            //Файл в который я хочу записать фотографию
            StorageFile imageFile = await GetSaveFilePicker();

            await WriteImageToFileAsync(imageFile);
        }
        #endregion

        #region private functions
        private async Task<StorageFile> PickImage()
        {
            FileOpenPicker picker = new FileOpenPicker();

            picker.FileTypeFilter.Add(".jpg");
            picker.FileTypeFilter.Add(".jpeg");
            picker.FileTypeFilter.Add(".png");

            return await picker.PickSingleFileAsync();
        }

        /// <summary>
        /// Записывает фотографию в файл
        /// </summary>
        /// <param name="imageFile">
        /// Файл в который нужно записать фотографию
        /// </param>
        /// <param name="bitmapForWrite">
        /// Фотографию которую нужно записать в файл
        /// </param>
        private async Task WriteImageToFileAsync(StorageFile imageFile)
        {
            //Открываем поток файла в который мы хотим записать изменённую фотографию
            //https://docs.microsoft.com/en-gb/windows/uwp/audio-video-camera/imaging#save-a-softwarebitmap-to-a-file-with-bitmapencoder
            using (IRandomAccessStream stream = await imageFile.OpenAsync(FileAccessMode.ReadWrite))
            {
                await ImageHolderEx.SaveAsync(stream, Microsoft.Toolkit.Uwp.UI.Controls.BitmapFileFormat.Jpeg);

                BitmapEncoder encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.JpegEncoderId, stream);
            }
        }

        /// <summary>
        /// Сохраняет файл через FileSavePicker
        /// </summary>
        /// <param name="fileForSave">
        /// Файл который нужно сохранить
        /// </param>
        private async Task<StorageFile> GetSaveFilePicker()
        {
            FileSavePicker fileSavePicker = new FileSavePicker();
            fileSavePicker.FileTypeChoices.Add("", new List<string>() { ".jpg", ".jpeg", ".png" });
            fileSavePicker.SuggestedStartLocation = PickerLocationId.Desktop;

            return await fileSavePicker.PickSaveFileAsync();
        }
        #endregion

    }
}
