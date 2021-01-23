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
        public static op_stct_count add_NULL_op(int slot_count, op_stct_count op)
        {
            if(slot_count == 0)
            {
                return op;
            }

            int d = slot_count - op.name.Count();
            List<op_stct2> op_fix = op.name.Select(x => x).ToList();

            for (int i = 0; i < d; i++)
            {
                op_fix.Add(OPDataContainer.GetOP_Stct("none"));
            }

            return new op_stct_count() { count = op.count, name = op_fix };
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

        //materialの一番根の数を数える
        public static int CountMaterial(material material)
        {
            int sum = 0;
            foreach(material m in material.material_childs)
            {
                sum += CountMaterial(m);
            }

            if (material.material_end != null)
            {
                sum += material.material_end.Count;
            }

            if(material.material_childs.Count == 0)
            {
                return material.material_op.Count;
            }

            return sum;

        }

        public static int CountMaterial(material_count material)
        {
            int sum = 0;
            foreach (material_count m in material.materials_childs_count)
            {
                sum += CountMaterial(m);
            }

            if (material.material_end_count != null)
            {
                sum += material.material_end_count.Count;
            }

            if (material.materials_childs_count.Count == 0 && material.material_end_count.Count == 0)
            {
                if (material.material_op_count.Count == 0)
                {
                    return 1;
                }
                else
                {
                    return material.material_op_count.Count;
                }
            }

            return sum;

        }
    }
}
