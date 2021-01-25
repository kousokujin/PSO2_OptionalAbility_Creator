using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSO2_OptionalAbility_Creator
{
    public class OP_CompositionEngine
    {
        OPDataContainer opdataContainer;
        RecipeDataContainer recipeDataContainer;

        public OP_CompositionEngine()
        {
            OPData_Memory sqtest = new OPData_Memory("pso2_op_base.db");
            opdataContainer = new OPDataContainer(sqtest);

            RecipeData_Memory res = new RecipeData_Memory("pso2_op_base.db");
            recipeDataContainer = new RecipeDataContainer(res);
        }

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

                if(op.op_name == "none")
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

            foreach(string d in dupSeries)
            {
                List<op_stct2> dupOP_st = ops.Where(x => (x.series == d)).ToList();
                foreach(op_stct2 op in dupOP_st)
                {
                    duplOP.Add(op);
                }
            }

            return duplOP;
        }

        public static List<int> seriseCount(List<op_stct2> ops)
        {
            List<int> count = new List<int>();
            List<string> opseries = new List<string>();

            foreach(op_stct2 o in ops)
            {
                if(opseries.Contains(o.series) == false)
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

            return count;

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
            var comArr = RecipeDataContainer.GetOP_Recipes(op,camp_parcent);
            //成功率が低い順にソートしているが、priorityを設定して高い順にしたい
            comArr = comArr.OrderBy(x => x.percent).ToList();

            if(comArr.Count == 0)
            {
                Console.WriteLine("Recepi Not Found:{0}", op.jp_name);
                return GetMaterials(OPDataContainer.GetOP_Stct("none"));
            }

            List<OP_Recipe2> opc = new List<OP_Recipe2>();

            foreach (OP_Recipe2 o in comArr)
            {
                int percent_plus_temp = 0;
                int[] percent_plus_items = new int[] { 0, 10, 20, 30, 40, 45, 50, 55, 60 };

                foreach (int px in percent_plus_items)
                {
                    if (px <= percent_Plus) {

                        int p = o.percent + px;
                        percent_plus_temp = px;
                        if (p >= 100)
                        {
                            break;
                        }
                    }
                }

                int p_temp = o.percent + percent_plus_temp;
                if(p_temp > 100)
                {
                    p_temp = 100;
                }

                opc.Add(new OP_Recipe2() { name = o.name, percent = p_temp, materials = o.materials, AddPercent = percent_Plus, noRecipe = false });

            }

            //成功率を高い順に変える
            opc.Reverse();

            //opc.AddPercent = percent_plus_temp;
            return opc;

            /*
            List<op_stct2> op_s = new List<op_stct2>();

            foreach (op_stct2 name in opc.materials)
            {
                op_s.Add(name);
            }

            int output_p = opc.percent + percent_Plus;
            if (output_p > 100)
            {
                output_p = 100;
            }
            return (output_p, op_s);
            */

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
                var op_t = GetMaterials(o, percent_Plus,camp_parcent);

                /*
                var data = new OP_Recipe2()
                {
                    name = o,
                    percent = op_t.Item1,
                    materials = op_t.Item2
                };
                */

                output_op.Add(op_t);
            }

            return output_op;
        }

        /// <summary>
        /// targetで指定されたOPを合成するために必要な素材を推定する。
        /// </summary>
        /// <param name="target">ほしいOP</param>
        /// <param name="percent_plus">特殊能力成功確率上昇</param>
        /// <returns>
        /// 合成成功確率,必要な素材
        /// </returns>
        static public material SerchOP_materialBodys(op_stct2[] target, int percent_plus = 0, int camp_parcent = 0)
        {
            List<List<op_stct2>> output_material_bodys = new List<List<op_stct2>>();
            List<List<int>> output_material_index = new List<List<int>>();

            List<List<OP_Recipe2>> output_target = GetMaterials(target.ToList(), percent_plus, camp_parcent);
            List<List<OP_Recipe_flag>> need_materials = new List<List<OP_Recipe_flag>>();
            List<int> recipe_index = new List<int>();   //何番目のレシピを使うか

            foreach(List<OP_Recipe2> oprecipe in output_target)
            {
                need_materials.Add(oprecipe.Select(x=>new OP_Recipe_flag(x)).ToList());
                recipe_index.Add(0);
            }


            //レシピがない場合はおわり
            //一番最初の配列を参照してるのは仮
            bool exist_recipe_flag = output_target[0].All(x => (x.name.op_name != "none"));

            if (exist_recipe_flag == false)
            {
                List<OP_Recipe2> notexistrecipe = new List<OP_Recipe2>();
                foreach(op_stct2 t in target)
                {
                    OP_Recipe2 newrecipe = new OP_Recipe2()
                    {
                        name = t,
                        materials = new List<op_stct2>(),
                        percent = 100,
                        AddPercent = 0,
                        noRecipe = true
                    };

                    notexistrecipe.Add(newrecipe);
                }

                return new material()
                {
                    material_op = new List<List<op_stct2>>(),
                    material_end = new List<List<op_stct2>>(),
                    Recipes = notexistrecipe,
                    error = "RECIPE_NOT_EXIST"
                };
            }


            //1sでソール <= ソール+ソールみたいなのは次で無限ループになってしまう。

            if (target.Length == 1)
            {
                List<bool> commonOP = new List<bool>();
                foreach (OP_Recipe2 r in output_target[0])
                {
                    bool fg = true;

                    foreach (op_stct2 o in r.materials)
                    {
                        if (r.name.op_name != o.op_name)
                        {
                            fg = false;
                        }
                    }

                    commonOP.Add(fg);
                }

                if (commonOP.All(x => (x == true)))
                {
                    //List<OP_Recipe2> select_recipe = output_target[0];

                    foreach(List<OP_Recipe2> selected_recipe in output_target)
                    {
                        foreach((op_stct2 op,int idx)in selected_recipe[0].materials.Select((x,i)=>(x,i)))
                        {
                            if(output_material_bodys.Count < idx + 1)
                            {
                                output_material_bodys.Add(new List<op_stct2>());
                            }

                            output_material_bodys[idx].Add(op);
                        }
                    }


                    return (new material()
                    {
                        //material_op = output_material_bodys.Select(x => add_NULL_op(target.Length, x)).ToList(),
                        material_op = output_material_bodys,
                        material_end = new List<List<op_stct2>>(),
                        Recipes = new List<OP_Recipe2>() { output_target[0][0]},
                        error = ""
                    });
                }
            }

            foreach ((List<OP_Recipe_flag> cmplist,int idx) in need_materials.Select((x,i)=>(x,i)).ToList())
            {

                bool nextRecipe = true;

                do
                {
                    OP_Recipe_flag cmp = cmplist[recipe_index[idx]];
                    foreach (OP_stct_flag osf in cmp.materials)
                    {
                        if (osf.flag == false && nextRecipe == true)
                        {
                            int material_slot = 0;
                            nextRecipe = true;

                            bool loopa = true;
                            while (loopa == true)
                            {
                                if (output_material_bodys.Count > material_slot == false)
                                {
                                    output_material_bodys.Add(new List<op_stct2>());
                                    output_material_index.Add(new List<int>());
                                }

                                var isExist_OP = output_material_bodys[material_slot].Where(x => (x.op_name == osf.op_name)).ToList();
                                int slot_count = output_material_bodys[material_slot].Count();


                                if (isExist_OP.Count == 0)
                                {
                                    //output_material_bodysの一番最初は素体なので、OPスロ数注意
                                    if (material_slot == 0)
                                    {
                                        if (slot_count <= target.Length)
                                        {
                                            //output_material_bodys[material_slot].Add(OP_Datas.options[osf.op_name]);
                                            output_material_bodys[material_slot].Add(OPDataContainer.GetOP_Stct(osf.op_name));
                                            output_material_index[material_slot].Add(idx);

                                            List<op_stct2> dupOP = checkOP(output_material_bodys[material_slot]);


                                            if (dupOP.Count > 0)
                                            {
                                                output_material_bodys[material_slot].RemoveAt(output_material_bodys[material_slot].Count - 1);
                                                output_material_index[material_slot].RemoveAt(output_material_index[material_slot].Count - 1);
                                            }
                                            else
                                            {
                                                loopa = false;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        output_material_bodys[material_slot].Add(OPDataContainer.GetOP_Stct(osf.op_name));
                                        output_material_index[material_slot].Add(idx);
                                        List<op_stct2> dupOP = checkOP(output_material_bodys[material_slot]);

                                        if (dupOP.Count > 0)
                                        {
                                            output_material_bodys[material_slot].RemoveAt(output_material_bodys[material_slot].Count - 1);
                                            output_material_index[material_slot].RemoveAt(output_material_index[material_slot].Count - 1);
                                        }
                                        else
                                        {
                                            loopa = false;
                                        }
                                    }
                                }

                                if (loopa == true)
                                {
                                    material_slot++;

                                    //8スロ以上になったら
                                    if (material_slot > 5)
                                    {
                                        loopa = false;
                                        nextRecipe = false;

                                        List<OP_Recipe2> output_recipe = new List<OP_Recipe2>();
                                        output_target.ForEach(x => output_recipe.Add(x[0]));

                                        recipe_index[idx]++;

                                        if (need_materials[idx].Count < recipe_index[idx] + 1)
                                        {
                                            //作成不可
                                            return new material()
                                            {
                                                material_op = new List<List<op_stct2>>(),
                                                material_end = new List<List<op_stct2>>(),
                                                Recipes = output_recipe,
                                                error = "NOT_RECIPE_MARGE",
                                            };
                                        }

                                        //やり直し
                                        foreach ((List<int> istI, int i) in output_material_index.Select((x, i) => (x, i))){
                                            foreach ((int lstJ, int j) in istI.Select((x, j) => (x, j)))
                                            {
                                                if (lstJ == idx)
                                                {
                                                    output_material_bodys[i].RemoveAt(j);
                                                }
                                            }
                                        }

                                        foreach ((List<int> istI, int i) in output_material_index.Select((x, i) => (x, i)))
                                        {
                                            istI.RemoveAt(idx);
                                        }



                                        //op付不可なのでとりあえず空き配列でも返す
                                        //レシピ変更して再試行したい
                                        /*
                                        return new material()
                                        {
                                            material_op = new List<List<op_stct2>>(),
                                            material_end = new List<List<op_stct2>>(),
                                            Recipes = output_recipe,
                                            error = "NOT_RECIPE_MARGE",
                                        };
                                        */
                                    }
                                }
                                else
                                {
                                    //素材のOP数がほしいOP数と同数以上じゃないと同じOPにはならない
                                    if (target.Length <= output_material_bodys[material_slot].Count)
                                    {
                                        //ほしいOP構成と同じOP構成の素材ができたらmaterial_slot+1
                                        List<bool> isExist = new List<bool>();

                                        foreach (var t in target)
                                        {
                                            isExist.Add(false);
                                        }

                                        foreach ((var t, int i) in target.Select((val, i) => (val, i)))
                                        {
                                            foreach (var x in output_material_bodys[material_slot])
                                            {
                                                if (t.op_name == x.op_name)
                                                {
                                                    isExist[i] = true;
                                                }
                                            }
                                        }

                                        //ほしいOP構成と同じOP構成の素材ができている場合
                                        if (isExist.All(x => (x == true)) == true)
                                        {
                                            output_material_bodys[material_slot].RemoveAt(output_material_bodys[material_slot].Count - 1);
                                            output_material_index[material_slot].RemoveAt(output_material_index[material_slot].Count - 1);
                                            material_slot++;
                                            loopa = true;
                                        }
                                    }


                                    //素材の素材を作るのに必要な素材数を調べる
                                    if (output_material_bodys.Count() > material_slot)
                                    {
                                        List<op_stct2> all_matel = new List<op_stct2>();
                                        foreach (op_stct2 o in output_material_bodys[material_slot])
                                        {
                                            //(int p, List<op_stct2> res_op) = GetMaterials(o);
                                            OP_Recipe2 resipes = GetMaterials(o)[0];
                                            foreach (op_stct2 reo in resipes.materials)
                                            {
                                                all_matel.Add(reo);
                                            }
                                        }

                                        List<op_stct2> next_dupOP = checkOP(all_matel);
                                        List<int> next_dupSer = seriseCount(next_dupOP);

                                        if (next_dupSer.Count != 0 && next_dupSer.Max() > 6)
                                        {
                                            output_material_bodys[material_slot].RemoveAt(output_material_bodys[material_slot].Count - 1);
                                            output_material_index[material_slot].RemoveAt(output_material_index[material_slot].Count - 1);
                                            material_slot++;
                                            loopa = true;
                                        }
                                    }

                                }
                            }

                        }
                    }
                }while (nextRecipe == false);
            }

            //特殊能力追加をすべて同じにする
            List<OP_Recipe2> selectedRecipes = new List<OP_Recipe2>();
            foreach((List<OP_Recipe2> recipe,int i) in output_target.Select((x, i) => (x, i))){
                selectedRecipes.Add(recipe[recipe_index[i]]);
            }


            int targetPercent = selectedRecipes.Max(x => x.AddPercent);
            List<OP_Recipe2> newOPRecipes = new List<OP_Recipe2>();
            selectedRecipes.ForEach(x => newOPRecipes.Add(new OP_Recipe2()
            {
                materials = x.materials,
                percent = x.percent,
                name = x.name,
                AddPercent = targetPercent
            }));



            return new material()
            {
                //material_op = output_material_bodys.Select(x => add_NULL_op(target.Length, x)).ToList(),
                material_op = output_material_bodys,
                Recipes = newOPRecipes,
                material_end = new List<List<op_stct2>>(),
                error = ""
            };
        }


        struct OP_Recipe_flag
        {
            public op_stct2 name;  //完成品
            public List<OP_stct_flag> materials;   //必要な素材
            public int percent;    //成功確率

            public OP_Recipe_flag(OP_Recipe2 o)
            {
                name = o.name;
                percent = o.percent;
                materials = o.materials.Select(x => new OP_stct_flag(x)).ToList();
            }
        }

        struct OP_stct_flag
        {
            public string jp_name;
            public string op_name;
            public string series;
            public bool flag;

            public OP_stct_flag(op_stct2 n)
            {
                jp_name = n.jp_name;
                op_name = n.op_name;
                series = n.series;
                flag = false;
            }
        }


        static public material SerchOP(op_stct2[] target, int percent_plus = 0, int camp_percent = 0)
        {

            material output_mat = SerchOP_materialBodys(target, percent_plus,camp_percent);
            //エラー時
            if(output_mat.error != "")
            {
                return output_mat;
            }


            List<string> target_names = target.Select(x=>x.op_name).ToList();
            List<string> flatten_mat = new List<string>();
            int max_slot = 1;
            
            //次の再帰に入るか判定
            foreach(List<op_stct2> x in output_mat.material_op)
            {
                foreach(op_stct2 y in x)
                {
                    flatten_mat.Add(y.op_name);
                }

                if(max_slot < x.Count())
                {
                    max_slot = x.Count();
                }
            }

            //目的のOPと素材のOPの差集合
            List<string> exist = tools.SubList(target_names, flatten_mat);


            if ((max_slot == 1 && exist.Count() == 0) == false)
            {
                foreach (List<op_stct2> o in output_mat.material_op)
                {
                    if(o.Count == 1)
                    {
                        int parcent = 0;
                        OP_Recipe2 mat = GetMaterials(o[0],parcent,camp_percent)[0];

                        List<string> subOP = tools.SubList(o.Select(x => x.op_name).ToList(), mat.materials.Select(x => x.op_name).ToList());

                        if(subOP.Count == 0)
                        {
                            output_mat.material_end.Add(o);
                            continue;
                        }
                    }

                    material m = SerchOP(o.ToArray(), percent_plus,camp_percent);
                    output_mat.material_childs.Add(m);
                }
            }

            return output_mat;
        }



    }
}
