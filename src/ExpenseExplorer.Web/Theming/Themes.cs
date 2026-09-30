using MudBlazor;

namespace ExpenseExplorer.Web.Theming;

/// <summary>
/// Colours of the app. The dark palette follows the owner's desktop theme: deep green-black
/// background, mint text and material green, red, orange, blue and purple accents.
/// </summary>
public static class Themes
{
    private const string Background = "#101814";
    private const string Surface = "#17221c";
    private const string Raised = "#1e2b24";
    private const string Foreground = "#e8f5e9";
    private const string Muted = "#a3b8a7";
    private const string Line = "#2a3a31";
    private const string Green = "#66bb6a";
    private const string GreenDark = "#2e7d32";
    private const string Red = "#ef5350";
    private const string Orange = "#ffa726";
    private const string Blue = "#42a5f5";
    private const string Purple = "#ab47bc";

    public static readonly MudTheme App = new()
    {
        PaletteDark = new PaletteDark
        {
            Black = Background,
            Background = Background,
            BackgroundGray = Surface,
            Surface = Surface,
            DrawerBackground = Surface,
            DrawerText = Foreground,
            AppbarBackground = Surface,
            AppbarText = Foreground,
            TextPrimary = Foreground,
            TextSecondary = Muted,
            TextDisabled = "#e8f5e961",
            ActionDefault = Muted,
            ActionDisabled = "#e8f5e94d",
            ActionDisabledBackground = "#e8f5e91f",
            Primary = Green,
            PrimaryContrastText = Background,
            Secondary = Purple,
            SecondaryContrastText = Foreground,
            Tertiary = Blue,
            Info = Blue,
            InfoContrastText = Background,
            Success = Green,
            SuccessContrastText = Background,
            Warning = Orange,
            WarningContrastText = Background,
            Error = Red,
            ErrorContrastText = Background,
            Dark = Raised,
            LinesDefault = Line,
            LinesInputs = "#4a5e52",
            TableLines = Line,
            TableHover = Raised,
            TableStriped = Raised,
            Divider = Line,
            DividerLight = Line,
            OverlayDark = "#0b110ecc",
            HoverOpacity = 0.08,
        },
        PaletteLight = new PaletteLight
        {
            Primary = GreenDark,
            AppbarBackground = GreenDark,
            Secondary = Purple,
            Tertiary = Blue,
            Info = "#1e88e5",
            Success = GreenDark,
            Warning = "#ef6c00",
            Error = "#d32f2f",
        },
    };
}
