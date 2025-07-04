﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SiteLixeiras.Context;

#nullable disable

namespace SiteLixeiras.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20250411144035_RemovendoTbPedidos")]
    partial class RemovendoTbPedidos
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.3")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("SiteLixeiras.Models.Categoria", b =>
                {
                    b.Property<int>("IdCategoria")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("IdCategoria"));

                    b.Property<string>("Descricao")
                        .IsRequired()
                        .HasMaxLength(400)
                        .HasColumnType("nvarchar(400)");

                    b.Property<string>("Nome")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.HasKey("IdCategoria");

                    b.ToTable("Categorias");
                });

            modelBuilder.Entity("SiteLixeiras.Models.Pedido", b =>
                {
                    b.Property<int>("PedidoId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("PedidoId"));

                    b.Property<string>("Cidade")
                        .IsRequired()
                        .HasMaxLength(999)
                        .HasColumnType("nvarchar(999)");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("Endereco1")
                        .IsRequired()
                        .HasMaxLength(999)
                        .HasColumnType("nvarchar(999)");

                    b.Property<string>("Endereco2")
                        .IsRequired()
                        .HasMaxLength(999)
                        .HasColumnType("nvarchar(999)");

                    b.Property<string>("Estado")
                        .IsRequired()
                        .HasMaxLength(999)
                        .HasColumnType("nvarchar(999)");

                    b.Property<string>("Nome")
                        .IsRequired()
                        .HasMaxLength(999)
                        .HasColumnType("nvarchar(999)");

                    b.Property<DateTime?>("PedididoEntregue")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("PedidoEnviado")
                        .HasColumnType("datetime2");

                    b.Property<decimal>("PedidoTotal")
                        .HasColumnType("decimal(18,2)");

                    b.Property<string>("SobreNome")
                        .IsRequired()
                        .HasMaxLength(999)
                        .HasColumnType("nvarchar(999)");

                    b.Property<string>("Telefone")
                        .IsRequired()
                        .HasMaxLength(25)
                        .HasColumnType("nvarchar(25)");

                    b.Property<int>("TotalItensPedidos")
                        .HasColumnType("int");

                    b.HasKey("PedidoId");

                    b.ToTable("Pedidos");
                });

            modelBuilder.Entity("SiteLixeiras.Models.PedidoDetalhe", b =>
                {
                    b.Property<int>("PedidoDetalheId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("PedidoDetalheId"));

                    b.Property<int>("PedidoId")
                        .HasColumnType("int");

                    b.Property<decimal>("Preco")
                        .HasColumnType("decimal(18,2)");

                    b.Property<int>("ProdutoId")
                        .HasColumnType("int");

                    b.Property<int>("Quantidade")
                        .HasColumnType("int");

                    b.HasKey("PedidoDetalheId");

                    b.HasIndex("PedidoId");

                    b.HasIndex("ProdutoId");

                    b.ToTable("PedidoDetalhes");
                });

            modelBuilder.Entity("SiteLixeiras.Models.Produtos", b =>
                {
                    b.Property<int>("Id_Produto")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id_Produto"));

                    b.Property<int>("CategoriaId")
                        .HasColumnType("int");

                    b.Property<int>("Codigo")
                        .HasColumnType("int");

                    b.Property<string>("Descricao")
                        .IsRequired()
                        .HasMaxLength(400)
                        .HasColumnType("nvarchar(400)");

                    b.Property<bool>("Destaque")
                        .HasColumnType("bit");

                    b.Property<string>("Imagem")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Nome")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<decimal>("Preco")
                        .HasColumnType("decimal(18,2)");

                    b.HasKey("Id_Produto");

                    b.HasIndex("CategoriaId");

                    b.ToTable("Produtos");
                });

            modelBuilder.Entity("SiteLixeiras.Models.PedidoDetalhe", b =>
                {
                    b.HasOne("SiteLixeiras.Models.Pedido", "Pedido")
                        .WithMany("PedidoItens")
                        .HasForeignKey("PedidoId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("SiteLixeiras.Models.Produtos", "Produto")
                        .WithMany()
                        .HasForeignKey("ProdutoId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Pedido");

                    b.Navigation("Produto");
                });

            modelBuilder.Entity("SiteLixeiras.Models.Produtos", b =>
                {
                    b.HasOne("SiteLixeiras.Models.Categoria", "Categoria")
                        .WithMany("Produtos")
                        .HasForeignKey("CategoriaId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Categoria");
                });

            modelBuilder.Entity("SiteLixeiras.Models.Categoria", b =>
                {
                    b.Navigation("Produtos");
                });

            modelBuilder.Entity("SiteLixeiras.Models.Pedido", b =>
                {
                    b.Navigation("PedidoItens");
                });
#pragma warning restore 612, 618
        }
    }
}
