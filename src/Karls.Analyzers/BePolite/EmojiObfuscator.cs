using System.Globalization;
using System.IO.Compression;
using System.Security.Cryptography;
using System.Text;

public static class EmojiObfuscator {
    // Define emoji Unicode ranges
    private static readonly List<Tuple<int, int>> _emojiRanges = [
        new Tuple<int, int>(0x1F600, 0x1F64F), // Emoticons
        new Tuple<int, int>(0x1F680, 0x1F6FF), // Transport & Map
        new Tuple<int, int>(0x1F700, 0x1F77F), // Alchemical Symbols
        new Tuple<int, int>(0x1F780, 0x1F7FF), // Geometric Shapes
        new Tuple<int, int>(0x1F800, 0x1F8FF), // Supplemental Arrows-C
        new Tuple<int, int>(0x1F900, 0x1F9FF), // Supplemental Symbols & Pictographs
        new Tuple<int, int>(0x1FA00, 0x1FA6F), // Chess Symbols, etc.
        new Tuple<int, int>(0x1FA70, 0x1FAFF)  // Symbols for Zodiac
    ];

    // Generate the list of valid emoji code points
    private static List<int> GenerateEmojiList() {
        var emojiList = new List<int>();
        foreach(var range in _emojiRanges) {
            for(var codePoint = range.Item1; codePoint <= range.Item2; codePoint++) {
                emojiList.Add(codePoint);
            }
        }

        return emojiList;
    }

    // Generate SHA256 checksum for integrity verification
    private static byte[] ComputeChecksum(string input) {
        using(var sha256 = SHA256.Create()) {
            return sha256.ComputeHash(Encoding.UTF8.GetBytes(input));
        }
    }

    // Compress the byte array using Deflate compression
    private static byte[] CompressData(byte[] data) {
        var output = new MemoryStream();
        using(var deflateStream = new DeflateStream(output, CompressionLevel.Fastest)) {
            deflateStream.Write(data, 0, data.Length);
        }

        return output.ToArray();
    }

    // Encode the input string as emojis with compression and checksum
    public static string EncodeToEmoji(string input) {
        var sb = new StringBuilder();
        var rng = new Random();

        // Generate the list of valid emojis
        var validEmojis = GenerateEmojiList();

        // Compute checksum of the original string
        var checksum = ComputeChecksum(input);

        // First, compress the string data (to obscure the length)
        var inputBytes = Encoding.UTF8.GetBytes(input);
        var compressedData = CompressData(inputBytes);

        // Start encoding with the checksum (first 2 bytes)
        sb.Append(char.ConvertFromUtf32(validEmojis[rng.Next(validEmojis.Count)])); // Start with a random emoji
        sb.Append(char.ConvertFromUtf32(validEmojis[rng.Next(validEmojis.Count)])); // Random emoji

        // Add the checksum bytes as emojis (checksum length is fixed)
        sb.Append(char.ConvertFromUtf32(validEmojis[rng.Next(validEmojis.Count)] + checksum[0]));
        sb.Append(char.ConvertFromUtf32(validEmojis[rng.Next(validEmojis.Count)] + checksum[1]));

        // Now encode the compressed data as emojis
        foreach(var byteVal in compressedData) {
            var emojiCodePoint = validEmojis[rng.Next(validEmojis.Count)] + (byteVal % 80); // Map byte to emoji
            sb.Append(char.ConvertFromUtf32(emojiCodePoint)); // Append as emoji
        }

        return sb.ToString();
    }

    public static string DecodeFromEmoji(string emojiString) {
        var enumerator = StringInfo.GetTextElementEnumerator(emojiString);

        if(!enumerator.MoveNext()) throw new Exception("Empty emoji string");
        var offset = Char.ConvertToUtf32(enumerator.GetTextElement(), 0);

        if(!enumerator.MoveNext()) throw new Exception("Missing checksum 1");
        var checksum1 = Char.ConvertToUtf32(enumerator.GetTextElement(), 0) - offset;

        if(!enumerator.MoveNext()) throw new Exception("Missing checksum 2");
        var checksum2 = Char.ConvertToUtf32(enumerator.GetTextElement(), 0) - offset;

        var byteList = new List<byte>();

        while(enumerator.MoveNext()) {
            var codePoint = Char.ConvertToUtf32(enumerator.GetTextElement(), 0);
            byteList.Add((byte)(codePoint - offset));
        }

        var compressed = byteList.ToArray();
        var expectedChecksum = ComputeChecksum(compressed);

        if(checksum1 != expectedChecksum[0] || checksum2 != expectedChecksum[1])
            throw new Exception("❌ Tampering detected: checksum mismatch");

        var decompressed = Decompress(compressed);
        return Encoding.UTF8.GetString(decompressed);
    }

    private static byte[] Compress(byte[] data) {
        using var output = new MemoryStream();
        using(var gzip = new GZipStream(output, CompressionLevel.Optimal)) {
            gzip.Write(data, 0, data.Length);
        }

        return output.ToArray();
    }

    private static byte[] Decompress(byte[] data) {
        using var input = new MemoryStream(data);
        using var gzip = new GZipStream(input, CompressionMode.Decompress);
        using var output = new MemoryStream();
        gzip.CopyTo(output);
        return output.ToArray();
    }

    private static byte[] ComputeChecksum(byte[] data) {
        using var sha256 = SHA256.Create();
        return sha256.ComputeHash(data); // First 2 bytes used
    }
}
