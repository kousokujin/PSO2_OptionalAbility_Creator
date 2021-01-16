using System;
using System.Collections.Generic;
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
    /// OP_TreePage.xaml の相互作用ロジック
    /// </summary>
    public partial class OP_TreePage : Page
    {
        //material material = null;
        int material_slot = 0;

        List<OP_MaterialBox> material_boxs;
        //ツリーの深さと幅
        List<int> material_levels;
        List<int> showLevel_Temp;

        int BoxHeight = 120;
        int BoxHeight_OP = 20;
        int BoxWidth = 200;

        int Width_margin = 20;
        int Height_margin = 100;

        int Height_top = 20;
        public OP_TreePage()
        {
            InitializeComponent();

            material_levels = new List<int>();
            showLevel_Temp = new List<int>();
            material_boxs = new List<OP_MaterialBox>();
            //ShowMaterial(material.Recipes, 0, 0);
        }

        //ツリーを表示（外部からアクセス用)
        public void ShowMaterialTree(material m)
        {
            ClearDisplay();

            material_slot = m.Recipes.Count;
            Material_Level(m, 0);
            (this.Width, this.Height) = PageWidthHeight(m.Recipes.Count);
            ShowMaterialTree(m, 0);
        }
        private void ClearDisplay()
        {
            material_levels.Clear();
            showLevel_Temp.Clear();
            material_slot = 0;
            
            foreach(OP_MaterialBox m in material_boxs)
            {
                m.Visibility = Visibility.Hidden;
                main_grid.Children.Remove(m);
            }
            material_boxs.Clear();
        }

        private void ShowMaterialTree(material material, int ylevel)
        {
            if (showLevel_Temp.Count - 1 < ylevel)
            {
                showLevel_Temp.Add(0);
            }

            ShowMaterial(material.Recipes, showLevel_Temp[ylevel], ylevel);
            showLevel_Temp[ylevel]++;

            Console.WriteLine("=================");
            foreach(OP_Recipe2 o in material.Recipes)
            {
                Console.WriteLine(o.name);
            }

            foreach(material m in material.material_childs)
            {
                ShowMaterialTree(m, ylevel + 1);
            }
        }

        private void ShowMaterial(List<OP_Recipe2> m, int x, int y)
        {
            int y_count = 1;
            if(y != 0)
            {
                y_count = material_levels[y - 1];
            }

            OP_MaterialBox box = new OP_MaterialBox(tools.add_NULL_Recipe(material_slot, m));
            main_grid.Children.Add(box);
            material_boxs.Add(box);

            double width_x = this.Width - Width_margin;
            double height_y = this.Height - Height_top;

            double dx_point = (width_x - ((box.Width+Width_margin) * y_count)) / (y_count + 1);
            double dy_point = (height_y - ((box.Height + Height_margin) * (material_levels.Count + 1)))/(material_levels.Count + 2);

            double pointX = dx_point + (dx_point + (box.Width+Width_margin)) * x;
            double pointY = dy_point + (dy_point + (box.Height + Height_margin)) * y;

            box.Margin = ConvertXY(pointX + Width_margin, pointY + Height_top,box.Width,BoxHeight);
            //box.Margin = new Thickness(pointX, pointY, this.Width - box.Width, this.Height - box.Height);

        }

        //右上原点からThicknessを作る
        private Thickness ConvertXY(double x, double y, double wx, double wy)
        {

            return new Thickness(x, y, this.Width - x - wx, this.Height - y - wy);
        }

        //ツリーの深さと幅の探索
        private void Material_Level(material material,int level)
        {
            if(material_levels.Count - 1 < level)
            {
                material_levels.Add(0);
            }

            material_levels[level] +=  material.material_op.Count;

            foreach(material m in material.material_childs)
            {
                Material_Level(m, level + 1);
            }
        }

        //表示領域の縦横の計算
        private (int,int) PageWidthHeight(int slot)
        {

            BoxHeight += BoxHeight_OP * (slot - 1);

            int width = (BoxWidth + Width_margin) * material_levels.Max();
            width += Width_margin;

            int height = (BoxHeight + Height_margin) * (material_levels.Count + 1);
            height += Height_top;

            return (width, height);
        }
    }
}
