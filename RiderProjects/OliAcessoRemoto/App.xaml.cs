using System.Windows;

namespace OliAcessoRemoto;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        
        // Configure global exception handling
        this.DispatcherUnhandledException += (sender, args) =>
        {
            MessageBox.Show($"Erro não tratado: {args.Exception.Message}", 
                          "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            args.Handled = true;
        };
    }
}
