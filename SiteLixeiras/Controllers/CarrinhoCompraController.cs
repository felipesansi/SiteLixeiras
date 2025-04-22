using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SiteLixeiras.Models;
using SiteLixeiras.Repositorios;
using SiteLixeiras.Repositorios.Interfaces;
using SiteLixeiras.ViewModel;

namespace SiteLixeiras.Controllers
{
    public class CarrinhoCompraController : Controller
    {
        private readonly CarrinhoCompra _carrinhoCompra;
        private readonly IProdutosRepositorio _produtosRepositorio;

        public CarrinhoCompraController(CarrinhoCompra carrinhoCompra, IProdutosRepositorio produtosRepositorio)
        {
            _carrinhoCompra = carrinhoCompra;
            _produtosRepositorio = produtosRepositorio;
        }

        public IActionResult Index()
        {
            var itens = _carrinhoCompra.GetCarrinhoCompraItems();
            _carrinhoCompra.CarrinhoCompraItems = itens;
            var carrinhoCompraViewModel = new CarrinhoCompraViewModel
            {
                CarrinhoCompra = _carrinhoCompra,
                TotalCarrinho = _carrinhoCompra.GetCarrinhoCompraTotal()
            };
            return View(carrinhoCompraViewModel);
        }
        [Authorize]
        public IActionResult AdicionarAoCarrinho(int id)
        {
            var produtoSelecionado = _produtosRepositorio.produtos.FirstOrDefault(p => p.Id_Produto == id);
            if (produtoSelecionado != null)
            {
                _carrinhoCompra.AdicionarAoCarrinho(produtoSelecionado);
            }
            return RedirectToAction("Index");
        }
        [Authorize]
        public IActionResult RemoverDoCarrinho(int id)
        {
            var produtoSelecionado = _produtosRepositorio.produtos.FirstOrDefault(p => p.Id_Produto == id);
            if (produtoSelecionado != null)
            {
                _carrinhoCompra.RemoverDoCarrinho(produtoSelecionado);
            }
            return RedirectToAction("Index");
        }
       
    }
}
