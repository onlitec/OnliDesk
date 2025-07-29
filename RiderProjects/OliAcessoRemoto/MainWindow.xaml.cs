using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using OliAcessoRemoto.Services;

namespace OliAcessoRemoto;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window, INotifyPropertyChanged
{
    private bool _isServerRunning = false;
    private bool _isConnected = false;
    private string _localId = "000 000 000";
    private string? _authToken;
    private readonly IServerApiService _serverApi;

    public ObservableCollection<RecentConnection> RecentConnections { get; set; } = new();

    public MainWindow()
    {
        try
        {
            _serverApi = new ServerApiService();
            InitializeComponent();
            InitializeData();
            UpdateUI();
            _ = InitializeServerConnectionAsync();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Erro ao inicializar a aplicação: {ex.Message}\n\nDetalhes: {ex}",
                          "Erro de Inicialização", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void InitializeData()
    {
        // Inicializar conexões recentes (dados de exemplo)
        RecentConnections = new ObservableCollection<RecentConnection>
        {
            new RecentConnection { Name = "Computador do Escritório", Id = "987 654 321" },
            new RecentConnection { Name = "Laptop Casa", Id = "456 789 123" },
            new RecentConnection { Name = "Servidor Principal", Id = "111 222 333" }
        };

        RecentConnectionsListBox.ItemsSource = RecentConnections;

        // Gerar ID local (normalmente seria obtido do servidor)
        GenerateLocalId();
    }

    private async Task InitializeServerConnectionAsync()
    {
        try
        {
            // Verificar se o servidor está online
            FooterStatusText.Text = "Verificando conexão com servidor...";

            bool serverOnline = await _serverApi.CheckServerHealthAsync();
            if (!serverOnline)
            {
                FooterStatusText.Text = "Servidor offline - Modo offline";
                MessageBox.Show("Não foi possível conectar ao servidor OnliDek.\nO aplicativo funcionará em modo offline.",
                              "Servidor Offline", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Registrar cliente no servidor
            FooterStatusText.Text = "Registrando cliente no servidor...";
            var computerName = Environment.MachineName;
            var userName = Environment.UserName;
            var clientName = $"{computerName} ({userName})";

            var registrationResult = await _serverApi.RegisterClientAsync(clientName);

            if (registrationResult.Success)
            {
                _localId = registrationResult.ClientId;
                _authToken = registrationResult.Token;
                _serverApi.SetAuthToken(_authToken);

                LocalIdTextBlock.Text = _localId;
                FooterStatusText.Text = $"Conectado ao servidor - ID: {_localId}";
                _isConnected = true;
            }
            else
            {
                FooterStatusText.Text = $"Erro no registro: {registrationResult.Message}";
                MessageBox.Show($"Erro ao registrar no servidor:\n{registrationResult.Message}",
                              "Erro de Registro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        catch (Exception ex)
        {
            FooterStatusText.Text = "Erro de conexão com servidor";
            MessageBox.Show($"Erro ao conectar com servidor:\n{ex.Message}",
                          "Erro de Conexão", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        UpdateUI();
    }

    private void GenerateLocalId()
    {
        // Este método agora é chamado apenas como fallback
        Random random = new Random();
        _localId = $"{random.Next(100, 999)} {random.Next(100, 999)} {random.Next(100, 999)}";
        LocalIdTextBlock.Text = _localId;
    }

    private void UpdateUI()
    {
        // Atualizar status da conexão
        if (_isConnected)
        {
            StatusIndicator.Fill = new SolidColorBrush(Colors.Green);
            StatusText.Text = "Conectado";
        }
        else
        {
            StatusIndicator.Fill = new SolidColorBrush(Colors.Red);
            StatusText.Text = "Desconectado";
        }

        // Atualizar status do servidor
        if (_isServerRunning)
        {
            ServerStatusIndicator.Fill = new SolidColorBrush(Colors.Green);
            ServerStatusText.Text = "Servidor ativo - Aguardando conexões";
            StartServerButton.Content = "Parar Servidor";
        }
        else
        {
            ServerStatusIndicator.Fill = new SolidColorBrush(Colors.Red);
            ServerStatusText.Text = "Servidor parado";
            StartServerButton.Content = "Iniciar Servidor";
        }
    }

    // Event Handlers
    private void ConnectButton_Click(object sender, RoutedEventArgs e)
    {
        string remoteId = RemoteIdTextBox.Text.Trim();

        if (string.IsNullOrEmpty(remoteId))
        {
            MessageBox.Show("Por favor, digite o ID do computador remoto.", "ID Necessário",
                          MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        // Validar formato do ID (XXX XXX XXX)
        if (!IsValidId(remoteId))
        {
            MessageBox.Show("Formato de ID inválido. Use o formato: XXX XXX XXX", "ID Inválido",
                          MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        // Simular tentativa de conexão
        FooterStatusText.Text = $"Conectando ao ID {remoteId}...";
        ConnectButton.IsEnabled = false;
        ConnectButton.Content = "Conectando...";

        // Implementar lógica real de conexão
        _ = Task.Run(async () =>
        {
            try
            {
                // Verificar se o cliente de destino está online
                var targetStatus = await _serverApi.GetClientStatusAsync(remoteId);
                if (targetStatus == null || !targetStatus.IsOnline)
                {
                    Dispatcher.Invoke(() =>
                    {
                        FooterStatusText.Text = "Cliente de destino offline";
                        MessageBox.Show($"O cliente {remoteId} não está online ou não existe.",
                                      "Cliente Offline", MessageBoxButton.OK, MessageBoxImage.Warning);
                        ConnectButton.IsEnabled = true;
                        ConnectButton.Content = "Conectar";
                    });
                    return;
                }

                // Solicitar conexão através da API
                var connectionResult = await _serverApi.RequestConnectionAsync(remoteId, _localId);

                Dispatcher.Invoke(() =>
                {
                    if (connectionResult.Success)
                    {
                        FooterStatusText.Text = $"Solicitação enviada para {remoteId}";
                        MessageBox.Show($"Solicitação de conexão enviada para {remoteId}.\n" +
                                      "Aguardando aprovação do usuário remoto.",
                                      "Solicitação Enviada", MessageBoxButton.OK, MessageBoxImage.Information);

                        // Salvar conexão se solicitado
                        if (SaveConnectionCheckBox.IsChecked == true)
                        {
                            SaveRecentConnection(remoteId);
                        }
                    }
                    else
                    {
                        FooterStatusText.Text = "Falha na solicitação de conexão";
                        MessageBox.Show($"Erro ao solicitar conexão:\n{connectionResult.Message}",
                                      "Erro de Conexão", MessageBoxButton.OK, MessageBoxImage.Error);
                    }

                    ConnectButton.IsEnabled = true;
                    ConnectButton.Content = "Conectar";
                    UpdateUI();
                });
            }
            catch (Exception ex)
            {
                Dispatcher.Invoke(() =>
                {
                    FooterStatusText.Text = "Erro na conexão";
                    MessageBox.Show($"Erro ao conectar:\n{ex.Message}",
                                  "Erro de Conexão", MessageBoxButton.OK, MessageBoxImage.Error);
                    ConnectButton.IsEnabled = true;
                    ConnectButton.Content = "Conectar";
                });
            }
        });
    }

    private void CopyIdButton_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            Clipboard.SetText(_localId);
            FooterStatusText.Text = "ID copiado para a área de transferência";

            // Feedback visual temporário
            CopyIdButton.Content = "✓ Copiado!";
            Task.Delay(2000).ContinueWith(t =>
            {
                Dispatcher.Invoke(() =>
                {
                    CopyIdButton.Content = "📋 Copiar";
                });
            });
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Erro ao copiar ID: {ex.Message}", "Erro",
                          MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void StartServerButton_Click(object sender, RoutedEventArgs e)
    {
        if (_isServerRunning)
        {
            // Parar servidor
            _isServerRunning = false;
            FooterStatusText.Text = "Servidor parado";
        }
        else
        {
            // Iniciar servidor
            _isServerRunning = true;
            FooterStatusText.Text = "Servidor iniciado - Aguardando conexões";
        }

        UpdateUI();
    }

    private void SaveSettingsButton_Click(object sender, RoutedEventArgs e)
    {
        // TODO: Implementar salvamento de configurações
        FooterStatusText.Text = "Configurações salvas";
        MessageBox.Show("Configurações salvas com sucesso!", "Configurações",
                      MessageBoxButton.OK, MessageBoxImage.Information);
    }

    private void RestoreDefaultsButton_Click(object sender, RoutedEventArgs e)
    {
        var result = MessageBox.Show("Deseja restaurar todas as configurações para os valores padrão?",
                                   "Restaurar Padrões", MessageBoxButton.YesNo, MessageBoxImage.Question);

        if (result == MessageBoxResult.Yes)
        {
            // Restaurar configurações padrão
            StartWithWindowsCheckBox.IsChecked = false;
            MinimizeToTrayCheckBox.IsChecked = true;
            ShowNotificationsCheckBox.IsChecked = true;
            ServerAddressTextBox.Text = "servidor.oliacesso.com";
            ServerPortTextBox.Text = "7070";
            QualityComboBox.SelectedIndex = 1;
            AdaptiveQualityCheckBox.IsChecked = true;
            RequirePasswordCheckBox.IsChecked = false;
            AccessPasswordTextBox.Text = "";
            AutoAcceptCheckBox.IsChecked = false;

            FooterStatusText.Text = "Configurações restauradas para os padrões";
        }
    }

    private void AboutButton_Click(object sender, RoutedEventArgs e)
    {
        MessageBox.Show("OliAcesso Remoto - Cliente\nVersão 1.0.0\n\nUm aplicativo de acesso remoto seguro e fácil de usar.\n\n© 2024 OliAcesso",
                      "Sobre", MessageBoxButton.OK, MessageBoxImage.Information);
    }

    // Helper Methods
    private bool IsValidId(string id)
    {
        // Validar formato XXX XXX XXX (números com espaços)
        var parts = id.Split(' ');
        if (parts.Length != 3) return false;

        foreach (var part in parts)
        {
            if (part.Length != 3 || !part.All(char.IsDigit))
                return false;
        }

        return true;
    }

    private void SaveRecentConnection(string remoteId)
    {
        // Verificar se já existe
        var existing = RecentConnections.FirstOrDefault(c => c.Id == remoteId);
        if (existing != null)
        {
            RecentConnections.Remove(existing);
        }

        // Adicionar no topo
        RecentConnections.Insert(0, new RecentConnection
        {
            Name = $"Computador {remoteId}",
            Id = remoteId
        });

        // Manter apenas os 10 mais recentes
        while (RecentConnections.Count > 10)
        {
            RecentConnections.RemoveAt(RecentConnections.Count - 1);
        }
    }

    private void OpenRemoteControlWindow(string remoteId)
    {
        // TODO: Implementar janela de controle remoto
        MessageBox.Show($"Abrindo sessão de controle remoto para {remoteId}\n\n" +
                       "Esta funcionalidade será implementada na próxima fase.",
                       "Controle Remoto", MessageBoxButton.OK, MessageBoxImage.Information);
    }

    // INotifyPropertyChanged implementation
    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}

// Model classes
public class RecentConnection
{
    public string Name { get; set; } = string.Empty;
    public string Id { get; set; } = string.Empty;
}