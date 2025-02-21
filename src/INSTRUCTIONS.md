## Instructions

### Install EF Cli
```bash
dotnet tool install --global dotnet-ef  
```

### Add migration files
```bash
dotnet ef migrations add InitialCreate -p src/ProductManagement.Infra -s src/ProductManagement.API 
```

### Apply migration
```bash
dotnet ef database update -p src/ProductManagement.Infra -s src/ProductManagement.API
```

#### Explanation:
- `-p` ProductManagement.Infra: Specifies the project that contains your DbContext (and migration files).
- `-s` ProductManagement.API: Specifies the startup project, which is used for runtime configurations like the connection string.
