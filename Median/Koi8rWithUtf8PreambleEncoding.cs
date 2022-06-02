using System.Text;

namespace Median;

public class Koi8rWithUtf8PreambleEncoding: Encoding
{
    static Koi8rWithUtf8PreambleEncoding()
    {
        RegisterProvider(CodePagesEncodingProvider.Instance);
    }

    public override byte[] GetPreamble() => new byte[3] { 0xEF, 0xBB, 0xBF };

    private Encoding RealEncoding { get; } = GetEncoding("koi8-r");
    
    public override int GetByteCount(char[] chars, int index, int count)
    {
        return RealEncoding.GetByteCount(chars, index, count);
    }

    public override int GetBytes(char[] chars, int charIndex, int charCount, byte[] bytes, int byteIndex)
    {
        return RealEncoding.GetBytes(chars, charIndex, charCount, bytes, byteIndex);
    }

    public override int GetCharCount(byte[] bytes, int index, int count)
    {
        return RealEncoding.GetCharCount(bytes, index, count);
    }

    public override int GetChars(byte[] bytes, int byteIndex, int byteCount, char[] chars, int charIndex)
    {
        return RealEncoding.GetChars(bytes, byteIndex, byteCount, chars, charIndex);
    }

    public override int GetMaxByteCount(int charCount)
    {
        return RealEncoding.GetMaxByteCount(charCount);
    }

    public override int GetMaxCharCount(int byteCount)
    {
        return RealEncoding.GetMaxCharCount(byteCount);
    }
}