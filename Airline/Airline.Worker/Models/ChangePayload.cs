namespace Worker.Models;

//Deðiþen fiyat bilgisini tutacaðýz.
public class ChangePayload
{
    public string Code { get; set; } = default!;
    public decimal NewPrice { get; set; } = default!;
}