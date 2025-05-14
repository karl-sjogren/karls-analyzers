using VerifyCS = Karls.Analyzers.Tests.RoslynUtils.CSharpAnalyzerVerifier<Karls.Analyzers.EmojiStrings.EmojiStringAnalyzer>;
using VerifyFixCS = Karls.Analyzers.Tests.RoslynUtils.CSharpCodeFixVerifier<Karls.Analyzers.EmojiStrings.EmojiStringAnalyzer, Karls.Analyzers.EmojiStrings.EmojiStringCodeFixProvider>;

public class EmojiAnalyzerTests {
    [Fact]
    public async Task Flags_Raw_String_LiteralAsync() {
        var test = @"
class C {
    void M() {
        var s = [|""abc123""|];
    }
}";
        await VerifyCS.VerifyAnalyzerAsync(test);
    }

    [Fact]
    public async Task CodeFix_Replaces_With_EmojiDecoderAsync() {
        var test = @"
class C {
    void M() {
        var s = [|""abc123""|];
    }
}";

        var fixedCode = @"
class C {
    void M() {
        var s = EmojiDecoder.Decode(""🛴🛹🛺..."");
    }
}";

        // You may want to mock `EmojiObfuscator.EncodeToEmoji` to return deterministic output.
        await VerifyFixCS.VerifyCodeFixAsync(test, fixedCode);
    }
}
