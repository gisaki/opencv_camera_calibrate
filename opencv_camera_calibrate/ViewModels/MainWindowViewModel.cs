using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        }

        // 操作

    }
}
