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
        var uomKg = new MeasurementUnit("KG", "Quilograma", "Medida de peso padrão");
        var uomL = new MeasurementUnit("L", "Litro", "Medida de volume padrão");
        var uomUn = new MeasurementUnit("UN", "Unidade", "Medida unitária padrão");

        context.MeasurementUnits.AddRange(uomKg, uomL, uomUn);

        // 2. Categories
        var catRacao = new ProductCategory("Ração", "Alimento para peixes");
        var catQuimicos = new ProductCategory("Químicos", "Tratamento de água");

        context.Categories.AddRange(catRacao, catQuimicos);

        // 3. Suppliers
        var supplierAqua = new Supplier("AquaNutri Ltda", "12.345.678/0001-90", "João Silva", "contato@aquanutri.com", "11999999999", "São Paulo");
        var supplierBio = new Supplier("BioVet", "98.765.432/0001-10", "Maria Souza", "vendas@biovet.com", "21988888888", "Rio de Janeiro");

        context.Suppliers.AddRange(supplierAqua, supplierBio);

        // 4. Warehouses
        // Passando: Name, Code, Address (nulo por enquanto), IsVirtual
        var whSilo = new Warehouse("Silo 01", "SILO-01", null, false);
        var whAlmox = new Warehouse("Almoxarifado Central", "WH-MAIN", "Galpão Principal", false);
        var whTanque1 = new Warehouse("Tanque 01", "TK-01", null, true); // Local lógico

        context.Warehouses.AddRange(whSilo, whAlmox, whTanque1);

        context.Warehouses.AddRange(whSilo, whAlmox, whTanque1);

        context.SaveChanges();

        // 5. Products
        var prodRacao = new Product(
            "RAC-45-EXT", "Ração Extrusada 45%", "Ração de crescimento",
            catRacao.Id, uomKg.Id, supplierAqua.Id,
            500, null, true, false, null, null, 12.50m
        );

        var prodAntibiotico = new Product(
            "MED-OXY-50", "Oxitetraciclina Pó", "Antibiótico",
            catQuimicos.Id, uomKg.Id, supplierBio.Id,
            10, null, true, false, "Oxytetracycline", 50, 150.00m
        );

        context.Products.AddRange(prodRacao, prodAntibiotico);

        // 6. Estoque Inicial
        var transacaoEntradaRacao = new InventoryTransaction(
            prodRacao.Id,
            whSilo.Id,
            TransactionType.InboundPurchase,
            2000,
            "LOTE-INI-01",
            12.50m,
            "CARGA-INICIAL",
            "Estoque inicial do sistema"
        );

        context.Transactions.Add(transacaoEntradaRacao);

        context.Transactions.Add(transacaoEntradaRacao);

        context.SaveChanges();
    }
}