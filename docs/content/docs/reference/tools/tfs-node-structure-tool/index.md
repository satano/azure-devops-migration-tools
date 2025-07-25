---
title: Tfs Node Structure Tool
description: Tool for creating missing area and iteration path nodes in the target project during migration. Configurable through TfsNodeStructureToolOptions to specify which node types to create.
dataFile: reference.tools.tfsnodestructuretool.yaml
slug: tfs-node-structure-tool
aliases:
  - /docs/Reference/Tools/TfsNodeStructureTool
  - /Reference/Tools/TfsNodeStructureTool
  - /learn/azure-devops-migration-tools/Reference/Tools/TfsNodeStructureTool
  - /learn/azure-devops-migration-tools/Reference/Tools/TfsNodeStructureTool/index.md
date: 2025-06-24T12:07:31Z
discussionId: 2807
---

{{< class-description >}}

## Options

{{< class-options >}}

## Samples

### Sample

{{< class-sample sample="sample" >}}

### Defaults

{{< class-sample sample="defaults" >}}

### Classic

{{< class-sample sample="classic" >}}

## Metadata

{{< class-metadata >}}

## Schema

{{< class-schema >}}

## Iteration Maps and Area Maps

**NOTE: It is NOT posible to migrate a work item if the Area or Iteration path does not exist on the target project. This is because the work item will be created with the same Area and Iteration path as the source work item. If the path does not exist, the work item will not be created. _There is not way around this!_**

You have two options to solve this problem:

1. You can manually create the mentioned work items. This is a good option if you have a small number of work items or a small number of missing nodes. This will not work if you have work items that were moved from one project to another. Those Nodes are impossible to create in the target project.
2. You can use the `Areas.Mappings` and `Iterations.Mappings` to remap the nodes to existing nodes in the target project. This is a good option if you have a large number of work items or a large number of missing nodes.

### Overview

These two configuration elements apply after the `Areas.Filters` and `Iterations.Filters` selectors, i.e.
only on Areas and Iterations that have been selected for migration. They allow
to change the area path, respectively the iteration path, of migrated work items.

These remapping rules are applied both while creating path nodes in the target
project and when migrating work items.

These remapping rules are applied with a higher priority than the
`PrefixProjectToNodes` option. This means that if no declared rule matches the
path and the `PrefixProjectToNodes` option is enabled, then the old behavior is
used.

The syntax uses structured mappings with Match and Replacement properties containing regular expressions and the replacement text.

_Warning_: These follow the
[.net regular expression language](https://docs.microsoft.com/en-us/dotnet/standard/base-types/regular-expression-language-quick-reference).
The Match property contains a regular expression search pattern, while the
Replacement property contains a regular expression replacement pattern. It is therefore possible to
use back-references in the replacement string.

_Warning_: Special characters in the acceptation of regular expressions _and_
json both need to be escaped. For the Match property, this means, for example, that a
literal backslash must be escaped for the regular expression language `\\`
_and_ each of these backslashes must then be escaped for the json encoding:
`\\\\`. In the Replacement property, a literal `$` must be escaped with an
additional `$` if it is followed by a number (due to the special meaning in
regular expression replacement strings), while a backslash must be escaped
(`\\`) due to the special meaning in json.

_Advice_: To avoid unexpected results, always match terminating backslashes in
the search pattern and replacement string: if a search pattern ends with a
backslash, you should also put one in the replacement string, and if the search
pattern does not include a terminating backslash, then none should be included
in the replacement string.

### Configuration

```json
"Iterations": {
  "Mappings": [
    {
      "Match": "^OriginalProject\\\\Path1(?=\\\\Sprint 2022)(.*)$",
      "Replacement": "TargetProject\\AnotherPath\\NewTeam$1"
    },
    {
      "Match": "^OriginalProject\\\\Path1(?=\\\\Sprint 2020)(.*)$",
      "Replacement": "TargetProject\\AnotherPath\\Archives\\Sprints 2020$1"
    },
    {
      "Match": "^OriginalProject\\\\Path2(.*)$",
      "Replacement": "TargetProject\\YetAnotherPath\\Path2$1"
    }
  ]
},
"Areas": {
  "Mappings": [
    {
      "Match": "^OriginalProject\\\\(DescopeThis|DescopeThat)(.*)$",
      "Replacement": "TargetProject\\Archive\\Descoped\\$1$2"
    },
    {
      "Match": "^OriginalProject\\\\(?!DescopeThis|DescopeThat)(.*)$",
      "Replacement": "TargetProject\\NewArea\\$1"
    }
  ]
}
```

- `"Match": "^OriginalProject\\\\Path1(?=\\\\Sprint 2022)(.*)$", "Replacement": "TargetProject\\AnotherPath\\NewTeam$1"`

  In an iteration path, `OriginalProject\Path1` found at the beginning of the
  path, when followed by `\Sprint 2022`, will be replaced by
  `TargetProject\AnotherPath\NewTeam`.

  `OriginalProject\Path1\Sprint 2022\Sprint 01` will become
  `TargetProject\AnotherPath\NewTeam\Sprint 2022\Sprint 01` but
  `OriginalProject\Path1\Sprint 2020\Sprint 03` will _not_ be transformed by
  this rule.

- `"Match": "^OriginalProject\\\\Path1(?=\\\\Sprint 2020)(.*)$", "Replacement": "TargetProject\\AnotherPath\\Archives\\Sprints 2020$1"`

  In an iteration path, `OriginalProject\Path1` found at the beginning of the
  path, when followed by `\Sprint 2020`, will be replaced by
  `TargetProject\AnotherPath\Archives\\Sprints 2020`.

  `OriginalProject\Path1\Sprint 2020\Sprint 01` will become
  `TargetProject\AnotherPath\Archives\Sprint 2020\Sprint 01` but
  `OriginalProject\Path1\Sprint 2021\Sprint 03` will _not_ be transformed by
  this rule.

- `"Match": "^OriginalProject\\\\Path2(.*)$", "Replacement": "TargetProject\\YetAnotherPath\\Path2$1"`

  In an iteration path, `OriginalProject\Path2` will be replaced by
  `TargetProject\YetAnotherPath\Path2`.

- `"Match": "^OriginalProject\\\\(DescopeThis|DescopeThat)(.*)$", "Replacement": "TargetProject\\Archive\\Descoped\\$1$2"`

  In an area path, `OriginalProject\` found at the beginning of the path, when
  followed by either `DescopeThis` or `DescopeThat` will be replaced by `TargetProject\Archive\Descoped\`.

  `OriginalProject\DescopeThis\Area` will be transformed to
  `TargetProject\Archive\Descoped\DescopeThis\Area`.
  `OriginalProject\DescopeThat\Product` will be transformed to
  `TargetProject\Archive\Descoped\DescopeThat\Product`.

- `"Match": "^OriginalProject\\\\(?!DescopeThis|DescopeThat)(.*)$", "Replacement": "TargetProject\\NewArea\\$1"`

  In an area path, `OriginalProject\` found at the beginning of the path will be
  replaced by `TargetProject\NewArea\` unless it is followed by `DescopeThis` or
  `DescopeThat`.

  `OriginalProject\ValidArea\` would be replaced by
  `TargetProject\NewArea\ValidArea\` but `OriginalProject\DescopeThis` would not
  be modified by this rule.

### PrefixProjectToNodes

The `PrefixProjectToNodes` was an option that was used to prepend the source project name to the target set of nodes. This was super valuable when the target Project already has nodes and you dont want to merge them all together. This is now replaced by the `Areas.Mappings` and `Iterations.Mappings` options.

```
"Iterations": {
  "Mappings": [
    {
      "Match": "^SourceServer\\\\(.*)$",
      "Replacement": "TargetServer\\SourceServer\\$1"
    }
  ]
},
"Areas": {
  "Mappings": [
    {
      "Match": "^SourceServer\\\\(.*)$",
      "Replacement": "TargetServer\\SourceServer\\$1"
    }
  ]
}
```

=======

### More Complex Regex

Before your migration starts it will validate that all of the Areas and Iterations from the **Source** work items revisions exist on the **Target**. Any that do not exist will be flagged in the logs and if and the migration will stop just after it outputs a list of the missing nodes.

Our algorithm that converts the Source nodes to Target nodes processes the mappings at that time. This means that any valid mapped nodes will never be caught by the `This path is not anchored in the source project` message as they are already altered to be valid.

> We recently updated the logging for this part of the system to more easily debug both your mappings and to see what they system is doing with the nodes and their current state. You can set `"LogLevel": "Debug"` to see the details.

To add a mapping, you can follow the documentation with this being the simplest way:

```
"Iterations": {
  "Mappings": [
    {
      "Match": "WorkItemMovedFromProjectName\\\\Iteration 1",
      "Replacement": "TargetProject\\Sprint 1"
    }
  ]
},
"Areas": {
  "Mappings": [
    {
      "Match": "WorkItemMovedFromProjectName\\\\Team 2",
      "Replacement": "TargetProject\\ProductA\\Team 2"
    }
  ]
}
```

Or you can use regular expressions to match the missing area or iteration paths:

```
"Iterations": {
  "Mappings": [
    {
      "Match": "^OriginalProject\\\\Path1(?=\\\\Sprint 2022)(.*)$",
      "Replacement": "TargetProject\\AnotherPath\\NewTeam$1"
    },
    {
      "Match": "^OriginalProject\\\\Path1(?=\\\\Sprint 2020)(.*)$",
      "Replacement": "TargetProject\\AnotherPath\\Archives\\Sprints 2020$1"
    },
    {
      "Match": "^OriginalProject\\\\Path2(.*)$",
      "Replacement": "TargetProject\\YetAnotherPath\\Path2$1"
    }
  ]
},
"Areas": {
  "Mappings": [
    {
      "Match": "^OriginalProject\\\\(DescopeThis|DescopeThat)(.*)$",
      "Replacement": "TargetProject\\Archive\\Descoped\\$1$2"
    },
    {
      "Match": "^OriginalProject\\\\(?!DescopeThis|DescopeThat)(.*)$",
      "Replacement": "TargetProject\\NewArea\\$1"
    }
  ]
}
```

If you want to use the matches in the replacement you can use the following:

```
"Iterations": {
  "Mappings": [
    {
      "Match": "^\\\\oldproject1(?:\\\\([^\\\\]+))?\\\\([^\\\\]+)$",
      "Replacement": "TargetProject\\Q1\\$2"
    }
  ]
}
```

If the old iteration path was `\oldproject1\Custom Reporting\Sprint 13`, then this would result in a match for each Iteration node after the project node. You would then be able to reference any of the nodes using "$" and then the number of the match.

Regular expressions are much more difficult to build and debug so it is a good idea to use a [regular expression tester](https://regex101.com/) to check that you are matching the right things and to build them in ChatGTP.

_NOTE: You need `\\` to escape a `\` the pattern, and `\\` to escape a `\` in JSON. Therefore in the Match property you need 4 `\` to represent the `\\` for the pattern and only 2 `\` in the Replacement property_

![image](https://github.com/nkdAgility/azure-devops-migration-tools/assets/5205575/2cf50929-7ea9-4a71-beab-dd8ff3b5b2a8)

### Example with PrefixProjectToNodes

This will prepend a bucket to the area and iteration paths. This is useful when you want to keep the original paths but also want to be able to identify them as being from the original project.

```json
"Areas": {
  "Mappings": [
    {
      "Match": "^OriginalProject(?:\\\\([^\\\\]+))?\\\\([^\\\\]+)$",
      "Replacement": "TargetProject\\BucketForIncommingAreas\\$2"
    }
  ]
},
"Iterations": {
  "Mappings": [
    {
      "Match": "^OriginalProject(?:\\\\([^\\\\]+))?\\\\([^\\\\]+)$",
      "Replacement": "TargetProject\\BucketForIncommingInterations\\$2"
    }
  ]
}
```

### Example with Areas and Iterations

```json
{
  "$type": "TfsNodeStructureToolOptions",
  "Enabled": true,
  "Areas": {
    "Filters": [],
    "Mappings": [
      {
        "Match": "^Skypoint Cloud$",
        "Replacement": "MigrationTest5"
      }
    ]
  },
  "Iterations": {
    "Filters": [],
    "Mappings": [
      {
        "Match": "^Skypoint Cloud\\\\Sprint 1$",
        "Replacement": "MigrationTest5\\Sprint 1"
      }
    ]
  },
  "ShouldCreateMissingRevisionPaths": true,
  "ReplicateAllExistingNodes": true
}
```

## <a name="Filters"></a>Filters

The `Areas.Filters` and `Iterations.Filters` entries allow the filtering of the nodes to be replicated on the target projects. To try to explain the correct usage let us assume that we have a source team project `SourceProj` with the following node structures

- AreaPath
  - SourceProj
  - SourceProj\Team 1
  - SourceProj\Team 2
  - SourceProj\Team 2\Sub-Area 1
  - SourceProj\Team 2\Sub-Area 2
  - SourceProj\Team 3
- IterationPath
  - SourceProj
  - SourceProj\Sprint 1
  - SourceProj\Sprint 2
  - SourceProj\Sprint 2\Sub-Iteration
  - SourceProj\Sprint 3

Depending upon what node structures you wish to migrate you would need the following settings. Exclusions are also possible by prefixing a path with an exclamation mark `!`. Example are

|           |                                                                                                                                                                                                                                                                                                                                                  |
| --------- | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------ |
| Intention | Migrate all areas and iterations and all Work Items                                                                                                                                                                                                                                                                                              |
| Filters   | `"Areas": { "Filters": [] }, "Iterations": { "Filters": [] }`                                                                                                                                                                                                                                                                                    |
| Comment   | The same AreaPath and Iteration Paths are created on the target as on the source. Hence, all migrated WI remain in their existing area and iteration paths. <br/> This will be affected by the `Areas.Mappings` and `Iterations.Mappings` settings.                                                                                              |
|           |
| Intention | Only migrate area path `Team 2` and it associated Work Items, but all iteration paths                                                                                                                                                                                                                                                            |
| Filters   | `"Areas": { "Filters": ["*\\Team 2"] }, "Iterations": { "Filters": ["*\\Sprint*"] }`                                                                                                                                                                                                                                                             |
| Comment   | Only the area path ending `Team 2` will be migrated. <br>The `WIQLQuery` should be edited to limit the WI migrated to this area path e.g. add `AND [System.AreaPath] UNDER 'SampleProject\\Team 2'` . <br> The migrated WI will have an area path of `TargetProj\Team 2` but retain their iteration paths matching the sprint name on the source |
|           |
| Intention | Move the `Team 2` area, including its `Sub-Area`, and any others at the same level                                                                                                                                                                                                                                                               |
| Filters   | `"Areas": { "Filters": ["*\\Team 2", "Team 2\\*"] }, "Iterations": { "Filters": [] }`                                                                                                                                                                                                                                                            |
| Comment   | The Work Items will have to be restricted to the right areas, e.g. with `AND [System.AreaPath] UNDER 'SampleProject\\Team 2' AND [System.AreaPath] NOT UNDER 'SampleProject\\Team 2\\Sub-Area'`, otherwise their migration will fail                                                                                                             |
|           |
| Intention | Move the `Team 2` area, but not its `Sub-Area`                                                                                                                                                                                                                                                                                                   |
| Filters   | `"Areas": { "Filters": ["*\\Team 2", "!Team 2\\SubArea"] }, "Iterations": { "Filters": [] }`                                                                                                                                                                                                                                                     |
| Comment   | The Work Items will have to be restricted to the right areas, e.g. with `AND [System.AreaPath] UNDER 'SampleProject\\Team 2' AND [System.AreaPath] NOT UNDER 'SampleProject\\Team 2\\Sub-Area'`, otherwise their migration will fail                                                                                                             |

# Patterns

The following patterns are supported:

> | Wildcard | Description                                                           | Example      | Matches                                                  | Does not match                        |
> | -------- | --------------------------------------------------------------------- | ------------ | -------------------------------------------------------- | ------------------------------------- |
> | \*       | matches any number of any characters including none                   | Law\*        | Law, Laws, or Lawyer                                     |
> | ?        | matches any single character                                          | ?at          | Cat, cat, Bat or bat                                     | at                                    |
> | [abc]    | matches one character given in the bracket                            | [CB]at       | Cat or Bat                                               | cat or bat                            |
> | [a-z]    | matches one character from the range given in the bracket             | Letter[0-9]  | Letter0, Letter1, Letter2 up to Letter9                  | Letters, Letter or Letter10           |
> | [!abc]   | matches one character that is not given in the bracket                | [!C]at       | Bat, bat, or cat                                         | Cat                                   |
> | [!a-z]   | matches one character that is not from the range given in the bracket | Letter[!3-5] | Letter1, Letter2, Letter6 up to Letter9 and Letterx etc. | Letter3, Letter4, Letter5 or Letterxx |

In addition, Glob also supports:

| Wildcard | Description                                                                                        | Example       | Matches                                            | Does not match |
| -------- | -------------------------------------------------------------------------------------------------- | ------------- | -------------------------------------------------- | -------------- |
| `**`     | matches any number of path / directory segments. When used must be the only contents of a segment. | /\*\*/some.\* | /foo/bar/bah/some.txt, /some.txt, or /foo/some.txt |

# Escaping special characters

Wrap special characters `?, *, [` in square brackets in order to escape them.
You can also use negation when doing this.
