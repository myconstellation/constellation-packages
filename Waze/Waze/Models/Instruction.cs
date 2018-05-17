using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Waze.Models
{
    internal class Instruction
    {
        public string Opcode { get; set; }
        public int Arg { get; set; }
        public object InstructionText { get; set; }
        public object Name { get; set; }
        public object Tts { get; set; }
    }
}
