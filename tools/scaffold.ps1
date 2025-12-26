param(
  # Default is relative to the repository root
  [string]$ManifestPath = "tools/docs/scaffold/app-map.json",
  [switch]$Force
)

# Always anchor generated output at the repository root, no matter where the script is executed from.
# This prevents accidental generation under tools/GIPractice.Wpf when running from the tools folder.
$RepoRoot = Resolve-Path (Join-Path $PSScriptRoot "..")

function Resolve-RepoPath([string]$p) {
  if ([System.IO.Path]::IsPathRooted($p)) { return $p }
  return (Join-Path $RepoRoot $p)
}

function Write-File($relativePath, $content) {
  $path = Resolve-RepoPath $relativePath
  $dir = Split-Path $path -Parent
  if (!(Test-Path $dir)) { New-Item -ItemType Directory -Path $dir | Out-Null }

  if ((Test-Path $path) -and (-not $Force)) {
    Write-Host "SKIP (exists): $path"
    return
  }

  $content | Out-File -FilePath $path -Encoding utf8
  Write-Host "WRITE: $path"
}

$manifest = Get-Content (Resolve-RepoPath $ManifestPath) -Raw | ConvertFrom-Json
$ns = $manifest.rootNamespace

# Generate Views/Templates/AutoTemplates.xaml
$templates = @()
$templates += @"
<ResourceDictionary
  xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
  xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
  xmlns:vm="clr-namespace:$ns.ViewModels"
  xmlns:v="clr-namespace:$ns.Views">

"@

foreach ($f in $manifest.features) {
  $featureName = $f.name

  # Convention:
  # VM classes:
  #   Search:  <FeatureName>SearchViewModel
  #   Details: <FeatureName>DetailsViewModel
  # Views:
  #   Search:  Views/Search/<FeatureName>SearchPage.xaml (Page)
  #   Details: Views/Details/<FeatureName>DetailsView.xaml (UserControl)

  $vmSearch = "${featureName}SearchViewModel"
  $vmDetails = "${featureName}DetailsViewModel"
  $viewSearch = "${featureName}SearchPage"
  $viewDetails = "${featureName}DetailsView"

  $templates += "  <DataTemplate DataType=""{x:Type vm:$vmSearch}"">"
  $templates += "    <v:$viewSearch />"
  $templates += "  </DataTemplate>"
  $templates += ""
  $templates += "  <DataTemplate DataType=""{x:Type vm:$vmDetails}"">"
  $templates += "    <v:$viewDetails />"
  $templates += "  </DataTemplate>"
  $templates += ""
}

$templates += @"
</ResourceDictionary>
"@

Write-File "GIPractice.Wpf/Views/Templates/AutoTemplates.xaml" ($templates -join "`r`n")

foreach ($f in $manifest.features) {
  $featureName = $f.name
  $entityName = $f.entity

  # ------------------------
  # ViewModels
  # ------------------------
  $vmSearchPath = "GIPractice.Wpf/ViewModels/Search/$featureName" + "SearchViewModel.cs"
  $vmDetailsPath = "GIPractice.Wpf/ViewModels/Details/$featureName" + "DetailsViewModel.cs"

  $vmSearchContent = @"
using System.Collections.ObjectModel;

namespace $ns.ViewModels;

public sealed class ${featureName}SearchViewModel : ViewModelBase
{
    private string? _query;
    public string? Query
    {
        get => _query;
        set => SetProperty(ref _query, value);
    }

    public ObservableCollection<object> Items { get; } = new();

    public RelayCommand SearchCommand { get; }
    public RelayCommand NewCommand { get; }
    public RelayCommand<object> OpenCommand { get; }

    public ${featureName}SearchViewModel()
    {
        SearchCommand = new RelayCommand(Search);
        NewCommand = new RelayCommand(New);
        OpenCommand = new RelayCommand<object>(Open);
    }

    private void Search()
    {
        // TODO: call ${featureName}Module.SearchAsync(...)
        Items.Clear();
    }

    private void New()
    {
        // TODO: navigate/open details editor in create mode
    }

    private void Open(object item)
    {
        // TODO: item -> id -> load details
    }
}
"@

  $vmDetailsContent = @"
namespace $ns.ViewModels;

public sealed class ${featureName}DetailsViewModel : ViewModelBase
{
    private int? _id;
    public int? Id
    {
        get => _id;
        set => SetProperty(ref _id, value);
    }

    private object? _model;
    public object? Model
    {
        get => _model;
        set => SetProperty(ref _model, value);
    }

    public RelayCommand SaveCommand { get; }
    public RelayCommand DeleteCommand { get; }
    public RelayCommand CloseCommand { get; }

    public ${featureName}DetailsViewModel()
    {
        SaveCommand = new RelayCommand(Save);
        DeleteCommand = new RelayCommand(Delete);
        CloseCommand = new RelayCommand(Close);
    }

    public void Load(int id)
    {
        Id = id;
        // TODO: call ${featureName}Module.GetByIdAsync(...)
    }

    private void Save()
    {
        // TODO: create/update via API
    }

    private void Delete()
    {
        // TODO: delete via API
    }

    private void Close()
    {
        // TODO: navigation close
    }
}
"@

  Write-File $vmSearchPath $vmSearchContent
  Write-File $vmDetailsPath $vmDetailsContent

  # ------------------------
  # Views
  # ------------------------
  $viewSearchPath = "GIPractice.Wpf/Views/Search/$featureName" + "SearchPage.xaml"
  $viewSearchCodeBehindPath = "GIPractice.Wpf/Views/Search/$featureName" + "SearchPage.xaml.cs"
  $viewDetailsPath = "GIPractice.Wpf/Views/Details/$featureName" + "DetailsView.xaml"
  $viewDetailsCodeBehindPath = "GIPractice.Wpf/Views/Details/$featureName" + "DetailsView.xaml.cs"

  $viewSearchXaml = @"
<Page x:Class="$ns.Views.${featureName}SearchPage"
      xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
      xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
      xmlns:d="http://schemas.microsoft.com/expression/blend/2008"
      xmlns:mc="http://schemas.openxmlformats.org/markup-compatibility/2006"
      mc:Ignorable="d"
      Title="$featureName Search">

    <Grid Margin="10">
        <Grid.RowDefinitions>
            <RowDefinition Height="Auto"/>
            <RowDefinition Height="*"/>
        </Grid.RowDefinitions>

        <DockPanel Grid.Row="0" Margin="0,0,0,10">
            <TextBox Width="300"
                     Text="{Binding Query, UpdateSourceTrigger=PropertyChanged}"
                     ToolTip="Search query..." />
            <Button Margin="10,0,0,0" Content="Search" Command="{Binding SearchCommand}" />
            <Button Margin="10,0,0,0" Content="New" Command="{Binding NewCommand}" />
        </DockPanel>

        <DataGrid Grid.Row="1"
                  ItemsSource="{Binding Items}"
                  AutoGenerateColumns="True"
                  IsReadOnly="True" />
    </Grid>
</Page>
"@

  $viewSearchCodeBehind = @"
using System.Windows.Controls;

namespace $ns.Views;

public partial class ${featureName}SearchPage : Page
{
    public ${featureName}SearchPage()
    {
        InitializeComponent();
    }
}
"@

  $viewDetailsXaml = @"
<UserControl x:Class="$ns.Views.${featureName}DetailsView"
      xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
      xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
      xmlns:d="http://schemas.microsoft.com/expression/blend/2008"
      xmlns:mc="http://schemas.openxmlformats.org/markup-compatibility/2006"
      mc:Ignorable="d">

    <Grid Margin="10">
        <Grid.RowDefinitions>
            <RowDefinition Height="Auto"/>
            <RowDefinition Height="*"/>
        </Grid.RowDefinitions>

        <DockPanel Grid.Row="0" Margin="0,0,0,10">
            <Button Content="Save" Command="{Binding SaveCommand}" />
            <Button Margin="10,0,0,0" Content="Delete" Command="{Binding DeleteCommand}" />
            <Button Margin="10,0,0,0" Content="Close" Command="{Binding CloseCommand}" />
        </DockPanel>

        <TextBlock Grid.Row="1"
                   Text="TODO: ${featureName} editor UI"
                   FontSize="18"
                   VerticalAlignment="Center"
                   HorizontalAlignment="Center"
                   Opacity="0.7"/>
    </Grid>
</UserControl>
"@

  $viewDetailsCodeBehind = @"
using System.Windows.Controls;

namespace $ns.Views;

public partial class ${featureName}DetailsView : UserControl
{
    public ${featureName}DetailsView()
    {
        InitializeComponent();
    }
}
"@

  Write-File $viewSearchPath $viewSearchXaml
  Write-File $viewSearchCodeBehindPath $viewSearchCodeBehind
  Write-File $viewDetailsPath $viewDetailsXaml
  Write-File $viewDetailsCodeBehindPath $viewDetailsCodeBehind

  # ------------------------
  # Client module stubs (optional)
  # ------------------------
  $clientPath = "GIPractice.Client/${featureName}Module.cs"
  $clientContent = @"
using System.Threading;
using System.Threading.Tasks;

namespace GIPractice.Client;

public interface I${featureName}Module
{
    Task<object> SearchAsync(object request, CancellationToken cancellationToken = default);
    Task<object?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<int> CreateAsync(object dto, CancellationToken cancellationToken = default);
    Task UpdateAsync(int id, object dto, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
}

// TODO: implement using GiPracticeApiClient like your PatientsModule pattern
"@
  Write-File $clientPath $clientContent
}

Write-Host ""
Write-Host "DONE. Now merge AutoTemplates.xaml into App.xaml."
