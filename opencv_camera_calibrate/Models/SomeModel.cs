using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace opencv_camera_calibrate.Models
{
    // 何らかのデータ
    public class SomeModel : Helpers.Observable
    {
        // コンストラクタ
        public SomeModel()
        {
        }

        // プロパティ
        private DateTime _TimeStamp;
        public DateTime TimeStamp
        {
            get { return this._TimeStamp; }
            set
            {
                // Setter で表示更新と連動させる仕組み
                this.Set(ref this._TimeStamp, value);
                // 他に表示更新が連動しているプロパティがあれば下記に記述
                OnPropertyChanged("Other");
            }
        }
    }
}
