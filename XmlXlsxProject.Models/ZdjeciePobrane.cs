﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XmlXlsxProject.Models
{
    public class ZdjeciePobrane
    {
        public long Id { get; set; }
        public List<string> PhotoPathList { get; set; } = new List<string>();
    }
}
