using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using OpenCvSharp;

namespace opencv_camera_calibrate.ViewModels
{
    public class MainWindowViewModel : Helpers.Observable
    {
        public Views.MainWindow View { get; private set; }

        // コンストラクタでViewと関連付ける
        public MainWindowViewModel(Views.MainWindow mainWindow)
            : base()
        {
            this.View = mainWindow;

            // 
            // データやプロパティの初期状態
            // 
            this.list_ = new System.Collections.ObjectModel.ObservableCollection<Models.SomeModel>();
            this.list_.Add(new Models.SomeModel() { TimeStamp = DateTime.Now.AddHours(1), });
            this.list_.Add(new Models.SomeModel() { TimeStamp = DateTime.Now.AddHours(2), });
            this.some_model_ = new Models.SomeModel() { TimeStamp = DateTime.Now, };
        }

        // プロパティ（画面にbinding）
        public Models.SomeModel some_model_ { get; set; }
        public System.Collections.ObjectModel.ObservableCollection<Models.SomeModel> list_ { get; set; }
        private BitmapSource _someImage;
        public BitmapSource SomeImage
        {
            get { return this._someImage; }
            set
            {
                // Setter で表示更新と連動させる仕組み
                this.Set(ref this._someImage, value);
            }
        }

        // コマンド（画面にbinding）
        private Helpers.RelayCommand _doSomethingCommand;
        public Helpers.RelayCommand DoSomethingCommand
        {
            get { return _doSomethingCommand = _doSomethingCommand ?? new Helpers.RelayCommand(DoSomethingExecute, DoSomethingCanExecute); }
        }
        private bool DoSomethingCanExecute()
        {
            return true;
        }
        private void DoSomethingExecute()
        {
            this.View.Dispatcher.Invoke((Action)(() =>
            {
                Models.SomeModel item = new Models.SomeModel() { TimeStamp = DateTime.Now.AddHours(1), };
                this.list_.Add(item);
            }));
            TestOpenCV();
        }

        // 操作
        [System.Runtime.InteropServices.DllImport("gdi32.dll")]
        public static extern bool DeleteObject(IntPtr hObject);
        private void TestOpenCV()
        {
            // 画像を読み込む
            Mat src = new Mat(@"Parrots.jpg", ImreadModes.Grayscale);

            // 二値化後画像
            Mat dst = src.Clone();

            // 二値化
            Cv2.Threshold(src, dst, 0, 255, ThresholdTypes.Otsu);

            // 表示する画像
            Bitmap bitmap = OpenCvSharp.Extensions.BitmapConverter.ToBitmap(dst);

            // 表示
            IntPtr hbitmap = bitmap.GetHbitmap();
            this.SomeImage = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(hbitmap, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
            DeleteObject(hbitmap);

            //メモリクリア
            bitmap.Dispose();
        }

    }
}
