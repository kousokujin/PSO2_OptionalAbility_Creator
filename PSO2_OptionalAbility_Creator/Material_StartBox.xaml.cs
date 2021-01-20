using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PSO2_OptionalAbility_Creator
{
    /// <summary>
    /// Material_StartBox.xaml の相互作用ロジック
    /// </summary>
    public partial class Material_StartBox : UserControl
    {
        private ObservableCollection<op_stct2> material;
        private int needcount;

        //親につながる線
        public Path path { get; set; }

        //public List<IMaterialBox> childrenbox { get; set; }
        //public event EventHandler moveEvent;

        /*
        public bool flag_memo
        {
            get; set;
        }
        */
        public Material_StartBox(op_stct_count material)
        {
            InitializeComponent();

            this.material = new ObservableCollection<op_stct2>();
            material.name.ForEach(x => this.material.Add(x));
            //childrenbox = new List<IMaterialBox>();

            OP_ListBox.ItemsSource = this.material;

            int add_height = 22 * (material.name.Count - 1);
            Height += add_height;
            
            needcount = material.count;
            NeedCountLabel.Content = string.Format("{0}個", needcount);

            //moveEvent?.Invoke(this, new EventArgs());
        }

        /*
        public Thickness GetBoxPosition()
        {
            return this.Margin;
        }

        public void MoveCenter()
        {
            //なにもしない
        }

        public void ChildrenMove(object sender,EventArgs e)
        {
            //なにもしない
        }

        //強制的にイベント発火
        public void forceEvent()
        {
            moveEvent?.Invoke(this, new EventArgs());
        }

        //縦横の大きさを返す
        public (double, double) GetWidthHeight()
        {
            return (this.Width, this.Height);
        }

        public Path getPath()
        {
            return path;
        }
        */
    }
}
