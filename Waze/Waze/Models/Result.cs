using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Waze.Models
{
    internal class Result
    {
        public Path Path { get; set; }
        public int Street { get; set; }
        public object AltStreets { get; set; }
        public int Distance { get; set; }
        public int Length { get; set; }
        public int CrossTime { get; set; }
        public int CrossTimeWithoutRealTime { get; set; }
        public object Tiles { get; set; }
        public object ClientIds { get; set; }
        public Instruction Instruction { get; set; }
        public bool KnownDirection { get; set; }
        public int Penalty { get; set; }
        public int RoadType { get; set; }
        public object AdditionalInstruction { get; set; }
        public bool IsToll { get; set; }
        public object NaiveRoute { get; set; }
        public int DetourSavings { get; set; }
        public int DetourSavingsNoRT { get; set; }
        public bool UseHovLane { get; set; }
    }
}
