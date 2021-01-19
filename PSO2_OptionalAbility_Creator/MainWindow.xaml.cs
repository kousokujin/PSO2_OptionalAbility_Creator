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
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        OP_CompositionEngine engine;
        //private ObservableCollection<op_stct2> AllOPLists;
        //private ObservableCollection<op_stct2> targetOPList;
        MainWindowData ContextData;

        public MainWindow()
        {
            InitializeComponent();

            engine = new OP_CompositionEngine();
            ContextData = new MainWindowData();
            DataContext = ContextData;

            /*
            AllOPLists = new ObservableCollection<op_stct2>();
            targetOPList = new ObservableCollection<op_stct2>();

            //OPリスト追加
            foreach (op_stct2 o in OPDataContainer.GetAllOP())
            {
                AllOPLists.Add(o);
            }
            */

            OP_ListBox.ItemsSource = ContextData.AllOPLists;
            TargetOP_ListBox.ItemsSource = ContextData.targetOPList;

            CampaignOP_Percent_Combobox.ItemsSource = ContextData.CampaignAdd;
            CampaignOP_Percent_Combobox.SelectedIndex = 0;

            OP_ParcentAdd_ComboBox.ItemsSource = ContextData.ItemAdd;
            OP_ParcentAdd_ComboBox.SelectedIndex = 0;
        }

        /// <summary>
        /// 閉じるボタンイベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        /// <summary>
        /// 最小化ボタンが押されたとき
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MiniWindowButton_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        /// <summary>
        /// 最大化ボタンが押されたとき
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MaxWindowButton_Click(object sender, RoutedEventArgs e)
        {
            if(this.WindowState == WindowState.Normal)
            {
                this.WindowState = WindowState.Maximized;
            }
            else
            {
                this.WindowState = WindowState.Normal;
            }
        }

        //OP検索ボックス
        private void OPSarchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var filterOP = ContextData.AllOPLists.Where(x => x.jp_name.Contains(OPSarchBox.Text)).ToList();
            OP_ListBox.ItemsSource = filterOP;
        }

        private void Add_OP_Button_Click(object sender, RoutedEventArgs e)
        {
            var selectOP = OP_ListBox.SelectedItem;

            if(selectOP is op_stct2)
            {
                op_stct2 op = (op_stct2)selectOP;
                ContextData.targetOPList.Add(op);

                var isDuped = OP_CompositionEngine.checkOP(ContextData.targetOPList.ToList());
                if(isDuped.Count != 0)
                {
                    foreach(var op_d in isDuped)
                    {
                        ContextData.targetOPList.Remove(op_d);
                    }
                    ContextData.targetOPList.Add(op);
                }

                if(ContextData.targetOPList.Count == 8)
                {
                    Add_OP_Button.IsEnabled = false;
                }

                Remove_OP_Button.IsEnabled = true;
                StartButton.IsEnabled = true;
            }

            TargetOP_ListBox.ItemsSource = ContextData.targetOPList;
        }

        private void Remove_OP_Button_Click(object sender, RoutedEventArgs e)
        {
            var selectOP = TargetOP_ListBox.SelectedItem;

            if(selectOP is op_stct2)
            {
                op_stct2 op = (op_stct2)selectOP;
                ContextData.targetOPList.Remove(op);

                if(ContextData.targetOPList.Count == 0)
                {
                    Remove_OP_Button.IsEnabled = false;
                    StartButton.IsEnabled = false;
                }
                Add_OP_Button.IsEnabled = true;

            }

            TargetOP_ListBox.ItemsSource = ContextData.targetOPList;

        }

        private void AllRemove_Click(object sender, RoutedEventArgs e)
        {
            ContextData.targetOPList.Clear();
            TargetOP_ListBox.ItemsSource = ContextData.targetOPList;
            Add_OP_Button.IsEnabled = true;
            Remove_OP_Button.IsEnabled = false;
            StartButton.IsEnabled = false;

        }

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            material m = OP_CompositionEngine.SerchOP(ContextData.targetOPList.ToArray());
            var tree_page = TreeFrame.Content;

            if(tree_page is OP_TreePage)
            {
                var page = (OP_TreePage)tree_page;
                page.ShowMaterialTree(m);
                //Console.WriteLine("FrameViewBox Width:{0}, Height:{1}", TreeFrame_ViewBox.Width, TreeFrame_ViewBox.Height);

                double edge_X = Width - (TreeFrameBoader.Margin.Left + TreeFrameBoader.Margin.Right);
                double edge_Y = Height - (TreeFrameBoader.Margin.Top + TreeFrameBoader.Margin.Bottom);
                TreeFrame_ViewBox.Width = edge_X;
                TreeFrame_ViewBox.Height = edge_Y;

            }
        }

        //設計図でマウスホイール動いたら
        private void ScrollViewer_Wheel(MouseWheelEventArgs e)
        {
            ScrollViewer scrollviewer = Tree_Scroll;

            KeyStates key_shiftL = KeyStateFilter(Keyboard.GetKeyStates(Key.LeftShift));
            KeyStates key_shiftR = KeyStateFilter(Keyboard.GetKeyStates(Key.RightShift));

            if (key_shiftL == KeyStates.Down || key_shiftR == KeyStates.Down)
            {
                if (e.Delta > 0)
                {
                    scrollviewer.LineLeft();
                }
                else
                {
                    scrollviewer.LineRight();
                }
            }
            else
            {
                if (e.Delta > 0)
                {
                    scrollviewer.LineUp();
                }
                else
                {
                    scrollviewer.LineDown();
                }
            }

            e.Handled = true;
        }

        //拡大縮小
        private void ViewBoxWheel(MouseWheelEventArgs e)
        {
            KeyStates ctrlL = KeyStateFilter(Keyboard.GetKeyStates(Key.LeftCtrl));
            KeyStates ctrlR = KeyStateFilter(Keyboard.GetKeyStates(Key.RightCtrl));

            int scale = 1;
            double offset = (TreeFrame_ViewBox.Width + TreeFrame_ViewBox.Height)/20;

            if (ctrlL == KeyStates.Down || ctrlR == KeyStates.Down)
            {
                /*
                if (TreeFrame_ViewBox.Width != double.NaN && TreeFrame_ViewBox.Height != double.NaN)
                {
                    double dWidth = offset*scale;
                    double dHeight = offset*scale;

                    if (e.Delta > 0)
                    {
                        TreeFrame_ViewBox.Width += dWidth;
                        TreeFrame_ViewBox.Height += dHeight;
                        Tree_Scroll.ScrollToVerticalOffset(Tree_Scroll.VerticalOffset + (dHeight/2));
                        Tree_Scroll.ScrollToHorizontalOffset(Tree_Scroll.HorizontalOffset + (dWidth/2));
                        //scale++;
                    }
                    else
                    {
                        if (TreeFrame_ViewBox.Width > dWidth && TreeFrame_ViewBox.Height > dHeight)
                        {

                            TreeFrame_ViewBox.Width -= dWidth;
                            TreeFrame_ViewBox.Height -= dHeight;
                            Tree_Scroll.ScrollToVerticalOffset(Tree_Scroll.VerticalOffset - (dHeight/2));
                            Tree_Scroll.ScrollToHorizontalOffset(Tree_Scroll.HorizontalOffset - (dWidth/2));
                            //scale--;
                        }
                    }
                }
                */

                if (e.Delta > 0)
                {
                    ScaleUPandDown(false);
                }
                else
                {
                    ScaleUPandDown(true);
                }

            }
        }

        private void ScaleUPandDown(bool isScaledown)
        {
            double offset = (TreeFrame_ViewBox.Width + TreeFrame_ViewBox.Height) / 20;

            if (TreeFrame_ViewBox.Width != double.NaN && TreeFrame_ViewBox.Height != double.NaN)
            {
                double dWidth = offset;
                double dHeight = offset;

                if (isScaledown == false)
                {
                    TreeFrame_ViewBox.Width += dWidth;
                    TreeFrame_ViewBox.Height += dHeight;
                    Tree_Scroll.ScrollToVerticalOffset(Tree_Scroll.VerticalOffset + (dHeight / 2));
                    Tree_Scroll.ScrollToHorizontalOffset(Tree_Scroll.HorizontalOffset + (dWidth / 2));
                    //scale++;
                }
                else
                {
                    if (TreeFrame_ViewBox.Width > dWidth && TreeFrame_ViewBox.Height > dHeight)
                    {

                        TreeFrame_ViewBox.Width -= dWidth;
                        TreeFrame_ViewBox.Height -= dHeight;
                        Tree_Scroll.ScrollToVerticalOffset(Tree_Scroll.VerticalOffset - (dHeight / 2));
                        Tree_Scroll.ScrollToHorizontalOffset(Tree_Scroll.HorizontalOffset - (dWidth / 2));
                        //scale--;
                    }
                }
            }
        }

        private KeyStates KeyStateFilter(KeyStates k)
        {
            var key_OK = KeyStates.Down | KeyStates.Toggled;

            if(k == key_OK)
            {
                return KeyStates.Down;
            }
            else
            {
                return k;
            }
        }

        //Ctrlで拡大縮小
        private void TreeFrameBoader_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            ScrollViewer_Wheel(e);
            ViewBoxWheel(e);
            e.Handled = true;
        }

        //拡大ボタン
        private void ScaleUpButton_Click(object sender, RoutedEventArgs e)
        {
            ScaleUPandDown(false);
        }
        //縮小ボタン
        private void ScaleDownButton_Click(object sender, RoutedEventArgs e)
        {
            ScaleUPandDown(true);
        }
    }

    public class MainWindowData
    {
        public ObservableCollection<op_stct2> AllOPLists;
        public ObservableCollection<op_stct2> targetOPList;

        //特殊能力成功率向上アイテム
        public ObservableCollection<PercentAdd> ItemAdd;
        //弱体化特殊能力成功率向上
        public ObservableCollection<PercentAdd> CampaignAdd;

        public MainWindowData()
        {
            AllOPLists = new ObservableCollection<op_stct2>();
            targetOPList = new ObservableCollection<op_stct2>();
            ItemAdd = new ObservableCollection<PercentAdd>();
            CampaignAdd = new ObservableCollection<PercentAdd>();

            //OPリスト追加
            foreach (op_stct2 o in OPDataContainer.GetAllOP())
            {
                AllOPLists.Add(o);
            }

            ItemAdd.Add(new PercentAdd() { addPercent = 0, itemName = "なし" });
            ItemAdd.Add(new PercentAdd() { addPercent = 5, itemName = "+5%" });
            ItemAdd.Add(new PercentAdd() { addPercent = 10, itemName = "+10%" });
            ItemAdd.Add(new PercentAdd() { addPercent = 20, itemName = "+20%" });
            ItemAdd.Add(new PercentAdd() { addPercent = 30, itemName = "+30%" });
            ItemAdd.Add(new PercentAdd() { addPercent = 40, itemName = "+40%" });
            ItemAdd.Add(new PercentAdd() { addPercent = 45, itemName = "+45%" });
            ItemAdd.Add(new PercentAdd() { addPercent = 50, itemName = "+50%" });
            ItemAdd.Add(new PercentAdd() { addPercent = 55, itemName = "+55%" });
            ItemAdd.Add(new PercentAdd() { addPercent = 60, itemName = "+60%" });

            CampaignAdd.Add(new PercentAdd() { addPercent = 0, itemName = "なし" });
            CampaignAdd.Add(new PercentAdd() { addPercent = 5, itemName = "+5%" });
            CampaignAdd.Add(new PercentAdd() { addPercent = 10, itemName = "+10%" });
            CampaignAdd.Add(new PercentAdd() { addPercent = 15, itemName = "+15%" });
            CampaignAdd.Add(new PercentAdd() { addPercent = 18, itemName = "+18%" });
            CampaignAdd.Add(new PercentAdd() { addPercent = -1, itemName = "打撃力系" });
            CampaignAdd.Add(new PercentAdd() { addPercent = -2, itemName = "射撃力系" });
            CampaignAdd.Add(new PercentAdd() { addPercent = -3, itemName = "法撃力系" });
            CampaignAdd.Add(new PercentAdd() { addPercent = -4, itemName = "HP/PP系" });
            CampaignAdd.Add(new PercentAdd() { addPercent = -5, itemName = "特殊系" });

        }
    }

    public class PercentAdd
    {
        public int addPercent;
        public string itemName;

        public override string ToString()
        {
            return itemName;
        }
    }
}
