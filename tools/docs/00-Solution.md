flowchart TB
  subgraph Solution["GIPractice.sln"]
    WPF["GIPractice.Wpf\n(WPF + MahApps)"]
    Client["GIPractice.Client\n(typed API modules)"]
    Api["GIPractice.Api\n(ASP.NET Core)"]
    Infra["GIPractice.Infrastructure\n(EF Core, repos)"]
    Core["GIPractice.Core\n(domain + shared types)"]
    ApiModels["GIPractice.Api.Models\n(DTOs)"]
  end

  WPF --> Client
  Client --> ApiModels
  Api --> ApiModels
  Api --> Infra
  Infra --> Core
  Api --> Core
  Client --> Core
  WPF --> Core
