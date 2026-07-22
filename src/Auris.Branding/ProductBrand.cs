using System.Reflection;

namespace Auris.Branding;

public static class ProductBrand
{
    private static readonly Assembly CurrentAssembly = typeof(ProductBrand).Assembly;

    public static string Name { get; } =
        CurrentAssembly
            .GetCustomAttribute<AssemblyProductAttribute>()
            ?.Product
        ?? "Auris";

    public static string Description { get; } =
        CurrentAssembly
            .GetCustomAttribute<AssemblyDescriptionAttribute>()
            ?.Description
        ?? "Local audio service";

    public static string Company { get; } =
        CurrentAssembly
            .GetCustomAttribute<AssemblyCompanyAttribute>()
            ?.Company
        ?? string.Empty;
}