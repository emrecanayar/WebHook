namespace Worker.Models;

//De�i�en fiyat bilgisini tutaca��z.
public class ChangePayload
{
    public string Code { get; set; } = default!;
    public decimal NewPrice { get; set; } = default!;
}