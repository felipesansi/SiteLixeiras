# SiteLixeiras

Sistema web para gestão de produtos, pedidos e endereços de entrega de lixeiras de resina, com área administrativa, integração com e-mail, criptografia de dados sensíveis e recursos de SEO.

## Tecnologias Utilizadas

- .NET 9
- C# 13
- ASP.NET Core MVC
- Blazor (componentes e integração)
- Entity Framework Core
- SQL Server
- Bootstrap 5
- Font Awesome
- Identity (autenticação e autorização)
- Razor Views

## Funcionalidades

- **Catálogo de Produtos:** Listagem, filtro por preço, detalhes e imagens dos produtos.
- **Carrinho de Compras:** Adição, remoção e resumo de itens.
- **Pedidos:** Checkout, histórico, confirmação e integração com e-mail.
- **Endereços de Entrega:** Cadastro, edição, exclusão e consulta automática de CEP via ViaCEP.
- **Área Administrativa:** Gerenciamento de produtos, categorias e pedidos.
- **Notificações:** Sistema de notificações para usuários autenticados.
- **SEO:** Sitemap.xml e robots.txt dinâmicos, meta tags dinâmicas por página.
- **Criptografia:** Dados sensíveis de endereço são criptografados no banco.
- **Política de Privacidade e Termos de Uso:** Páginas dedicadas para compliance.

## Estrutura do Projeto

- `Controllers/` - Controllers principais do site.
- `Areas/Admin/Controllers/` - Controllers da área administrativa.
- `Models/` - Modelos de dados.
- `Views/` - Views Razor para cada controller.
- `wwwroot/` - Arquivos estáticos (CSS, JS, imagens, robots.txt).
- `Context/` - Contexto do Entity Framework.
- `Helpers/` - Utilitários como criptografia.
- `Services/` - Serviços como envio de e-mail.

3. **Execução do Projeto:**
   
## Configuração e Execução

1. **Pré-requisitos:**
   - .NET 9 SDK
   - SQL Server (local ou remoto)

2. **Configuração do Banco de Dados:**
   - Ajuste a string de conexão no `appsettings.json`.
   - Execute as migrations:
     
   
   
   O site estará disponível em `https://localhost:5001` (ou porta configurada).

4. **Acesso à Área Administrativa:**
   - Acesse `/Admin` com um usuário com perfil de administrador.

## SEO e Indexação

- O projeto gera automaticamente o arquivo `sitemap.xml` e `robots.txt` para facilitar a indexação por mecanismos de busca.
- As meta tags de título, descrição e palavras-chave são dinâmicas por página.

## Segurança

- Dados sensíveis de endereço são criptografados antes de serem salvos no banco.
- Áreas administrativas e de usuário são protegidas por autenticação e autorização via Identity.


