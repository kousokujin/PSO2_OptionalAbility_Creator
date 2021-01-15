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

        private void OP_ListBox_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var lb = sender as ListBox;

            if (lb != null && lb.SelectedItem != null)
            {
                DragDrop.DoDragDrop(lb, lb.SelectedItem, DragDropEffects.Move);
            }
        }

        private void TargetOP_DragOver(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(ListBoxItem)))
            {
                e.Effects = DragDropEffects.Move;
            }
        }

        private void TargetOP_ListBox_Drop(object sender, DragEventArgs e)
        {
            var lb = sender as ListBox;
            foreach(var x in lb.Items)
            {
                Console.WriteLine(x);
            }

            var item = e.Data.GetData(typeof(op_stct2));
            Console.WriteLine((op_stct2)item);

            if(item != null && item is op_stct2)
            {
                op_stct2 op_item = (op_stct2)item;
                //targetOPList.Add(op_item);

                //var isDup = OP_CompositionEngine.checkOP(targetOPList.ToList());

                /*
                if(isDup.Count() != 0)
                {
                    targetOPList.RemoveAt(targetOPList.Count() - 1);
                }
                */
                //TargetOP_ListBox.ItemsSource = targetOPList;

            }
        }
    }
}
