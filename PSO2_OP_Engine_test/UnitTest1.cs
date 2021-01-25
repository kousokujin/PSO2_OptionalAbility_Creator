using Microsoft.VisualStudio.TestTools.UnitTesting;
using PSO2_OptionalAbility_Creator;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PSO2_OP_Engine_test
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            OPData_Memory sqtest = new OPData_Memory("pso2_op_base.db");
            var opdataContainer = new OPDataContainer(sqtest);

            RecipeData_Memory res = new RecipeData_Memory("pso2_op_base.db");
            var recipeDataContainer = new RecipeDataContainer(res);


            List<string> op_str = new List<string>() { "astral_soul", "mana_rev" };
            List<op_stct2> getop = op_str.Select(x => OPDataContainer.GetOP_Stct(x)).ToList();
            List<List<OP_Recipe2>> getrecipe = getop.Select(x => RecipeDataContainer.GetOP_Recipes(x,0)).ToList();
            List<OP_Recipe2> SelectRecipe = getrecipe.Select(x => x[0]).ToList();

            material m = OP_CompositionEngine2.SerchOP(getop.ToArray());
        }
    }
}
