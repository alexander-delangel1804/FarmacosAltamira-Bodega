using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace Altamira.Bodega.Presentation.Resources;

public sealed class EstadoAlColorConverter : IValueConverter
{
    public static readonly EstadoAlColorConverter Instance = new();

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var estado = value as string;
        if (string.IsNullOrEmpty(estado))
            return new SolidColorBrush(Color.FromRgb(0x64, 0x74, 0x8B)); // gris

        return estado switch
        {
            "VIGENTE" => new SolidColorBrush(Color.FromRgb(0x10, 0xB9, 0x81)), // verde
            "PROXIMO_A_CADUCAR" => new SolidColorBrush(Color.FromRgb(0xF5, 0x9E, 0x0B)), // ámbar
            "CADUCADO" => new SolidColorBrush(Color.FromRgb(0xEF, 0x44, 0x44)), // rojo
            "AGOTADO" => new SolidColorBrush(Color.FromRgb(0x94, 0xA3, 0xB8)), // gris oscuro
            "BAJO_MINIMO" => new SolidColorBrush(Color.FromRgb(0xF5, 0x9E, 0x0B)), // ámbar
            _ => new SolidColorBrush(Color.FromRgb(0x64, 0x74, 0x8B))
        };
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
