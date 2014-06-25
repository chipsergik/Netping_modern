using NetPing.PriceGeneration.Word;

namespace NetPing.PriceGeneration
{
    public interface IReplacement
    {
        void Apply(WordRange range, object source);
        string TagName { get; }
    }
}
