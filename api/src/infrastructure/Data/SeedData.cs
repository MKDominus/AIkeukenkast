using api.Domain;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;

namespace api.Infrastructure.Data;

public static class SeedData
{
    public static void Initialize(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        if (context.Products.Any()) return; // already seeded

        // Ingredients (Dutch examples)
        var water = new Ingredient { Name = "Water", Description = "Gedistilleerd water", IsHazardous = false, Concentration = 100 };
        var bleach = new Ingredient { Name = "Natriumhypochloriet", Description = "Schoonmaakbleekmiddel", IsHazardous = true, Concentration = 5 };
        var ammonia = new Ingredient { Name = "Ammoniak", Description = "Ammoniakoplossing", IsHazardous = true, Concentration = 2 };
        var ethanol = new Ingredient { Name = "Ethanol", Description = "Alcohol voor desinfectie", IsHazardous = true, Concentration = 70 };

        context.Ingredients.AddRange(water, bleach, ammonia, ethanol);
        context.SaveChanges();

        // Products (Dutch examples)
        var ajax = new Product
        {
            ProductName = "Ajax Allesreiniger",
            ProductType = "Reiniger",
            ImageURL = "https://example.com/images/ajax.jpg",
            RiskLevel = ProductRiskLevel.Riskant,
            WarningLabels = new System.Collections.Generic.List<ProductWarningLabel> {
                new ProductWarningLabel { Type = "ghs07", Description = "Irriterend" }
            },
            Dangers = new System.Collections.Generic.List<string> { "Irritatie aan ogen", "Huidirritatie" },
            Precautions = new System.Collections.Generic.List<string> { "Vermijd contact met ogen", "Draag handschoenen" },
            Alternatives = new System.Collections.Generic.List<string> { "Milieuvriendelijke allesreiniger" },
            Ingredients = new System.Collections.Generic.List<Ingredient> { water, ammonia }
        };

        var cloroxLike = new Product
        {
            ProductName = "BleachPlus",
            ProductType = "Bleekmiddel",
            ImageURL = "https://example.com/images/bleach.jpg",
            RiskLevel = ProductRiskLevel.Onveilig,
            WarningLabels = new System.Collections.Generic.List<ProductWarningLabel> {
                new ProductWarningLabel { Type = "ghs02", Description = "Ontvlambaar" },
                new ProductWarningLabel { Type = "ghs05", Description = "Bijtend" }
            },
            Dangers = new System.Collections.Generic.List<string> { "Bijtend voor huid en ogen", "Gevaar bij inademing" },
            Precautions = new System.Collections.Generic.List<string> { "Niet mengen met andere reinigers", "Gebruik in goed geventileerde ruimte" },
            Alternatives = new System.Collections.Generic.List<string> { "Bleekvrij ontsmettingsmiddel" },
            Ingredients = new System.Collections.Generic.List<Ingredient> { water, bleach }
        };

        var handgel = new Product
        {
            ProductName = "HandSanit Alcoholgel",
            ProductType = "Desinfectiemiddel",
            ImageURL = "https://example.com/images/handgel.jpg",
            RiskLevel = ProductRiskLevel.Veilig,
            WarningLabels = new System.Collections.Generic.List<ProductWarningLabel> {
                new ProductWarningLabel { Type = "ghs02", Description = "Ontvlambaar" }
            },
            Dangers = new System.Collections.Generic.List<string> { "Ontvlambaar" },
            Precautions = new System.Collections.Generic.List<string> { "Houd uit de buurt van open vuur" },
            Alternatives = new System.Collections.Generic.List<string> { "Alcoholvrije handreiniger" },
            Ingredients = new System.Collections.Generic.List<Ingredient> { ethanol, water }
        };

        context.Products.AddRange(ajax, cloroxLike, handgel);
        context.SaveChanges();

        // Municipalities (Netherlands examples)
        var amsterdam = new Municipality { Name = "Amsterdam", Population = 881000 };
        var rotterdam = new Municipality { Name = "Rotterdam", Population = 651000 };
        context.Municipalities.AddRange(amsterdam, rotterdam);
        context.SaveChanges();

        // Users
        var user = new User { Name = "Test Gebruiker", Age = 30 };
        context.Users.Add(user);
        context.SaveChanges();

        // Scans with detected products
        var scan1 = new Scan
        {
            ScanDate = DateTime.UtcNow.AddDays(-1),
            ImageUrl = "https://example.com/scans/scan1.jpg",
            MunicipalityId = amsterdam.Id,
            UserId = user.Id,
            DetectedProducts = new System.Collections.Generic.List<DetectedProduct>
            {
                new DetectedProduct { ProductId = ajax.ProductId, Confidence = 0.92, Count = 1 },
                new DetectedProduct { ProductId = handgel.ProductId, Confidence = 0.85, Count = 2 }
            }
        };

        var scan2 = new Scan
        {
            ScanDate = DateTime.UtcNow.AddDays(-2),
            ImageUrl = "https://example.com/scans/scan2.jpg",
            MunicipalityId = rotterdam.Id,
            UserId = user.Id,
            DetectedProducts = new System.Collections.Generic.List<DetectedProduct>
            {
                new DetectedProduct { ProductId = cloroxLike.ProductId, Confidence = 0.97, Count = 1 }
            }
        };

        context.Scans.AddRange(scan1, scan2);
        context.SaveChanges();
    }
}
