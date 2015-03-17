using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XNA8DFramework
{
#if WINDOWS8
    public interface ICloneable
    {
        Object Clone();
    }
#endif
}
