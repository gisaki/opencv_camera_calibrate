using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows;
using System.Windows.Media.Imaging;
using OpenCvSharp;

namespace opencv_camera_calibrate.Models
{
    // 実行パラメータ
    public class ExecParams
    {
        public String Filepath;
        private List<double> Params = new List<double>() { 0.0, 0.0, 0.0, 0.0, 0.0 };
        public double k1 { get { return this.Params[0]; } private set { } }
        public double k2 { get { return this.Params[1]; } private set { } }
        public double p1 { get { return this.Params[2]; } private set { } }
        public double p2 { get { return this.Params[3]; } private set { } }
        public double k3 { get { return this.Params[4]; } private set { } }

        // コンストラクタ
        private ExecParams() { }
        public ExecParams(String filepath, double k1, double k2, double p1, double p2, double k3)
        {
            this.Filepath = filepath;
            this.Params[0] = k1;
            this.Params[1] = k2;
            this.Params[2] = p1;
            this.Params[3] = p2;
            this.Params[4] = k3;
        }
        public ExecParams(ExecParams other)
        {
            this.Filepath = other.Filepath;
            this.Params = new List<double>(other.Params);
        }

        // 比較
        public bool IsSame(ExecParams other)
        {
            if (other == null) { return false; }
            if (this.Filepath != other.Filepath) { return false; }
            return Enumerable.SequenceEqual(this.Params, other.Params);
        }
    }

    // 画像加工
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
            ExecParams param = new ExecParams(filepath, k1, k2, p1, p2, k3);
            this.CameraCalibrateWrapper(param);
        }
        public void Update(double k1, double k2, double p1, double p2, double k3)
        {
            ExecParams param = new ExecParams(this.Filepath, k1, k2, p1, p2, k3);
            this.CameraCalibrateWrapper(param);
        }
        private Object lockobj_ = new Object();
        private bool ongping_ = false;
        private ExecParams last_ = null;
        private ExecParams done_ = null;
        private void CameraCalibrateWrapper(ExecParams param)
        {
            // 複数回呼び出された場合に、途中を間引く
            lock (this.lockobj_)
            {
                // 最新を控えておく
                this.last_ = new ExecParams(param);
                // 実施中なら何もしない
                if (this.ongping_)
                {
                    return;
                }
            }

            // 完了時、より新しいパラメータが届いていなければ終了
            while (true)
            {
                ExecParams param_exec = null;
                lock (this.lockobj_)
                {
                    if (this.last_.IsSame(this.done_)) {
                        this.ongping_ = false; // 実施完了
                        return;
                    }
                    param_exec = new ExecParams(this.last_);
                    this.done_ = new ExecParams(param_exec);
                    this.ongping_ = true; // 実施中
                }
                // 最新で実施
                this.CameraCalibrate(param_exec);
            }　// while
        }
        [System.Runtime.InteropServices.DllImport("gdi32.dll")]
        public static extern bool DeleteObject(IntPtr hObject);
        private void CameraCalibrate(ExecParams param)
        {
            this.ErrorMessage = String.Empty;
            try
            {
                Mat src = Cv2.ImRead(param.Filepath, ImreadModes.Color);

                {
                    int w = src.Size().Width;
                    int h = src.Size().Height;

                    int fx = w;
                    int fy = w; //  hかな？
                    double Cx = w / 2.0;
                    double Cy = h / 2.0;

                    double[] array_mtx = { fx, 0, Cx, 0, fy, Cy, 0, 0, 1 };
                    Mat mtx = new Mat<double>(3, 3, array_mtx);

                    double[] array_dist = { param.k1, param.k2, param.p1, param.p2, param.k3 };
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
