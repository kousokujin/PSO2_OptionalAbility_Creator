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
        public List<Path> PathList;

        public OP_MaterialBox(List<OP_Recipe2> recipes)
        {
            this.recipe = new ObservableCollection<OP_recipe_Data>();
            recipes.ForEach(x => this.recipe.Add(new OP_recipe_Data(x)));

            InitializeComponent();
            opr_boxdata = new OP_RecipiBox_Data(recipes);
            PathList = new List<Path>();
            DataContext = opr_boxdata;
            OP_ListBox.ItemsSource = opr_boxdata.recipe;

            int add_height = 20 * (opr_boxdata.recipe.Count - 1);
            Height += add_height;
        }
    }

    public class OP_RecipiBox_Data
    {
        public ObservableCollection<OP_recipe_Data> recipe;

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

        public OP_RecipiBox_Data(List<OP_Recipe2> recipe)
        {
            this.recipe =new ObservableCollection<OP_recipe_Data>();
            recipe.ForEach(x => this.recipe.Add(new OP_recipe_Data(x)));
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
}
