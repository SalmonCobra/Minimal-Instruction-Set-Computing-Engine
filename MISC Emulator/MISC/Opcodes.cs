using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISC
{
    internal class Opcodes : MISC_Engine
    {
        
        public static void Nop()
        {
            registers[(int)Register.PC] += 1; // increase the Program Counter (PC)
        }

        public static void Mov()
        {
            ReadRam();
            if (ReadReg(regSrc) == (long)Register.None) { WriteReg(regDest, ReadReg(regSrc)); }
            else { WriteReg(regDest, Convert.ToInt64(data));  }
            ReadRam(true);
        }
    }
}
