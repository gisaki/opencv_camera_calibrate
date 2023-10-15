using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using GongSolutions.Wpf.DragDrop;

namespace opencv_camera_calibrate.ViewModels
{
    // 何らかのデータ
    public class SliderValue : Helpers.Observable
    {
        public ViewModels.MainWindowViewModel ViewModel { get; private set; }

        // コンストラクタ
        public SliderValue(MainWindowViewModel vm) { this.ViewModel = vm; }

        // プロパティ
        private double _v;
        public double v
        {
            get { return this._v; }
            set
            {
                // Setter で表示更新と連動させる仕組み
                this.Set(ref this._v, value);
                // 他に表示更新が連動しているプロパティがあれば下記に記述
                OnPropertyChanged("str");
                // 再描画のトリガ
                this.ViewModel.Slider_ValueChanged();
            }
        }
        public String str
        {
            get { return String.Format("{0:0.00}", this.v); }
            private set { } // dummy
        }
    }

    public class MainWindowViewModel : Helpers.Observable, GongSolutions.Wpf.DragDrop.IDropTarget
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
            this.param_k1_ = new SliderValue(this);
            this.param_k2_ = new SliderValue(this);
            this.param_p1_ = new SliderValue(this);
            this.param_p2_ = new SliderValue(this);
            this.param_k3_ = new SliderValue(this);
        }

        // プロパティ（画面にbinding）
        public Models.ImageCalibration image_ { get; set; } = new Models.ImageCalibration() { };
        public SliderValue param_k1_ { get; set; }
        public SliderValue param_k2_ { get; set; }
        public SliderValue param_p1_ { get; set; }
        public SliderValue param_p2_ { get; set; }
        public SliderValue param_k3_ { get; set; }

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
            }));
        }

        // 操作
        public void DragOver(IDropInfo dropInfo)
        {
            var files = ((DataObject)dropInfo.Data).GetFileDropList().Cast<string>();
            dropInfo.Effects = files.Any(fname => 
                (
                    fname.EndsWith(".jpeg", StringComparison.OrdinalIgnoreCase) ||
                    fname.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase) ||
                    fname.EndsWith(".png", StringComparison.OrdinalIgnoreCase)
                )
            ) ? DragDropEffects.Copy : DragDropEffects.None;
        }

        public void Drop(IDropInfo dropInfo)
        {
            var files = ((DataObject)dropInfo.Data).GetFileDropList().Cast<string>()
                .Where(fname => 
                    (
                    fname.EndsWith(".jpeg", StringComparison.OrdinalIgnoreCase) ||
                    fname.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase) ||
                    fname.EndsWith(".png", StringComparison.OrdinalIgnoreCase)
                    )
                ).ToList();

            if (files.Count == 0) return;

            foreach (var file in files)
            {
                // 先頭だけ
                this.image_.LoadNewFile(
                    file,
                    this.param_k1_.v,
                    this.param_k2_.v,
                    this.param_p1_.v,
                    this.param_p2_.v,
                    this.param_k3_.v
                    );
                break;
            }
        }

        public void Slider_ValueChanged()
        {
            Thread thread = new Thread(new ThreadStart(() =>
            {
                this.View.Dispatcher.Invoke((Action)(() =>
                {
                    this.image_.Update(
                        this.param_k1_.v,
                        this.param_k2_.v,
                        this.param_p1_.v,
                        this.param_p2_.v,
                        this.param_k3_.v
                    );
                }));
            }));
            thread.Start();
        }

    }
}
