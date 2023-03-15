using System.Reflection;
using Caliburn.Micro;

namespace Presentation.WpfApp.ViewModels;

public sealed class AcercaDeViewModel : Screen
{
    public AcercaDeViewModel()
    {
        DisplayName = "Acerca De";
    }

    public static string Company
    {
        get { return GetExecutingAssemblyAttribute<AssemblyCompanyAttribute>(a => a.Company); }
    }

    public static string Product
    {
        get { return GetExecutingAssemblyAttribute<AssemblyProductAttribute>(a => a.Product); }
    }

    public static string Copyright
    {
        get { return GetExecutingAssemblyAttribute<AssemblyCopyrightAttribute>(a => a.Copyright); }
    }

    public static string Trademark
    {
        get { return GetExecutingAssemblyAttribute<AssemblyTrademarkAttribute>(a => a.Trademark); }
    }

    public static string Title
    {
        get { return GetExecutingAssemblyAttribute<AssemblyTitleAttribute>(a => a.Title); }
    }

    public static string Description
    {
        get { return GetExecutingAssemblyAttribute<AssemblyDescriptionAttribute>(a => a.Description); }
    }

    public static string Configuration
    {
        get { return GetExecutingAssemblyAttribute<AssemblyDescriptionAttribute>(a => a.Description); }
    }

    public static string FileVersion
    {
        get { return GetExecutingAssemblyAttribute<AssemblyFileVersionAttribute>(a => a.Version); }
    }

    public static Version Version => Assembly.GetExecutingAssembly().GetName().Version;
    public static string VersionFull => Version.ToString();
    public static string VersionMajor => Version.Major.ToString();
    public static string VersionMinor => Version.Minor.ToString();
    public static string VersionBuild => Version.Build.ToString();
    public static string VersionRevision => Version.Revision.ToString();

    public string Email => "andres@arsoft.net";
    public string Website => "https://www.arsoft.net/";
    public string Facebook => "https://www.facebook.com/AndresRamosSoftware/";
    public string Twitter => "https://twitter.com/ar_software";

    private static string GetExecutingAssemblyAttribute<T>(Func<T, string> value) where T : Attribute
    {
        var attribute = (T)Attribute.GetCustomAttribute(Assembly.GetExecutingAssembly(), typeof(T));
        return value.Invoke(attribute);
    }
}
