﻿using System.Collections.Generic;
using MigrationTools._EngineV1.Configuration;
using MigrationTools.Enrichers;
using MigrationTools.Processors;

namespace MigrationTools.Processors
{
    public class FixGitCommitLinksProcessorOptions : ProcessorOptions
    {
        public string TargetRepository { get; set; }
        public string Query { get; set; }


    }
}