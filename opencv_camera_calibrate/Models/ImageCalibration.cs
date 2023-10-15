using System;
using System.Drawing;
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
            this.LoadNewFile(@"sample_480_x_360_k1[-0.3]_k3[-0.05]_p1[0.03].png", 0, 0, 0, 0, 0);
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
        public void LoadNewFile(String filepath, double k1, double k2, double p1, double p2, double k3)
        {
            this.Filepath = filepath;
            this.OpenCVTest(k1, k2, p1, p2, k3);
        }
        public void Update(double k1, double k2, double p1, double p2, double k3)
        {
            this.OpenCVTest(k1, k2, p1, p2, k3);
        }
        [System.Runtime.InteropServices.DllImport("gdi32.dll")]
        public static extern bool DeleteObject(IntPtr hObject);
        private void OpenCVTest(double k1, double k2, double p1, double p2, double k3)
        {
            this.ErrorMessage = String.Empty;
            try
            {
                Mat src = Cv2.ImRead(this.Filepath, ImreadModes.Color);

                {
                    int w = src.Size().Width;
                    int h = src.Size().Height;

                    int fx = w;
                    int fy = w; //  hかな？
                    double Cx = w / 2.0;
                    double Cy = h / 2.0;

                    double[] array_mtx = { fx, 0, Cx, 0, fy, Cy, 0, 0, 1 };
                    Mat mtx = new Mat<double>(3, 3, array_mtx);

                    double[] array_dist = { k1, k2, p1, p2, k3 };
                    Mat dist = new Mat<double>(5, 1, array_dist);

                    // Refining the camera matrix using parameters obtained by calibration
                    // ROI:Region Of Interest(対象領域)
                    var newcameramtx = Cv2.GetOptimalNewCameraMatrix(mtx, dist, src.Size(), 1, src.Size(), out OpenCvSharp.Rect validPixROI);

                    Mat calib = new Mat();
                    Cv2.Undistort(src, calib, mtx, dist, newcameramtx);

                    // 表示する画像
                    Bitmap bitmap = OpenCvSharp.Extensions.BitmapConverter.ToBitmap(calib);

                    // 表示
                    IntPtr hbitmap = bitmap.GetHbitmap();
                    this.BitmapSource = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(hbitmap, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                    DeleteObject(hbitmap);

                    //メモリクリア
                    bitmap.Dispose();
                }
            }
            catch (Exception e)
            {
                this.ErrorMessage = e.ToString();
            }
        }

    }
}
