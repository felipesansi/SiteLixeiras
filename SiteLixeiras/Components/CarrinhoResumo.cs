using Microsoft.AspNetCore.Mvc;
using SiteLixeiras.Models;
using SiteLixeiras.ViewModel;

namespace SiteLixeiras.Components
{
    public class CarrinhoResumo:ViewComponent
    {
        private readonly CarrinhoCompra _carrinhoCompra;
        public CarrinhoResumo(CarrinhoCompra carrinhoCompra)
        {
            _carrinhoCompra = carrinhoCompra;
        }
        public IViewComponentResult Invoke()
        {
            var itens = _carrinhoCompra.GetCarrinhoCompraItems();
            _carrinhoCompra.CarrinhoCompraItems = itens;
            var carrinhoResumo = new CarrinhoCompraViewModel
            {
                CarrinhoCompra = _carrinhoCompra,
                TotalCarrinho = _carrinhoCompra.GetCarrinhoCompraTotal()
            };
            return View(carrinhoResumo);
        }
    }
}
