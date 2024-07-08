using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntDesign.Extensions;
public class IdUtil
{
    public static string GenId()
    {
        return $"{Constants.ClsPrefix}-{Guid.NewGuid().ToString()}";
    }
}
