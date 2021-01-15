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
        private ObservableCollection<op_stct2> AllOPLists;
        private ObservableCollection<op_stct2> targetOPList;

        public MainWindow()
        {
            InitializeComponent();
            engine = new OP_CompositionEngine();
            AllOPLists = new ObservableCollection<op_stct2>();
            targetOPList = new ObservableCollection<op_stct2>();

            //OPリスト追加
            foreach (op_stct2 o in OPDataContainer.GetAllOP())
            {
                AllOPLists.Add(o);
            }

            OP_ListBox.ItemsSource = AllOPLists;
            //TargetOP_ListBox.ItemsSource = targetOPList;
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
            var filterOP = AllOPLists.Where(x => x.jp_name.Contains(OPSarchBox.Text)).ToList();
            OP_ListBox.ItemsSource = filterOP;
        }

        private void Add_OP_Button_Click(object sender, RoutedEventArgs e)
        {
            var selectOP = OP_ListBox.SelectedItem;

            if(selectOP is op_stct2)
            {
                op_stct2 op = (op_stct2)selectOP;
                targetOPList.Add(op);

                var isDuped = OP_CompositionEngine.checkOP(targetOPList.ToList());
                if(isDuped.Count != 0)
                {
                    foreach(var op_d in isDuped)
                    {
                        targetOPList.Remove(op_d);
                    }
                    targetOPList.Add(op);
                }

                if(targetOPList.Count == 8)
                {
                    Add_OP_Button.IsEnabled = false;
                }

                Remove_OP_Button.IsEnabled = true;
            }

            TargetOP_ListBox.ItemsSource = targetOPList;
        }

        private void Remove_OP_Button_Click(object sender, RoutedEventArgs e)
        {
            var selectOP = TargetOP_ListBox.SelectedItem;

            if(selectOP is op_stct2)
            {
                op_stct2 op = (op_stct2)selectOP;
                targetOPList.Remove(op);

                if(targetOPList.Count == 0)
                {
                    Remove_OP_Button.IsEnabled = false;
                }
                Add_OP_Button.IsEnabled = true;

            }

            TargetOP_ListBox.ItemsSource = targetOPList;

        }

        private void AllRemove_Click(object sender, RoutedEventArgs e)
        {
            targetOPList.Clear();
            TargetOP_ListBox.ItemsSource = targetOPList;
            Add_OP_Button.IsEnabled = true;
            Remove_OP_Button.IsEnabled = false;

        }
    }
}
