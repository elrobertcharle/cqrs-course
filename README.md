Update database:
dotnet ef database update --startup-project ..\Post.Query.Api

Add migration:
dotnet ef migrations add Initial --startup-project ..\Post.Query.Api
