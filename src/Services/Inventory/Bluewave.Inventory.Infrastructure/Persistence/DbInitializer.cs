using Bluewave.Inventory.Domain.Entities;
using Bluewave.Inventory.Domain.Enums;

namespace Bluewave.Inventory.Infrastructure.Persistence;

public static class DbInitializer
{
    public static void Seed(InventoryDbContext context)
    {
        // If there are already products, do nothing (avoid duplicating data)
        if (context.Products.Any())
        {
            return;
        }

        // 1. Measurement Units
        var uomKg = new MeasurementUnit { Code = "KG", Name = "Quilograma" };
        var uomL = new MeasurementUnit { Code = "L", Name = "Litro" };
        var uomUn = new MeasurementUnit { Code = "UN", Name = "Unidade" };

        context.MeasurementUnits.AddRange(uomKg, uomL, uomUn);

        // 2. Categories
        var catRacao = new ProductCategory { Name = "Ração", Description = "Alimento para peixes" };
        var catQuimicos = new ProductCategory { Name = "Químicos", Description = "Tratamento de água" };

        context.Categories.AddRange(catRacao, catQuimicos);

        // 3. Suppliers
        var supplierAqua = new Supplier { CompanyName = "AquaNutri Ltda", Email = "contato@aquanutri.com" };
        var supplierBio = new Supplier { CompanyName = "BioVet", Email = "vendas@biovet.com" };

        context.Suppliers.AddRange(supplierAqua, supplierBio);

        // 4. Warehouses
        var whSilo = new Warehouse { Name = "Silo 01", Code = "SILO-01", IsVirtual = false };
        var whAlmox = new Warehouse { Name = "Almoxarifado Central", Code = "WH-MAIN", IsVirtual = false };
        var whTanque1 = new Warehouse { Name = "Tanque 01", Code = "TK-01", IsVirtual = true }; // Local lógico

        context.Warehouses.AddRange(whSilo, whAlmox, whTanque1);

        context.SaveChanges();
        
        // 5. Products
        var prodRacao = new Product
        {
            Sku = "RAC-45-EXT",
            Name = "Ração Extrusada 45%",
            Description = "Ração de crescimento",
            CategoryId = catRacao.Id,
            UomId = uomKg.Id,
            PreferredSupplierId = supplierAqua.Id,
            MinStockLevel = 500,
            IsPerishable = true,
            StandardCost = 12.50m
        };

        var prodAntibiotico = new Product
        {
            Sku = "MED-OXY-50",
            Name = "Oxitetraciclina Pó",
            Description = "Antibiótico",
            CategoryId = catQuimicos.Id,
            UomId = uomKg.Id,
            PreferredSupplierId = supplierBio.Id,
            MinStockLevel = 10,
            IsPerishable = true,
            ActiveIngredient = "Oxytetracycline",
            Concentration = 50,
            StandardCost = 150.00m
        };

        context.Products.AddRange(prodRacao, prodAntibiotico);

        // 6. Estoque Inicial (Transações de Entrada)
        var transacaoEntradaRacao = new InventoryTransaction
        {
            ProductId = prodRacao.Id,
            WarehouseId = whSilo.Id,
            TransactionType = Bluewave.Inventory.Domain.Enums.TransactionType.InboundPurchase,
            Quantity = 2000,
            BatchNumber = "LOTE-INI-01",
            UnitCost = 12.50m,
            ReferenceDocument = "CARGA-INICIAL",
            Notes = "Estoque inicial do sistema"
        };

        context.Transactions.Add(transacaoEntradaRacao);

        context.SaveChanges();
    }
}