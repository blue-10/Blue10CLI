﻿using CommandLine;
using System;
using System.Collections.Generic;
using System.Text;

namespace Blue10CLI
{
    class Options
    {
        [Option('v', "verbose", Required = false, HelpText = "Set output to verbose messages.")]
        public bool Verbose { get; set; }

    }
}
