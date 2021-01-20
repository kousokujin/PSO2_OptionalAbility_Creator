using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSO2_OptionalAbility_Creator
{
    public class material
    {
        public List<List<op_stct2>> material_op; //素材op
        public List<List<op_stct2>> material_end; //マイショップで買うOP
        public List<material> material_childs;
        public List<OP_Recipe2> Recipes;

        public material()
        {
            material_op = new List<List<op_stct2>>();
            material_childs = new List<material>();
            Recipes = new List<OP_Recipe2>();
        }
    }

    public class material_count : material
    {
        public new List<List<op_stct2>> material_op
        {
            get
            {
                return convertStct(material_op_count);
            }
            set
            {
                material_op_count = conveetStctList(value);
            }
        }

        public new List<List<op_stct2>> material_end
        {
            get
            {
                return convertStct(material_end_count);
            }
            set
            {
                material_end_count = conveetStctList(value);
            }
        }

        public List<material_count> materials_childs_count;
        public int count;
        public List<op_stct_count> material_op_count;
        public List<op_stct_count> material_end_count;
        //public List<OP_Recipe2> Recipes;


        public material_count(material m,int count)
        {
            this.material_op_count = conveetStctList(m.material_op);
            this.material_end_count = conveetStctList(m.material_end);
            this.Recipes = m.Recipes;
            this.materials_childs_count = new List<material_count>();
            this.count = count;

            (List<material> margeMaterial,List<int> chldcount) = MargeChildMaterial(m);

            foreach((material x,int i) in margeMaterial.Select((x,i)=>(x,i)))
            {
                var chd = new material_count(x, chldcount[i]);
                materials_childs_count.Add(chd);
            }
        }

        public List<op_stct_count> conveetStctList(List<List<op_stct2>> oplists)
        {
            List<op_stct_count> outputList = new List<op_stct_count>(); ;

            foreach (List<op_stct2> o in oplists)
            {
                bool isDupOP = false;
                //int index = 0;

                foreach ((op_stct_count ops,int i) in outputList.Select((ox,idx) => (ox,idx)))
                {
                    List<string> dupStr = tools.SubList(o.Select(x => x.op_name).ToList(), ops.name.Select(x => x.op_name).ToList());

                    if (dupStr.Count == 0)
                    {
                        isDupOP = true;
                        //index = i;
                        ops.count++;
                    }
                }

                if (isDupOP == false)
                {
                    outputList.Add(new op_stct_count() { count = 1, name = o });
                }
            }

            return outputList;
        }

        public (List<material>,List<int>) MargeChildMaterial(material m)
        {
            List<material> marged = new List<material>();
            List<int> marged_cnt = new List<int>();

            foreach(material md in m.material_childs)
            {
                bool isDup = false;
                foreach ((material mrg,int i) in marged.Select((x,i)=>(x,i)))
                {
                    List<string> child_res_str = md.Recipes.Select(x => x.name.op_name).ToList();
                    List<string> mrg_str = mrg.Recipes.Select(x => x.name.op_name).ToList();
                    List<string> dupRecipe = tools.SubList(child_res_str, mrg_str);

                    if(dupRecipe.Count == 0)
                    {
                        isDup = true;
                        marged_cnt[i]++;
                    }
                }

                if(isDup == false)
                {
                    marged.Add(md);
                    marged_cnt.Add(1);
                }
            }

            return (marged,marged_cnt);
        }

        public List<List<op_stct2>> convertStct(List<op_stct_count> inList){
            List<List<op_stct2>> output = new List<List<op_stct2>>();

            foreach (op_stct_count osc in material_op_count)
            {
                for (int i = 0; i < osc.count; i++)
                {
                    output.Add(osc.name);
                }
            }

            return output;
        }
    }

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

    public struct OP_Recipe2
    {
        public op_stct2 name;  //完成品
        public List<op_stct2> materials;   //必要な素材
        public int percent;    //成功確率
    }

    public class op_stct_count
    {
        public List<op_stct2> name;
        public int count;
    }
}
