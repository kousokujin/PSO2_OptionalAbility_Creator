using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSO2_OptionalAbility_Creator
{
    public class OP_CompositionEngine2
    {

        /// <summary>
        /// 引数に指定したOPが共存できるか
        /// </summary>
        /// <param name="ops">オプション</param>
        /// <returns>共存不可のオプション</returns>
        public static List<op_stct2> checkOP(List<op_stct2> ops)
        {
            //共存不可OP
            List<op_stct2> duplOP = new List<op_stct2>();

            List<string> dupSeries_checked = new List<string>();
            List<string> dupSeries = new List<string>();

            foreach (op_stct2 op in ops)
            {

                if (op.op_name == "none")
                {
                    continue;
                }

                //共存不可(ソールとか)があるかチェック
                if (dupSeries_checked.Contains(op.series) == true)
                {
                    if (dupSeries.Contains(op.series) == false)
                    {
                        dupSeries.Add(op.series);
                    }
                }
                else
                {
                    dupSeries_checked.Add(op.series);
                }
            }

            foreach (string d in dupSeries)
            {
                List<op_stct2> dupOP_st = ops.Where(x => (x.series == d)).ToList();
                foreach (op_stct2 op in dupOP_st)
                {
                    duplOP.Add(op);
                }
            }

            return duplOP;
        }

        public static (List<string>, List<int>) seriseCount(List<op_stct2> ops)
        {
            List<int> count = new List<int>();
            List<string> opseries = new List<string>();

            foreach (op_stct2 o in ops)
            {
                if (opseries.Contains(o.series) == false)
                {
                    opseries.Add(o.series);
                    count.Add(1);
                }
                else
                {
                    int i = opseries.IndexOf(o.series);
                    count[i]++;
                }
            }

            return (opseries, count);

        }

        /// <summary>
        /// opに指定された素材と、確率を返す。
        /// percent_plusで成功確率を上げる
        /// </summary>
        /// <param name="op">素材</param>
        /// <param name="percent_Plus">確率上昇率</param>
        /// <returns>素材</returns>
        static public List<OP_Recipe2> GetMaterials(op_stct2 op, int percent_Plus = 0, int camp_parcent = 0)
        {
            var comArr = RecipeDataContainer.GetOP_Recipes(op, camp_parcent);
            //成功率が低い順にソートしているが、priorityを設定して高い順にしたい
            comArr = comArr.OrderBy(x => x.percent).ToList();

            if (comArr.Count == 0)
            {
                Console.WriteLine("Recepi Not Found:{0}", op.jp_name);
                //return GetMaterials(OPDataContainer.GetOP_Stct("none"));
                return new List<OP_Recipe2>();
            }

            List<OP_Recipe2> opc = new List<OP_Recipe2>();

            foreach (OP_Recipe2 o in comArr)
            {
                int percent_plus_temp = 0;
                int[] percent_plus_items = new int[] { 0, 10, 20, 30, 40, 45, 50, 55, 60 };

                foreach (int px in percent_plus_items)
                {
                    if (px <= percent_Plus)
                    {

                        int p = o.percent + px;
                        percent_plus_temp = px;
                        if (p >= 100)
                        {
                            break;
                        }
                    }
                }

                int p_temp = o.percent + percent_plus_temp;
                if (p_temp > 100)
                {
                    p_temp = 100;
                }

                opc.Add(new OP_Recipe2() { name = o.name, percent = p_temp, materials = o.materials, AddPercent = percent_Plus, noRecipe = false });

            }

            //成功率を高い順に変える
            opc.Reverse();

            //opc.AddPercent = percent_plus_temp;
            return opc;

        }

        /// <summary>
        /// opに指定された素材(複数)と必要な素材を返す
        /// </summary>
        /// <param name="op">素材（複数）</param>
        /// <param name="percent_Plus">確率上昇率
        /// [
        ///     target = op_stct つけるop
        ///     material = List<op_stct> 必要なop
        ///     percent = int 成功確率
        /// ]
        /// </param>
        /// <returns></returns>
        static public List<List<OP_Recipe2>> GetMaterials(List<op_stct2> op, int percent_Plus = 0, int camp_parcent = 0)
        {
            List<List<OP_Recipe2>> output_op = new List<List<OP_Recipe2>>();

            foreach (op_stct2 o in op)
            {
                var op_t = GetMaterials(o, percent_Plus, camp_parcent);
                output_op.Add(op_t);
            }

            return output_op;
        }

        static public (List<OP_Recipe2>, List<List<op_stct2>>) SerchOP_materialBodys(List<OP_Recipe2> recipe)
        {
            List<List<op_stct2>> output_material_bodys = new List<List<op_stct2>>();
            List<OP_Recipe2> NG_Recipe = new List<OP_Recipe2>();

            //何個目の素材にいれるか
            int slot = 0;

            foreach (OP_Recipe2 res in recipe)
            {
                bool NG = false;
                foreach (op_stct2 op in res.materials)
                {
                    bool loop = true;
                    int loopcount = 0;

                    if (NG == false)
                    {

                        do
                        {
                            if (output_material_bodys.Count < (slot + 1))
                            {
                                output_material_bodys.Add(new List<op_stct2>());
                            }

                            output_material_bodys[slot].Add(op);

                            List<op_stct2> outputdupList = checkOP(output_material_bodys[slot]);

                            if (outputdupList.Count == 0)
                            {
                                loop = false;
                            }
                            else
                            {
                                //共存不可なのでやり直し
                                output_material_bodys[slot].RemoveAt(output_material_bodys[slot].Count - 1);
                            }

                            slot = (slot + 1) % 6;
                            loopcount++;

                            if (loopcount > 6)
                            {
                                NG_Recipe.Add(res);
                                NG = true;
                                loop = false;
                            }
                        } while (loop == true);
                    }
                }
            }

            return (NG_Recipe, output_material_bodys);

        }

        //recipeに指定されたのに必要なOPを返す
        static public List<op_stct2> NeedOPs(List<OP_Recipe2> recipe)
        {
            List<op_stct2> needOP = new List<op_stct2>();

            foreach (OP_Recipe2 opr in recipe)
            {
                foreach (op_stct2 o in opr.materials)
                {
                    needOP.Add(o);
                }
            }

            return needOP;
        }

        static public List<OP_Recipe2> CanMargeCheck(List<OP_Recipe2> recipes)
        {
            List<op_stct2> needops = NeedOPs(recipes);
            (List<string> serise, List<int> count) = seriseCount(needops);

            List<OP_Recipe2> output = new List<OP_Recipe2>();

            foreach ((int c, int idx) in count.Select((x, i) => (x, i)))
            {
                //6個以上は合成不可
                if (c > 6)
                {
                    string s = serise[idx];

                    foreach (OP_Recipe2 opr in recipes)
                    {
                        foreach (op_stct2 st in opr.materials)
                        {
                            if (st.series == s)
                            {
                                output.Add(opr);
                                break;
                            }
                        }
                    }
                }
            }

            return output;

        }

        static public (bool next,List<int>) SelectRecipe(List<List<OP_Recipe2>> recipes, List<int> selected)
        {
            List<int> maxlevel = recipes.Select(x => x.Count).ToList();
            List<int> outputarr = selected;
            //maxlevel = maxlevel.Select(x => x - 1).ToList();
            int maxval = selected.Max();

            bool change = false;
            for (int i = 0; i < maxval; i++)
            {
                foreach ((int x, int idx) in maxlevel.Select((x, j) => (x, j)))
                {
                    if(change == false &&　selected[idx] < i && maxlevel[idx] > i)
                    {
                        outputarr[idx] = i;
                        change = true;
                    }
                }
            }

            return (change, outputarr);

        }

        static public List<OP_Recipe2> createReciArray(List<List<OP_Recipe2>> recipelist,List<int> indexes)
        {
            List<OP_Recipe2> returnrecipe = new List<OP_Recipe2>();
            foreach ((List<OP_Recipe2> u, int idx) in recipelist.Select((x, i) => (x, i)))
            {
                returnrecipe.Add(u[indexes[idx]]);
            }

            return returnrecipe;
        }

        //ソール→ソール+ソールみたいなレシピかどうか
        static public bool SimpleRecipe(List<OP_Recipe2> recipes)
        {
            if(recipes.Count == 1)
            {
                OP_Recipe2 temprecipe = recipes[0];
                bool isDup = true;

                foreach(op_stct2 o in temprecipe.materials)
                {
                    if(o.op_name != temprecipe.name.op_name)
                    {
                        isDup = false;
                        break;
                    }
                }

                return isDup;
            }
            else
            {
                return false;
            }
        }

        static public material SerchOP(op_stct2[] target, int percent_plus = 0, int camp_percent = 0)
        {
            List<List<OP_Recipe2>> recipes = GetMaterials(target.ToList(), percent_plus, camp_percent);
            List<int> UseRecipeIndexs = new List<int>();
            recipes.ForEach(x => UseRecipeIndexs.Add(0));

            //すべてのレシピが存在するか
            foreach (List<OP_Recipe2> recipe in recipes)
            {
                //存在しないレシピがあったら
                if(recipe.Count == 0)
                {
                    return new material()
                    {
                        material_op = new List<List<op_stct2>>(),
                        material_end = new List<List<op_stct2>>(),
                        Recipes = createReciArray(recipes,UseRecipeIndexs),
                        error = "NOT_EXIST_MARGE"
                    };

                }
            }

            //レシピの決定
            bool loop = true;
            do
            {
                List<OP_Recipe2> UseRecipe_temp = createReciArray(recipes, UseRecipeIndexs);

                //干渉するOPの数の確認
                List<OP_Recipe2> dup_recipe = CanMargeCheck(UseRecipe_temp);
                if(dup_recipe.Count > 0)
                {
                    (bool next, List<int> idses) = SelectRecipe(recipes, UseRecipeIndexs);

                    if(next == true)
                    {
                        UseRecipeIndexs = idses;
                    }
                    else
                    {
                        return new material()
                        {
                            material_op = new List<List<op_stct2>>(),
                            material_end = new List<List<op_stct2>>(),
                            Recipes = UseRecipe_temp,
                            error = "NOT_RECIPE_MARGE",
                        };

                    }
                }
                else
                {
                    loop = false;
                }

            } while (loop == true);

            List<OP_Recipe2> UseRecipe = createReciArray(recipes, UseRecipeIndexs);
            
            //ソール→ソール+ソールみたいなのだったら終わり
            bool end = SimpleRecipe(UseRecipe);
            if(end == true)
            {
                List<List<op_stct2>> opends = new List<List<op_stct2>>();
                foreach(op_stct2 o in UseRecipe[0].materials)
                {
                    List<op_stct2> op_arr = new List<op_stct2>() { o };
                    opends.Add(op_arr);
                }

                return new material()
                {
                    material_op = new List<List<op_stct2>>(),
                    material_end = opends,
                    Recipes = UseRecipe,
                    error = ""
                };

            }


            //ここらへんまだ未完全

            (List<OP_Recipe2> ngRecipe, List<List<op_stct2>> mat) = SerchOP_materialBodys(createReciArray(recipes, UseRecipeIndexs));

            return new material()
            {
                material_op = mat,
                material_end = new List<List<op_stct2>>(),
                Recipes = createReciArray(recipes, UseRecipeIndexs),
                error = ""
            };

        }


    }
}
