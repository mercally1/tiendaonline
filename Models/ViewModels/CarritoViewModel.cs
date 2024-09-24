namespace tienda.Models.ViewModels;

public class CarritoViewModel
{
    public List<CarritoItemViewModel> Item { get; set; } = new List<CarritoItemViewModel>();

    public decimal Total { get; set; }
}
