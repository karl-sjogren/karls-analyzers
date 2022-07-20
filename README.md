# Karls Analyzers ![CI workflow](https://github.com/karl-sjogren/karls-analyzers/actions/workflows/dotnet.yml/badge.svg)

## Installing

The analyzers are currently only available via the CI builds on Github. They will be published to the NuGet Gallery
when they are ready for public consumption.

## Analyzers

### Optimizely

| Id | Value | Title |
| ------------------------------------ | -------- | ------------------------------------------------------------------------------------------- |
| [KO0001](docs/analyzers/KO0001.md)   | KO0001   | Properties on content types should have matching sort and source order.                     |
| [KO0002](docs/analyzers/KO0002.md)   | KO0002   | Content types need to have unique identifiers.                                              |

### Be polite

| Id | Value | Title |
| ------------------------------------ | -------- | ------------------------------------------------------------------------------------------- |
| [KP0001](docs/analyzers/KP0001.md)   | KP0001   | Identifiers and comments should not contain impolite or degrading words or sentences.       |

## Attributions

Based on [Roslynator](https://github.com/JosefPihrt/Roslynator) which made this quite
easy to get going. If you want to write you own analyzers, check it out!

"Be Polite" analyzers are inspired by [InclusivenessAnalyzer](https://github.com/merill/InclusivenessAnalyzer).
