using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;

namespace PSO2_OptionalAbility_Creator
{
    public class OPData_Memory : MemorySQL_Load, IOPContainer
    {

        public List<op_stct2> op_lists;

        public OPData_Memory(string datasource) : base(datasource, "SELECT * FROM OP_Lists ")
        {

        }

        public override void init()
        {
            op_lists = new List<op_stct2>();
        }

        public override void load(SQLiteDataReader data)
        {
            var op_name = data.GetValue(0);
            var Display_name = data.GetValue(1);
            var Series = data.GetValue(2);

            op_stct2 newst = new op_stct2()
            {
                jp_name = Display_name as string,
                op_name = op_name as string,
                series = Series as string
            };
            op_lists.Add(newst);
        }

        public op_stct2 GetOP_Stct(string op_name)
        {
            List<op_stct2> res = op_lists.Where(x => (x.op_name == op_name)).ToList();

            //多分1つしかないはず
            if(res.Count == 0)
            {
                //一番最初はゴミ
                return op_lists[0];
            }

            return res[0];
        }

        public List<op_stct2> GetAllOPStct()
        {
            return op_lists;
        }

    }
}
