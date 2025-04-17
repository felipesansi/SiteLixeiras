using SiteLixeiras.Context;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace SiteLixeiras.Models
{
    public class CarrinhoCompra
    {
        private readonly AppDbContext _appDbContext; // Corrigido: Adicionada a propriedade AppDbContext
        private readonly IHttpContextAccessor _httpContextAccessor; // Corrigido: Adicionada a propriedade IHttpContextAccessor

        public CarrinhoCompra(AppDbContext appDbContext, IHttpContextAccessor httpContextAccessor)
        {
            _appDbContext = appDbContext;
            _httpContextAccessor = httpContextAccessor;
        }

        public string CarrinhoCompraId { get; set; } = string.Empty; // Inicializa com valor padrão
        public List<CarrinhoCompraItem> CarrinhoCompraItems { get; set; } = new List<CarrinhoCompraItem>(); // Inicializa com uma lista vazia

        public static CarrinhoCompra GetCarrinhoCompra(IServiceProvider services) // Método para obter o carrinho de compras
        {
            var context = services.GetRequiredService<AppDbContext>(); // Corrigido: Adicionada a propriedade AppDbContext
            var httpContextAccessor = services.GetRequiredService<IHttpContextAccessor>(); // Corrigido: Adicionada a propriedade IHttpContextAccessor

            var carrinhoId = httpContextAccessor?.HttpContext?.Session?.GetString("CarrinhoId") ?? Guid.NewGuid().ToString(); // Adicionada a propriedade CarrinhoId
            httpContextAccessor?.HttpContext?.Session?.SetString("CarrinhoId", carrinhoId); // Adicionada a propriedade CarrinhoId

            return new CarrinhoCompra(context, httpContextAccessor // Adicionada a propriedade IHttpContextAccessor
            )
            {
                CarrinhoCompraId = carrinhoId // Adicionada a propriedade CarrinhoId
            };
        }

        // Método para adicionar produto ao carrinho
        public void AdicionarAoCarrinho(Produtos produto) // adiciona um produto ao carrinho
        {
            var carrinhoCompraItem = _appDbContext.CarrinhoCompraItens.SingleOrDefault(
                c => c.Produtos.Id_Produto == produto.Id_Produto && c.CarrinhoCompraId == CarrinhoCompraId); //  Adicionada a propriedade CarrinhoId

            if (carrinhoCompraItem == null) // Se o item não existe no carrinho
            {
                carrinhoCompraItem = new CarrinhoCompraItem // Adicionada a propriedade CarrinhoCompraItem
                {
                    CarrinhoCompraId = CarrinhoCompraId, // Adicionada a propriedade CarrinhoId
                    Produtos = produto, // Adicionada a propriedade Produtos
                    Quantidade = 1 // Adicionada a propriedade Quantidade
                };

                _appDbContext.CarrinhoCompraItens.Add(carrinhoCompraItem); // Adicionada a propriedade CarrinhoCompraItem
            }
            else
            {
                carrinhoCompraItem.Quantidade++; // Se o item já existe, incrementa a quantidade
            }

            _appDbContext.SaveChanges(); // Salva as alterações no banco de dados
        }
        // Método para remover produto do carrinho
        public void RemoverDoCarrinho(Produtos produto) // remove um produto do carrinho
        {
            var carrinhoCompraItem = _appDbContext.CarrinhoCompraItens.SingleOrDefault(
                c => c.Produtos.Id_Produto == produto.Id_Produto && c.CarrinhoCompraId == CarrinhoCompraId); // Adicionada a propriedade CarrinhoId
            if (carrinhoCompraItem != null) // Se o item existe no carrinho
            {
                if (carrinhoCompraItem.Quantidade > 1) // Se a quantidade for maior que 1
                {
                    carrinhoCompraItem.Quantidade--; // Decrementa a quantidade
                }
                else
                {
                    _appDbContext.CarrinhoCompraItens.Remove(carrinhoCompraItem); // Remove o item do carrinho
                }
            }
            _appDbContext.SaveChanges(); // Salva as alterações no banco de dados
        }
        // Método para obter o total do carrinho
        public decimal GetCarrinhoCompraTotal() // retorna o total do carrinho
        {
            var total = _appDbContext.CarrinhoCompraItens.Where(c => c.CarrinhoCompraId == CarrinhoCompraId) // Adicionada a propriedade CarrinhoId
                .Select(c => c.Produtos.Preco * c.Quantidade).Sum(); // Adicionada a propriedade Preco_Produto
            return total; // Retorna o total
        }


    
    public List<CarrinhoCompraItem> GetCarrinhoCompraItems() // retorna os itens do carrinho
        {
            return CarrinhoCompraItems ?? (_appDbContext.CarrinhoCompraItens.Where(c => c.CarrinhoCompraId == CarrinhoCompraId) // Adicionada a propriedade CarrinhoId
                .Include(c => c.Produtos).ToList()); // Adicionada a propriedade Produtos
        }
    }
}

