---
optionsClassName: FixGitCommitLinksProcessorOptions
optionsClassFullName: MigrationTools.Processors.FixGitCommitLinksProcessorOptions
configurationSamples:
- name: defaults
  description: 
  code: >-
    {
      "MigrationTools": {
        "ProcessorDefaults": {
          "FixGitCommitLinksProcessor": []
        }
      }
    }
  sampleFor: MigrationTools.Processors.FixGitCommitLinksProcessorOptions
- name: sample
  description: 
  code: >-
    {
      "MigrationTools": {
        "ProcessorDefaults": {
          "FixGitCommitLinksProcessor": []
        }
      }
    }
  sampleFor: MigrationTools.Processors.FixGitCommitLinksProcessorOptions
- name: classic
  description: 
  code: >-
    {
      "$type": "FixGitCommitLinksProcessorOptions",
      "Enabled": false,
      "TargetRepository": null,
      "Query": null,
      "Enrichers": null,
      "ProcessorEnrichers": null,
      "SourceName": null,
      "TargetName": null,
      "RefName": null
    }
  sampleFor: MigrationTools.Processors.FixGitCommitLinksProcessorOptions
description: missng XML code comments
className: FixGitCommitLinksProcessor
typeName: Processors
architecture: 
options:
- parameterName: Enabled
  type: Boolean
  description: If set to `true` then the processor will run. Set to `false` and the processor will not run.
  defaultValue: missng XML code comments
- parameterName: Enrichers
  type: List
  description: A list of enrichers that can augment the proccessing of the data
  defaultValue: missng XML code comments
- parameterName: ProcessorEnrichers
  type: List
  description: List of Enrichers that can be used to add more features to this processor. Only works with Native Processors and not legacy Processors.
  defaultValue: missng XML code comments
- parameterName: Query
  type: String
  description: missng XML code comments
  defaultValue: missng XML code comments
- parameterName: RefName
  type: String
  description: '`Refname` will be used in the future to allow for using named Options without the need to copy all of the options.'
  defaultValue: missng XML code comments
- parameterName: SourceName
  type: String
  description: missng XML code comments
  defaultValue: missng XML code comments
- parameterName: TargetName
  type: String
  description: missng XML code comments
  defaultValue: missng XML code comments
- parameterName: TargetRepository
  type: String
  description: missng XML code comments
  defaultValue: missng XML code comments
status: missng XML code comments
processingTarget: missng XML code comments
classFile: /src/MigrationTools.Clients.AzureDevops.ObjectModel/Processors/FixGitCommitLinksProcessor.cs
optionsClassFile: /src/MigrationTools.Clients.AzureDevops.ObjectModel/Processors/FixGitCommitLinksProcessorOptions.cs

redirectFrom:
- /Reference/Processors/FixGitCommitLinksProcessorOptions/
layout: reference
toc: true
permalink: /Reference/Processors/FixGitCommitLinksProcessor/
title: FixGitCommitLinksProcessor
categories:
- Processors
- 
topics:
- topic: notes
  path: /docs/Reference/Processors/FixGitCommitLinksProcessor-notes.md
  exists: false
  markdown: ''
- topic: introduction
  path: /docs/Reference/Processors/FixGitCommitLinksProcessor-introduction.md
  exists: false
  markdown: ''

---