using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using OpenCvSharp;

namespace opencv_camera_calibrate.Models
{
    // 何らかのデータ
    public class ImageCalibration : Helpers.Observable
    {
        // コンストラクタ
        public ImageCalibration()
        {
            // デフォルト
            this.LoadNewFile(@"sample_480_x_360_k1[-0.3]_k3[-0.05]_p1[0.03].png");
        }

        // プロパティ
        private String _filepath;
        public String Filepath
        {
            get { return this._filepath; }
            set
            {
                // Setter で表示更新と連動させる仕組み
                this.Set(ref this._filepath, value);
                // 他に表示更新が連動しているプロパティがあれば下記に記述
                OnPropertyChanged("Other");
            }
        }
        private String _errorMessage;
        public String ErrorMessage
        {
            get { return this._errorMessage; }
            set
            {
                // Setter で表示更新と連動させる仕組み
                this.Set(ref this._errorMessage, value);
            }
        }
        private BitmapSource _bitmapSource;
        public BitmapSource BitmapSource
        {
            get { return this._bitmapSource; }
            set
            {
                // Setter で表示更新と連動させる仕組み
                this.Set(ref this._bitmapSource, value);
            }
        }

        // 操作
        public void LoadNewFile(String filepath)
        {
            this.Filepath = filepath;
            this.OpenCVTest();
        }
        [System.Runtime.InteropServices.DllImport("gdi32.dll")]
        public static extern bool DeleteObject(IntPtr hObject);
        private void OpenCVTest()
        {
            this.ErrorMessage = String.Empty;
            try
            {
                // 画像を読み込む
                Mat src = new Mat(this.Filepath, ImreadModes.Grayscale);

                // 二値化後画像
                Mat dst = src.Clone();

                // 二値化
                Cv2.Threshold(src, dst, 0, 255, ThresholdTypes.Otsu);

                // 表示する画像
                Bitmap bitmap = OpenCvSharp.Extensions.BitmapConverter.ToBitmap(dst);

                // 表示
                IntPtr hbitmap = bitmap.GetHbitmap();
                this.BitmapSource = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(hbitmap, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                DeleteObject(hbitmap);

                //メモリクリア
                bitmap.Dispose();
            }
            catch (Exception e)
            {
                this.ErrorMessage = e.ToString();
            }
        }

    }
}
