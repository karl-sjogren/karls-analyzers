# Karls Optimizely Analyzers ![CI workflow](https://github.com/karl-sjogren/optimizely-analyzers/actions/workflows/dotnet.yml/badge.svg)

## Analyzers

### SO001

Properties on content types should have matching sort and source order. Will only analyze
classes with an attribute named "ContentType".

Will emit a diagnostic for the following since the properties are not ordered in the source
as they are with the `DisplayAttribute`

```cs
[ContentType]
public class Block {
    [Display(Order = 2)]
    public virtual string Prop2 { get; set; }

    [Display(Order = 1)]
    public virtual string Prop1 { get; set; }
}
```

## Attributions

Based on [Roslynator](https://github.com/JosefPihrt/Roslynator) which made this easy enough
to be worth the effort.
