using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;

namespace PSO2_OptionalAbility_Creator
{
    public class RecipeData_Memory : MemorySQL_Load, IRecipeContainer
    {
        static public List<OP_Recipe2> op_recipes;

        public RecipeData_Memory(string datasource) : base(datasource, "SELECT * FROM OP_Recipes ")
        {

        }

        public override void init()
        {
            op_recipes = new List<OP_Recipe2>();
        }

        public override void load(SQLiteDataReader data)
        {
            op_stct2 out_name =  OPDataContainer.GetOP_Stct(data.GetValue(1) as string);
            List<op_stct2> out_marteials = new List<op_stct2>();

            for(int i=2; i <= 7; i++)
            {
                var s = data.GetValue(i);

                if (s is System.DBNull == false)
                {
                    op_stct2 mat_temp = OPDataContainer.GetOP_Stct(s as string);
                    out_marteials.Add(mat_temp);
                }
            }

            var per_str = data.GetValue(8) as long?;
            int out_percent = 0;

            if(per_str != null)
            {
                out_percent = (int)per_str;
            }

            OP_Recipe2 recp = new OP_Recipe2()
            {
                name = out_name,
                materials = out_marteials,
                percent = out_percent,
                AddPercent = 0,
                noRecipe = false
            };

            op_recipes.Add(recp);
        }

        public List<OP_Recipe2> GetOP_Recipe(op_stct2 op)
        {
            List<OP_Recipe2> op_r = op_recipes.Where(x => (x.name.op_name == op.op_name)).ToList();
            return op_r;
        }
    }
}
