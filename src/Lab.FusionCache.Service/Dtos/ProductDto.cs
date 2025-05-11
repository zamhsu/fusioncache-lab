namespace Lab.FusionCache.Service.Dtos;

public class ProductDto
{
    public int Id { get; set; }
    public string Title { get; set; }
    public int Quantity { get; set; }
    public int Price { get; set; }
    public string Description { get; set; }
    public DateTime CreateDate { get; set; }
    public DateTime UpdateDate { get; set; }
}