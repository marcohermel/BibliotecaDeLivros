# BibliotecaDeLivros


Sistema para gerenciamento de biblioteca
O sistema possui 2 perfis de acesso: 

Administrador(Gerência Livros)

Dados de acesso do perfil admin:
usuário:admin@admin.com
senha:Admin123!

Cliente(Aluga Livros)
Cadastrar na hora


Arquitetura:
- DotNet core 2.2
- MVC 
- Bootstrap 4
- Autenticação via Identity
- SQLSERVEREXPRESS

Instruções para rodar o projeto.

- Abrir a solution
- Setar o servidor SQL na ConnectionString do banco no arquivo appsettings.json
- Abrir o console (Tools > Nuget Package Manager > Package Manager Console)
- Executar a Migration com o comando Update-Database para a criar a base de dados
- Executar o projeto
