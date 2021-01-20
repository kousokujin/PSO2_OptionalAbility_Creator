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
    /// OP_MaterialBox.xaml の相互作用ロジック
    /// </summary>
    public partial class OP_MaterialBox : UserControl
    {
        public ObservableCollection<OP_recipe_Data> recipe;
        private OP_RecipiBox_Data opr_boxdata;
        
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

        public OP_MaterialBox(List<OP_Recipe2> recipes,int count)
        {
            this.recipe = new ObservableCollection<OP_recipe_Data>();
            recipes.ForEach(x => this.recipe.Add(new OP_recipe_Data(x)));

            InitializeComponent();
            opr_boxdata = new OP_RecipiBox_Data(recipes,count);
            DataContext = opr_boxdata;
            OP_ListBox.ItemsSource = opr_boxdata.recipe;

            int add_height = 20 * (opr_boxdata.recipe.Count - 1);
            Height += add_height;

            //childrenbox = new List<IMaterialBox>();
        }

        //子要素に応じてちょうどいい位置に動く
        /*
        public void MoveCenter()
        {
            //子要素の一番左のx座標
            double x_left = childrenbox.Select(x => x.GetBoxPosition().Left).ToList().Min();

            //子要素の一番右のx座標
            double x_right = childrenbox.Select(x => x.GetBoxPosition().Right).ToList().Min();

            //親コントロールの幅
            double maxWidth = Margin.Left + Margin.Right + Width;

            double PosWidth = (maxWidth - x_left - x_right - Width);
            PosWidth /= 2.0;
            double LeftMargin = x_left + PosWidth;
            double RightMargin = x_right + PosWidth;

            double top = this.Margin.Top;
            double bottom = this.Margin.Bottom;

            this.Margin = new Thickness(LeftMargin, top, RightMargin, bottom);

            //線を引き直す
            double thisCenterX = Margin.Left + Width / 2;
            double thisCenterY = Margin.Top + Height;
            
            foreach(IMaterialBox m in childrenbox)
            {
                (double childW, double childH) = m.GetWidthHeight();
                double childcenterX = m.GetBoxPosition().Left + childW/2;
                double childcenterY = m.GetBoxPosition().Top;

                string geoStr = string.Format("M{0},{1} L{2},{3}", thisCenterX, thisCenterY,childcenterX, childcenterY);
                m.path.Data = Geometry.Parse(geoStr);
            }

            moveEvent?.Invoke(this, new EventArgs());

        }

        public Path getPath()
        {
            return path;
        }

        public Thickness GetBoxPosition()
        {
            return this.Margin;
        }

        //子要素が動いたとき
        public void ChildrenMove(object sender,EventArgs e)
        {
            if(sender is IMaterialBox)
            {
                var box = (IMaterialBox)sender;
                box.flag_memo = true;

                bool all_flag = childrenbox.All(x => x.flag_memo == true);

                if(all_flag == true)
                {
                    MoveCenter();
                    childrenbox.ForEach(x => x.flag_memo = false);
                }

                MoveCenter();
            }
        }

        public (double,double) GetWidthHeight()
        {
            return (this.Width, this.Height);
        }
        //強制的にイベント発火
        public void forceEvent()
        {
            moveEvent?.Invoke(this, new EventArgs());
        }

        */
    }

    public class OP_RecipiBox_Data
    {
        public ObservableCollection<OP_recipe_Data> recipe;
        private int needcount;

        public string needcount_str
        {
            get
            {
                return string.Format("{0}個", needcount);
            }
        }

        //全体の成功率
        public float getRaito
        {
            get
            {
                float x = 1.0f;
                foreach(OP_recipe_Data r in recipe)
                {
                    x *= ((float)r.origin_recipe.percent/100.0f);
                }
                return x;
            }
        }

        public string getRaito_str
        {
            get
            {
                return string.Format("{0:P}",getRaito);
            }
        }

        public string boarder_color
        {
            get
            {
                float percent = getRaito;

                if (percent >= 1.0)
                {
                       return "#FF08d41d";
                }

                if (percent < 1.0 && percent >= 0.8)
                {
                    return "#FF082ad4";
                }

                if (percent < 0.8 && percent >= 0.5)
                {
                    return "#FFe7eb1a";
                }

                if(percent < 0.5 && percent >= 0.25)
                {
                    return "#ffff1c1c";
                }

                if(percent < 0.25 && percent >= 0)
                {
                    return "#FF7300ff";
                }

                return "#FFFFFFFF";
            }
        }

        public OP_RecipiBox_Data(List<OP_Recipe2> recipe,int needcount = 1)
        {
            this.recipe =new ObservableCollection<OP_recipe_Data>();
            recipe.ForEach(x => this.recipe.Add(new OP_recipe_Data(x)));
            this.needcount = needcount;
        }
    }

    public class OP_recipe_Data
    {
        public OP_Recipe2 origin_recipe;

        public string name
        {
            get
            {
               return origin_recipe.name.jp_name;
            }
        }

        public string percent
        {
            get
            {
                return string.Format("{0}%",origin_recipe.percent);
            }
        }

        public OP_recipe_Data(OP_Recipe2 recipe)
        {
            this.origin_recipe = recipe;
        }
    }

    /*
    public interface IMaterialBox
    {
        Thickness GetBoxPosition();
        (double, double) GetWidthHeight();
        bool flag_memo { get; set; }

        //親につながる線
        Path path { get; set; }

        //自分が移動したとき
        event EventHandler moveEvent;
        List<IMaterialBox> childrenbox { get; set; }

        //void MoveCenter();

        void ChildrenMove(object sender, EventArgs e);

        void forceEvent();
    }
    */
}
