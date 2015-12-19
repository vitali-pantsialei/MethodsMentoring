using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Visitor
{
    public class IterationControlArgs
    {
        public FileSystemInfo CurrentFile { get; set; }
        public bool TerminateSearch { get; set; }
        public bool Exclude { get; set; }
    }
}
