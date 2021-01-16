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
        OP_MaterialBox test;
        public OP_TreePage()
        {
            InitializeComponent();
            List<OP_Recipe2> test_recipe = new List<OP_Recipe2>();
            //とりあえずテスト
            test_recipe.Add(RecipeDataContainer.GetOP_Recipes(OPDataContainer.GetOP_Stct("power4"))[0]);
            test_recipe.Add(RecipeDataContainer.GetOP_Recipes(OPDataContainer.GetOP_Stct("stamina4"))[0]);
            //test_recipe.Add(RecipeDataContainer.GetOP_Recipes(OPDataContainer.GetOP_Stct("supirita4"))[0]);

            test = new OP_MaterialBox(test_recipe);
            test.Name = "test_materialBox";
            main_grid.Children.Add(test);
        }
    }
}
