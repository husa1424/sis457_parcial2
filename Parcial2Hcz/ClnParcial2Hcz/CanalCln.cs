using CadParcial2Hcz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClnParcial2Hcz
{
    public class CanalCln
    {
        public static List<Canal> listar()
        {
            using (var context = new Parcial2HczEntities())
            {
                return context.Canal
                    .Where(x => x.estadoRegistro == 1)
                    .OrderBy(x => x.nombre)
                    .ToList();
            }
        }
    }
}
