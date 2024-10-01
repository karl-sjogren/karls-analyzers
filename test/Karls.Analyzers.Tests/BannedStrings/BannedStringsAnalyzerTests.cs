using System.Text;
using Karls.Analyzers.BannedStrings;
using Roslynator.Testing.CSharp;
using Roslynator.Testing.CSharp.Xunit;

using VerifyCS = Karls.Analyzers.Tests.RoslynUtils.CSharpAnalyzerVerifier<Karls.Analyzers.BannedStrings.BannedStringsAnalyzer>;
using VerifyFixCS = Karls.Analyzers.Tests.RoslynUtils.CSharpCodeFixVerifier<Karls.Analyzers.BannedStrings.BannedStringsAnalyzer, Karls.Analyzers.BannedStrings.BannedStringsCodeFixProvider>;

namespace Karls.Analyzers.Tests.BannedStrings;

public class BannedStringsTests : XunitDiagnosticVerifier<BannedStringsAnalyzer, BannedStringsCodeFixProvider> {
    public override CSharpTestOptions Options => CSharpTestOptions.Default
        .WithParseOptions(CSharpTestOptions.Default.ParseOptions.WithLanguageVersion(LanguageVersion.CSharp10))
        .WithAllowedCompilerDiagnosticIds(["CS0414"]);

    public override DiagnosticDescriptor Descriptor { get; } = DiagnosticRules.UsePredefinedConstInsteadOfString;

    [Fact]
    public async Task ShouldReportStringInMethodBodyAsync() {
        var test = new VerifyCS.Test {
            TestState =
            {
                Sources = { """
public class MyClass {
    public void MyMethod() {
        var myString = "DoNotUseMe";
    }
}

""" },
                AdditionalFiles = { ("BannedStrings.txt", "DoNotUseMe;Constants.UseThisInstead") }
            },
            ExpectedDiagnostics =
            {
                VerifyCS.Diagnostic().WithSpan(3, 24, 3, 36).WithArguments("\"DoNotUseMe\"", "Constants.UseThisInstead")
            }
        };

        await test.RunAsync();
    }

    [Fact]
    public async Task ShouldReportStringInPrivateFieldAsync() {
        var test = new VerifyCS.Test {
            TestState =
            {
                Sources = { """
public class MyClass {
    private string _myString = [|"DoNotUseMe"|];
}

""" },
                AdditionalFiles = { ("BannedStrings.txt", "DoNotUseMe;Constants.UseThisInstead") }
            }
        };

        await test.RunAsync();
    }

    [Fact]
    public async Task ShouldReportStringInPublicGetterAsync() {
        var test = new VerifyCS.Test {
            TestState =
            {
                Sources = { """
public class MyClass {
    public string _myString => [|"DoNotUseMe"|];
}

""" },
                AdditionalFiles = { ("BannedStrings.txt", "DoNotUseMe;Constants.UseThisInstead") }
            }
        };

        await test.RunAsync();
    }

    [Fact]
    public async Task ShouldNotReportConstantStringAsync() {
        var test = new VerifyCS.Test {
            TestState =
            {
                Sources = { """
public class Constants {
    public const string UseThisInstead = "DoNotUseMe";
}

""" },
                AdditionalFiles = { ("BannedStrings.txt", "DoNotUseMe;Constants.UseThisInstead") }
            }
        };

        await test.RunAsync();
    }

    [Fact]
    public async Task ShouldNotCrashWhenNoAdditionalFileIsPresentAsync() {
        var test = new VerifyCS.Test {
            TestState =
            {
                Sources = { """
public class Constants {
    public const string UseThisInstead = "DoNotUseMe";
}

""" },
            }
        };

        await test.RunAsync();
    }

    [Fact]
    public async Task ShouldFixStringInPrivateFieldAsync() {
        var test = new VerifyFixCS.Test {
            TestState =
            {
                Sources = { """
public class Constants {
    public const string UseThisInstead = "UseThisInstead";
}

public class MyClass {
    private string _myString = [|"DoNotUseMe"|];
}

""" },
                AdditionalFiles = { ("BannedStrings.txt", "DoNotUseMe;Constants.UseThisInstead") }
            },
            FixedCode = """
public class Constants {
    public const string UseThisInstead = "UseThisInstead";
}

public class MyClass {
    private string _myString = Constants.UseThisInstead;
}

"""
        };

        await test.RunAsync();
    }

    [Fact]
    public async Task ShouldLoadBannedStringsFromParentDirectoryIfNeededAsync() {
        var test = new VerifyCS.Test {
            TestState =
            {
                Sources = { """
public class MyClass {
    public void MyMethod() {
        var myString = "DoNotUseMe";
    }
}

""" },
                AdditionalFiles = { ("../BannedStrings.txt", "DoNotUseMe;Constants.UseThisInstead") }
            },
            ExpectedDiagnostics =
            {
                VerifyCS.Diagnostic().WithSpan(3, 24, 3, 36).WithArguments("\"DoNotUseMe\"", "Constants.UseThisInstead")
            }
        };

        await test.RunAsync();
    }

    [Fact]
    public async Task ShouldFindMultipleInstancesOfBannedStringsAsync() {
        var sb = new StringBuilder();
        sb.AppendLine("DoNotUseMe;Constants.UseThisInstead");
        sb.AppendLine("Zebra;Constants.Animals.Zebra");
        sb.AppendLine("Lion;Constants.Animals.Lion");
        sb.AppendLine("Tiger;Constants.Animals.Tiger");
        sb.AppendLine("Bear;Constants.Animals.Bear");
        sb.AppendLine("Elephant;Constants.Animals.Elephant");
        sb.AppendLine("Fish;Constants.Animals.Fish");
        sb.AppendLine("Liger;Constants.Animals.Liger");

        var test = new VerifyCS.Test {
            TestState =
            {
                Sources = { """
using System.Collections.Generic;
using System.Linq;

public class MyClass {
    public string _myString => [|"DoNotUseMe"|];
    public string Zebra => [|"Zebra"|];

    public string Lion { get; } = [|"Lion"|];
    public string Tiger { get { return [|"Tiger"|]; } }

    public string LiteralLiger => [|"Liger"|];
    public string TemplateLiger => [|$"Liger"|];
    public string VerbatimLiger => [|@"Liger"|];
    public string VerbatimTemplateLiger => [|@$"Liger"|];

    public string GetBear() {
        return [|"Bear"|];
    }

    public IEnumerable<string> FilterElephants => GetAnimals().Where(a => a == [|"Elephant"|]);

    public string[] GetAnimals() {
        return new [] { [|"Fish"|] };
    }
}

""" },
                AdditionalFiles = { ("BannedStrings.txt", sb.ToString()) }
            }
        };

        await test.RunAsync();
    }
}
