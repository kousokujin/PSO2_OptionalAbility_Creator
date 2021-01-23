using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSO2_OptionalAbility_Creator
{
    class OP_CompositionEngine
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
        static public OP_Recipe2 GetMaterials(op_stct2 op, int percent_Plus = 0, int camp_parcent = 0)
        {
            var comArr = RecipeDataContainer.GetOP_Recipes(op,camp_parcent);
            comArr = comArr.OrderBy(x => x.percent).ToList();

            if(comArr.Count == 0)
            {
                Console.WriteLine("Recepi Not Found:{0}", op.jp_name);
                return GetMaterials(OPDataContainer.GetOP_Stct("none"));
            }

            OP_Recipe2 opc = comArr[0];
            int percent_plus_temp = 0;
            int[] percent_plus_items = new int[] { 0, 10, 20, 30, 40, 45, 50, 55, 60 };

            foreach (OP_Recipe2 o in comArr)
            {
                opc = o;
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
            }

            opc.AddPercent = percent_plus_temp;
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
        static public List<OP_Recipe2> GetMaterials(List<op_stct2> op, int percent_Plus = 0, int camp_parcent = 0)
        {
            List<OP_Recipe2> output_op = new List<OP_Recipe2>();

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

            List<OP_Recipe2> output_target = GetMaterials(target.ToList(), percent_plus, camp_parcent);
            List<OP_Recipe_flag> need_materials = output_target.Select(x => new OP_Recipe_flag(x)).ToList();


            //レシピがない場合はおわり
            bool exist_recipe_flag = output_target.All(x => (x.name.op_name != "none"));

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
                foreach (OP_Recipe2 r in output_target)
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
                    foreach (OP_Recipe2 r in output_target)
                    {
                        foreach ((op_stct2 x, int i) in r.materials.Select((x, i) => (x, i)))
                        {
                            if (output_material_bodys.Count < (i + 1))
                            {
                                output_material_bodys.Add(new List<op_stct2>());
                            }
                            output_material_bodys[i].Add(x);
                        }
                    }

                    return (new material()
                    {
                        //material_op = output_material_bodys.Select(x => add_NULL_op(target.Length, x)).ToList(),
                        material_op = output_material_bodys,
                        material_end = new List<List<op_stct2>>(),
                        Recipes = output_target,
                        error = ""
                    });
                }
            }

            foreach (OP_Recipe_flag cmp in need_materials)
            {

                foreach (OP_stct_flag osf in cmp.materials)
                {
                    if (osf.flag == false)
                    {
                        int material_slot = 0;

                        bool loopa = true;
                        while (loopa == true)
                        {
                            if (output_material_bodys.Count > material_slot == false)
                            {
                                output_material_bodys.Add(new List<op_stct2>());
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

                                        List<op_stct2> dupOP = checkOP(output_material_bodys[material_slot]);


                                        if (dupOP.Count > 0)
                                        {
                                            output_material_bodys[material_slot].RemoveAt(output_material_bodys[material_slot].Count - 1);
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
                                    List<op_stct2> dupOP = checkOP(output_material_bodys[material_slot]);

                                    if (dupOP.Count > 0)
                                    {
                                        output_material_bodys[material_slot].RemoveAt(output_material_bodys[material_slot].Count - 1);
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

                                    //op付不可なのでとりあえず空き配列でも返す
                                    //レシピ変更して再試行したい
                                    return new material() { 
                                        material_op = new List<List<op_stct2>>(),
                                        material_end = new List<List<op_stct2>>(),
                                        Recipes = output_target,
                                        error="NOT_RECIPE_MARGE",
                                    };
                                }
                            }
                            else
                            {
                                //素材のOP数がほしいOP数と同数以上じゃないと同じOPにはならない
                                if (target.Length <= output_material_bodys[material_slot].Count) {
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
                                        OP_Recipe2 resipes = GetMaterials(o);
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
                                        material_slot++;
                                        loopa = true;
                                    }
                                }

                            }

                        }

                    }
                }
            }

            //特殊能力追加をすべて同じにする
            int targetPercent = output_target.Max(x => x.AddPercent);
            List<OP_Recipe2> newOPRecipes = new List<OP_Recipe2>();
            output_target.ForEach(x => newOPRecipes.Add(new OP_Recipe2()
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
                        OP_Recipe2 mat = GetMaterials(o[0],parcent,camp_percent);

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
