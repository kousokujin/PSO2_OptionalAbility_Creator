using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSO2_OptionalAbility_Creator
{
    static class tools
    {
        static public List<T> SubList<T>(List<T> x,List<T> y) where T : IComparable 
        {
            List<T> output = new List<T>();

            foreach(T i in x)
            {
                var i_T = i as IComparable;
                bool exist = false;

                foreach(T j in y)
                {
                    var j_T = j as IComparable;

                    if (i_T.CompareTo(j_T) == 0)
                    {
                        exist = true;
                    }
                }

                if (exist == false)
                {
                    output.Add(i);
                }
            }

            foreach (T j in y)
            {
                var j_T = j as IComparable;
                bool exist = false;

                foreach (T i in x)
                {
                    var i_T = i as IComparable;

                    if (i_T.CompareTo(j_T) == 0)
                    {
                        exist = true;
                    }
                }

                if (exist == false) 
                {
                    var e = output.IndexOf(j);
                    if (e == -1)
                    {
                        output.Add(j);
                    }
                }
            }

            return output;


        }

        //slot_countより少ないop数の場合ゴミで埋める
        public static List<op_stct2> add_NULL_op(int slot_count, List<op_stct2> op)
        {
            if(slot_count == 0)
            {
                return op;
            }

            int d = slot_count - op.Count();
            List<op_stct2> op_fix = op.Select(x => x).ToList();

            for (int i = 0; i < d; i++)
            {
                op_fix.Add(OPDataContainer.GetOP_Stct("none"));
            }

            return op_fix;
        }

        public static List<OP_Recipe2> add_NULL_Recipe(int slot_count, List<OP_Recipe2> op)
        {
            if(slot_count == 0)
            {
                return op;
            }

            int d = slot_count - op.Count();
            List<OP_Recipe2> op_fix = op.Select(x => x).ToList();

            for (int i = 0; i < d; i++)
            {
                var OP_none = RecipeDataContainer.GetOP_Recipes(OPDataContainer.GetOP_Stct("none"));
                op_fix.Add(OP_none[0]);
            }

            return op_fix;
        }
    }
}
