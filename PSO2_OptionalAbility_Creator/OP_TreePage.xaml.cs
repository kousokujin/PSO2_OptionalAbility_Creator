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
        List<Material_StartBox> material_start;
        //ツリーの深さと幅
        List<int> material_levels;
        List<int> showLevel_Temp;

        //イベント発火処理用（かえたい）
        //List<IMaterialBox> EventBox = new List<IMaterialBox>();

        int BoxHeight = 140;
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
            material_start = new List<Material_StartBox>();
            //EventBox = new List<IMaterialBox>();
            //ShowMaterial(material.Recipes, 0, 0);
        }

        //ツリーを表示（外部からアクセス用)
        public void ShowMaterialTree(material_count m)
        {
            ClearDisplay();

            material_slot = m.Recipes.Count;
            Material_Level(m, 0);

            //var box = ShowMaterialTree(m, 0);

            int ChildCout = tools.CountMaterial(m);
            (this.Width, this.Height) = PageWidthHeight(m.Recipes.Count,ChildCout);
            ShowMaterialTree2(m, ChildCout);

            //EventBox.ForEach(x => x.forceEvent());
            //EventBox.Clear();

            Console.WriteLine("Material:{0}", tools.CountMaterial(m));
        }
        private void ClearDisplay()
        {
            material_levels.Clear();
            showLevel_Temp.Clear();
            material_slot = 0;

            foreach(OP_MaterialBox m in material_boxs)
            {
                //Path matPath = m.getPath();
                //matPath.Visibility = Visibility.Hidden;

                m.Visibility = Visibility.Hidden;

                //m.path.Visibility = Visibility.Hidden;
                main_grid.Children.Remove(m.path);
                //線を消す
                main_grid.Children.Remove(m);
            }

            foreach(Material_StartBox m in material_start)
            {
                m.Visibility = Visibility.Hidden;

                m.path.Visibility = Visibility.Hidden;
                main_grid.Children.Remove(m.path);

                main_grid.Children.Remove(m);
            }
            material_boxs.Clear();
            material_start.Clear();
        }

        //--------------------------------------
        //新マテリアルツリーコード
        //--------------------------------------

        //boxの上真ん中のxy座標を返す
        private (OP_MaterialBox,double,double) ShowMaterialTree2(material_count m, int all_material, int shift = 0, int level = 0)
        {
            double width_fix = Width - (Width_margin*2);
            int nodecount = tools.CountMaterial(m);
            double wn = ((double)nodecount / (double)(2 * all_material)) * width_fix;
            wn -= BoxWidth / 2;

            double x_shift = ((double)shift / (double)all_material) * width_fix;
            double windowX = wn + x_shift + Width_margin;
            double windowY = CalcShowPointY(level);

            //OP_MaterialBox matBox = ShowMaterial(m.Recipes, windowX, windowY);
            OP_MaterialBox matBox = ShowMaterial(m.Recipes,m.count,windowX, windowY);

            int shift2 = shift;
            foreach (material_count mx in m.materials_childs_count)
            {
                int nodecount_c = tools.CountMaterial(mx);
                (OP_MaterialBox childBox, double childX,double childY) = ShowMaterialTree2(mx, all_material, shift2, level+1);
                shift2 += nodecount_c;

                //線を引く
                Path p = new Path();
                p.Stroke = new SolidColorBrush(Color.FromArgb(255, 255, 255, 255));
                p.StrokeThickness = 2;
                string geoStr = string.Format("M{0},{1} L{2},{3}", windowX + BoxWidth / 2, windowY+BoxHeight,childX,childY);
                p.Data = Geometry.Parse(geoStr);
                childBox.path = p;
                main_grid.Children.Add(p);
            }

            if(m.material_end_count != null && m.material_end_count.Count != 0)
            {
                MaterialBoxPut(m.material_end_count, Width, all_material, shift2, level, windowX, windowY);
            }


            //末端ノードのみだと思う
            if (m.materials_childs_count.Count == 0)
            {
                MaterialBoxPut(m.material_op_count, Width, all_material, shift, level, windowX, windowY);
                /*
                double dw = width_fix / all_material;
                double x = Width_margin+(dw * shift);

                double y = CalcShowPointY(level+1, BoxHeight);

                foreach(List<op_stct2> ops in m.material_op)
                {
                    Material_StartBox opBox =　ShowMaterial(ops, x, y);

                    //線を引く
                    Path p = new Path();
                    p.Stroke = new SolidColorBrush(Color.FromArgb(255, 255, 255, 255));
                    p.StrokeThickness = 2;
                    string geoStr = string.Format("M{0},{1} L{2},{3}", windowX + BoxWidth/2, windowY + BoxHeight, x + BoxWidth/2, y);
                    p.Data = Geometry.Parse(geoStr);
                    opBox.path = p;
                    main_grid.Children.Add(p);

                    x += dw;
                }
                */
            }


            return (matBox, windowX + BoxWidth / 2, windowY);

        }

        private void MaterialBoxPut(List<op_stct_count> opList,double width_fix,int all_material,double shift,int level,double windowX,double windowY)
        {
            double dw = width_fix / all_material;
            double x = Width_margin + (dw * shift);

            double y = CalcShowPointY(level + 1);

            foreach (op_stct_count ops in opList)
            {
                Material_StartBox opBox = ShowMaterial(ops, x, y);

                //線を引く
                Path p = new Path();
                p.Stroke = new SolidColorBrush(Color.FromArgb(255, 255, 255, 255));
                p.StrokeThickness = 2;
                string geoStr = string.Format("M{0},{1} L{2},{3}", windowX + BoxWidth / 2, windowY + BoxHeight, x + BoxWidth / 2, y);
                p.Data = Geometry.Parse(geoStr);
                opBox.path = p;
                main_grid.Children.Add(p);

                x += dw;
            }
        }

        private double CalcShowPointY(int ylevel)
        {
            double hight_y = Height_top;
            double dy = BoxHeight + Height_margin;

            return hight_y + (dy*ylevel);
        }

        private OP_MaterialBox ShowMaterial(List<OP_Recipe2> m, int count,double x, double y)
        {
            //int y_count = material_levels[y];
            OP_MaterialBox box = new OP_MaterialBox(tools.add_NULL_Recipe(material_slot, m),count);
            main_grid.Children.Add(box);
            material_boxs.Add(box);

            Thickness p = ConvertXY(x, y, box.Width, box.Height);

            box.Margin = p;

            return box;

        }

        private Material_StartBox ShowMaterial(op_stct_count m, double x, double y)
        {
            //int y_count = material_levels[y];
            Material_StartBox box = new Material_StartBox(tools.add_NULL_op(material_slot,m));
            main_grid.Children.Add(box);
            material_start.Add(box);

            Thickness p = ConvertXY(x, y, box.Width, box.Height);

            box.Margin = p;

            return box;

        }


        //--------------------------------------
        //旧マテリアルツリーコード（廃止）
        //--------------------------------------

        /*
        private IMaterialBox ShowMaterialTree(material material, int ylevel, double x = 0,double y = 0)
        {
            if (showLevel_Temp.Count - 1 < ylevel)
            {
                showLevel_Temp.Add(0);
            }

            (IMaterialBox parent,double Px,double Py) = ShowMaterial(material.Recipes, showLevel_Temp[ylevel], ylevel,x,y);
            showLevel_Temp[ylevel]++;

            foreach (material m in material.material_childs)
            {
                IMaterialBox childBox = ShowMaterialTree(m, ylevel + 1,Px,Py);
                childBox.moveEvent += parent.ChildrenMove;　//これどうにかしたい
                parent.childrenbox.Add(childBox);
            }

            if(material.material_childs.Count == 0)
            {
                
                if(showLevel_Temp.Count < ylevel + 2)
                {
                    showLevel_Temp.Add(0);
                }
                foreach(List<op_stct2> op in material.material_op)
                {
                    IMaterialBox startbox = ShowStartMaterial(op, showLevel_Temp[ylevel + 1], ylevel + 1,Px,Py);
                    startbox.moveEvent += parent.ChildrenMove;
                    showLevel_Temp[ylevel + 1]++;
                    parent.childrenbox.Add(startbox);
                    EventBox.Add(startbox);
                }
            }

            return parent;
        }

        //(double x,double y)の戻り値はboxの下真ん中の座標
        private (IMaterialBox, double,double) ShowMaterial(List<OP_Recipe2> m, int x, int y,double parentX=0,double parentY=0)
        {
            //int y_count = material_levels[y];
            OP_MaterialBox box = new OP_MaterialBox(tools.add_NULL_Recipe(material_slot, m));
            main_grid.Children.Add(box);
            material_boxs.Add(box);

            (Thickness point, string geoStr) = CalcShowPoint(x, y, box.Width, box.Height, parentX, parentY);
            box.Margin = point;

            if (parentX != 0 && parentY != 0)
            {
                /*
                double centerX = pointX + box.Width / 2;
                double centerY = pointY;
                */
        /*

                Path p = new Path();
                p.Stroke = new SolidColorBrush(Color.FromArgb(255, 255, 255, 255));
                p.StrokeThickness = 2;
                //string geoStr = string.Format("M{0},{1} L{2},{3}",centerX,centerY,parentX,parentY);
                p.Data = Geometry.Parse(geoStr);

                box.path = p;
                main_grid.Children.Add(p);
            }

            return (box,point.Left + box.Width /2, point.Top+box.Height);

        }

        private IMaterialBox ShowStartMaterial(List<op_stct2> op,int x,int y,double parentX,double parentY)
        {
            //int y_count = material_levels[y];

            Material_StartBox box = new Material_StartBox(tools.add_NULL_op(material_slot,op));
            main_grid.Children.Add(box);
            material_start.Add(box);

            (Thickness point, string geoStr) = CalcShowPoint(x, y, box.Width, box.Height, parentX, parentY);
            box.Margin = point;

            if (parentX != 0 && parentY != 0)
            {
                /*
                double centerX = pointX + box.Width / 2;
                double centerY = pointY;
                */
        /*
                Path p = new Path();
                p.Stroke = new SolidColorBrush(Color.FromArgb(255, 255, 255, 255));
                p.StrokeThickness = 2;
                //string geoStr = string.Format("M{0},{1} L{2},{3}", centerX, centerY, parentX, parentY);
                p.Data = Geometry.Parse(geoStr);

                box.path = p;
                main_grid.Children.Add(p);
            }

            return box;
        }

        private (Thickness, string) CalcShowPoint(int x, int y, double windowWidth, double windowHeight, double parentX = 0, double parentY = 0)
        {
            int y_count = material_levels[y];

            double width_x = this.Width - Width_margin;
            double height_y = this.Height - Height_top;

            double dx_point = (width_x - ((windowWidth + Width_margin) * y_count)) / (y_count + 1);
            double dy_point = (height_y - ((windowHeight + Height_margin) * (material_levels.Count + 1))) / (material_levels.Count + 2);

            double pointX = dx_point + (dx_point + (windowWidth + Width_margin)) * x;
            double pointY = dy_point + (dy_point + (windowHeight + Height_margin)) * y;

            Thickness point = ConvertXY(pointX + Width_margin, pointY + Height_top, windowWidth, windowHeight);

            string geoStr = "";
            if (parentX != 0 && parentY != 0)
            {
                double centerX = point.Left + windowWidth / 2;
                double centerY = point.Top;
                geoStr = string.Format("M{0},{1} L{2},{3}", centerX, centerY, parentX, parentY);
            }

            return (point,geoStr);
        }
        */



        //右上原点からThicknessを作る
        private Thickness ConvertXY(double x, double y, double wx, double wy)
        {

            return new Thickness(x, y, this.Width - x - wx, this.Height - y - wy);
        }

        //ツリーの深さと幅の探索
        private void Material_Level(material_count material, int level)
        {
            if(level == 0)
            {
                material_levels.Add(1);
            }

            if (material_levels.Count - 1 < level + 2)
            {
                material_levels.Add(0);
            }

            //material_levels[level] +=  material.material_op.Count;
            material_levels[level + 1] += material.materials_childs_count.Count;

            foreach (material_count m in material.materials_childs_count)
            {
                Material_Level(m, level + 1);
            }

            if (material.materials_childs_count.Count == 0)
            {
                if (material_levels.Count < level + 2)
                {
                    material_levels.Add(0);
                }

                material_levels[level + 1] += material.material_op_count.Count;
            }
        }

        //表示領域の縦横の計算
        private (int,int) PageWidthHeight(int slot,int childcount)
        {
            BoxHeight = 140;
            BoxHeight += BoxHeight_OP * (slot - 1);

            int width = (BoxWidth + Width_margin) * childcount;
            width += Width_margin;

            int height = (BoxHeight + Height_margin) * material_levels.Count;
            height += Height_top;

            return (width, height);
        }
    }
}
