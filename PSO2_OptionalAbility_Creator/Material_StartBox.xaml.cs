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
        public List<Path> PathList;
        public Material_StartBox(List<op_stct2> material)
        {
            InitializeComponent();

            this.material = new ObservableCollection<op_stct2>();
            material.ForEach(x => this.material.Add(x));
            PathList = new List<Path>();

            OP_ListBox.ItemsSource = this.material;

            int add_height = 22 * (material.Count - 1);
            Height += add_height;
        }
    }
}
