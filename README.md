# Karls Analyzers ![CI workflow](https://github.com/karl-sjogren/karls-analyzers/actions/workflows/dotnet.yml/badge.svg)

## Installing

```sh
dotnet add package Karls.Analyzers
```

## Analyzers

### Optimizely

| Id | Has Code fix | Title |
| ------------------------------------ | --- | ------------------------------------------------------------------------------------------- |
| [KO0001](docs/analyzers/KO0001.md)   | Yes | Properties on content types should have matching sort and source order.                     |
| [KO0002](docs/analyzers/KO0002.md)   | Yes | Content types need to have unique identifiers.                                              |

### Be polite

| Id | Has Code fix | Title |
| ------------------------------------ | --- | ------------------------------------------------------------------------------------------- |
| [KP0001](docs/analyzers/KP0001.md)   | No  | Identifiers and comments should not contain impolite or degrading words or sentences.       |

## Attributions

Using test helpers from [Roslynator](https://github.com/JosefPihrt/Roslynator).

"Be Polite" analyzers are inspired by [InclusivenessAnalyzer](https://github.com/merill/InclusivenessAnalyzer).
