using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using GongSolutions.Wpf.DragDrop;

namespace opencv_camera_calibrate.ViewModels
{
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
            this.image_ = new Models.ImageCalibration() { };
        }

        // プロパティ（画面にbinding）
        public Models.ImageCalibration image_ { get; set; }

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
                this.image_.LoadNewFile(file);
                break;
            }
        }

    }
}
