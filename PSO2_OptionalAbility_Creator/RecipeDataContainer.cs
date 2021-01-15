using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSO2_OptionalAbility_Creator
{
    class RecipeDataContainer
    {
        static IRecipeContainer recipe_data;
        public RecipeDataContainer(IRecipeContainer recipedata)
        {
            recipe_data = recipedata;
        }

        static public List<OP_Recipe2> GetOP_Recipes(op_stct2 op)
        {
            return recipe_data.GetOP_Recipe(op);
        }
    }

    class OPDataContainer
    {
        static IOPContainer op_Data;
        public OPDataContainer(IOPContainer opdata)
        {
            op_Data = opdata;
        }

        static public op_stct2 GetOP_Stct(string op_name)
        {
            return op_Data.GetOP_Stct(op_name);
        }

        static public List<op_stct2> GetAllOP()
        {
            return op_Data.GetAllOPStct();
        }

       
    }

    interface IRecipeContainer
    {
        /// <summary>
        /// opに指定されたレシピを返す
        /// </summary>
        /// <param name="op">op</param>
        /// <returns>レシピ</returns>
        List<OP_Recipe2> GetOP_Recipe(op_stct2 op);

    }

    interface IOPContainer
    {
        op_stct2 GetOP_Stct(string op_name);
        List<op_stct2> GetAllOPStct();
    }

    //OP情報構造(sqlite)
    public struct op_stct2
    {
        public string jp_name;
        public string op_name;
        public string series;

        public override string ToString()
        {
            return jp_name;
        }
    }

    struct OP_Recipe2
    {
        public op_stct2 name;  //完成品
        public List<op_stct2> materials;   //必要な素材
        public int percent;    //成功確率
    }
}
