using Infrastructure.Database;
using Infrastructure;
using Microsoft.Extensions.DependencyInjection;
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
using TextGeneration;
using Microsoft.EntityFrameworkCore;
using System.Configuration;
using Gui.Services;

namespace Gui
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public IServiceProvider ServiceProvider { get; private set; }
		
		public MainWindow()
		{
			ServiceCollection services = new();
			services.AddSingleton<Generator>();
			services.AddSingleton<IWriter, ToFileWriter>();
			services.AddDbContext<AppDbContext>(options =>
				options.UseSqlServer(
					ConfigurationManager.AppSettings["DefaultConnection"],
					x => x.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName)
				)
			);
			services.AddTransient<IDataRepository, DataRepository>();
			services.AddTransient<FileManipulator>();

			services.AddScoped<DialogService>();
			services.AddSingleton<MainWindowViewModel>();

			Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
			ServiceProvider = services.BuildServiceProvider();

			services.AddScoped<ServiceProvider>();
			MainWindowViewModel viewModel = ServiceProvider.GetRequiredService<MainWindowViewModel>();
			DataContext = viewModel;
			//InitializeComponent();
		}
	}
}