namespace NetPing.PriceGeneration.PriceList
{
    public interface IProduct
    {
        string Title { get; }
        string Description { get; }
        string Cost { get; }
        string ImageFileName { get; }
    }
}
