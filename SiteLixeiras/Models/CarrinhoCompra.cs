﻿using SiteLixeiras.Context;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace SiteLixeiras.Models
{
    public class CarrinhoCompra
    {
        private readonly AppDbContext _appDbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CarrinhoCompra(AppDbContext appDbContext, IHttpContextAccessor httpContextAccessor)
        {
            _appDbContext = appDbContext;
            _httpContextAccessor = httpContextAccessor;
        }

        public string CarrinhoCompraId { get; set; } = string.Empty;
        public string UsuarioId { get; set; } = string.Empty;

        public List<CarrinhoCompraItem> CarrinhoCompraItems { get; set; } = new List<CarrinhoCompraItem>();

        public static CarrinhoCompra GetCarrinhoCompra(IServiceProvider services)
        {
            var context = services.GetRequiredService<AppDbContext>();
            var httpContextAccessor = services.GetRequiredService<IHttpContextAccessor>();

            var user = httpContextAccessor.HttpContext?.User;
            var userId = user?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            string carrinhoId;

            if (!string.IsNullOrEmpty(userId))
            {
                // Se usuário está logado, usa o Id dele para identificar o carrinho
                carrinhoId = userId;
            }
            else
            {
                // Se não está logado, usa sessão para identificar o carrinho
                carrinhoId = httpContextAccessor.HttpContext?.Session?.GetString("CarrinhoId") ?? Guid.NewGuid().ToString();
                httpContextAccessor.HttpContext?.Session?.SetString("CarrinhoId", carrinhoId);
            }

            return new CarrinhoCompra(context, httpContextAccessor)
            {
                CarrinhoCompraId = carrinhoId,
                UsuarioId = userId ?? string.Empty
            };
        }

        public void AdicionarAoCarrinho(Produtos produto)
        {
            // Busca o item por CarrinhoCompraId (que pode ser UserId ou Sessão)
            var carrinhoCompraItem = _appDbContext.CarrinhoCompraItens.SingleOrDefault(
                c => c.Produtos.Id_Produto == produto.Id_Produto && c.CarrinhoCompraId == CarrinhoCompraId);

            if (carrinhoCompraItem == null)
            {
                carrinhoCompraItem = new CarrinhoCompraItem
                {
                    CarrinhoCompraId = CarrinhoCompraId,
                    UsuarioId = UsuarioId, // seta usuário se tiver
                    Produtos = produto,
                    Quantidade = 1
                };

                _appDbContext.CarrinhoCompraItens.Add(carrinhoCompraItem);
            }
            else
            {
                carrinhoCompraItem.Quantidade++;
            }

            _appDbContext.SaveChanges();
        }

        public void RemoverDoCarrinho(Produtos produto)
        {
            var carrinhoCompraItem = _appDbContext.CarrinhoCompraItens.SingleOrDefault(
                c => c.Produtos.Id_Produto == produto.Id_Produto && c.CarrinhoCompraId == CarrinhoCompraId);
            if (carrinhoCompraItem != null)
            {
                if (carrinhoCompraItem.Quantidade > 1)
                {
                    carrinhoCompraItem.Quantidade--;
                }
                else
                {
                    _appDbContext.CarrinhoCompraItens.Remove(carrinhoCompraItem);
                }
            }
            _appDbContext.SaveChanges();
        }

        public decimal GetCarrinhoCompraTotal()
        {
            var total = _appDbContext.CarrinhoCompraItens.Where(c => c.CarrinhoCompraId == CarrinhoCompraId)
                .Select(c => c.Produtos.Preco * c.Quantidade).Sum();
            return total;
        }

        public List<CarrinhoCompraItem> GetCarrinhoCompraItems()
        {
            CarrinhoCompraItems = _appDbContext.CarrinhoCompraItens
                .Where(c => c.CarrinhoCompraId == CarrinhoCompraId)
                .Include(c => c.Produtos)
                .ToList();

            return CarrinhoCompraItems;
        }

        public void LimparCarrinho()
        {
            var carrinhoCompraItems = _appDbContext.CarrinhoCompraItens
                .Where(c => c.CarrinhoCompraId == CarrinhoCompraId);
            _appDbContext.CarrinhoCompraItens.RemoveRange(carrinhoCompraItems);
            _appDbContext.SaveChanges();
        }
    }
}
