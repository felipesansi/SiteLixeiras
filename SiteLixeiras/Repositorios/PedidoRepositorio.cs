using SiteLixeiras.Context;
using SiteLixeiras.Models;
using SiteLixeiras.Repositorios.Interfaces;

namespace SiteLixeiras.Repositorios
{
    public class PedidoRepositorio : IPedido
    {
        private readonly AppDbContext _context;
        private readonly CarrinhoCompra carrinhoCompra;

        public PedidoRepositorio(AppDbContext context, CarrinhoCompra carrinhoCompra)
        {
            _context = context;
            this.carrinhoCompra = carrinhoCompra;
        }

        public void CriarPedido(Pedido pedido)
        {
            pedido.PedidoEnviado = DateTime.Now;

            _context.Pedidos.Add(pedido);
            _context.SaveChanges();
            var carrinhoCompraItens = carrinhoCompra.CarrinhoCompraItems;

            foreach (var carrinhoItem in carrinhoCompraItens)
            {
                var pedidoDetalhe = new PedidoDetalhe
                {
                    Quantidade = carrinhoItem.Quantidade,
                    ProdutoId = carrinhoItem.Produtos.Id_Produto,
                    PedidoId = pedido.PedidoId,
                    Preco = carrinhoItem.Produtos.Preco
                };
                _context.PedidoDetalhes.Add(pedidoDetalhe);
            }
            _context.SaveChanges();
        }
    }
}
