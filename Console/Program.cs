// See https://aka.ms/new-console-template for more information
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

Console.WriteLine("Hello, World!");
//string userDataDirPath = @"C:\Users\ormul\AppData\Local\Google\Chrome\User Data";

//ChromeOptions options = new ChromeOptions();
//options.AddArgument($"--user-data-dir={userDataDirPath}");
//options.AddArgument("--profile-directory=Default"); // especifica o perfil
//options.AddExcludedArgument("enable-automation");   // remove aviso "automated browser"
//options.AddAdditionalOption("useAutomationExtension", false);
//var driver = new ChromeDriver(options);
//driver.Navigate().GoToUrl("https://www.linkedin.com/jobs/?focusToMoreMenuTrigger=true");

try
{
    // Caminho da pasta de usuário do Chrome
    string userDataDirPath = @"C:\Users\ormul\AppData\Local\Google\Chrome\User Data";

    // Verifica se o caminho existe
    if (!Directory.Exists(userDataDirPath))
    {
        throw new DirectoryNotFoundException($"Caminho não encontrado: {userDataDirPath}");
    }

    // Configurações do Chrome
    var options = new ChromeOptions();
    options.AddArgument($"--user-data-dir={userDataDirPath}");
    options.AddArgument("--profile-directory=Default"); // ou "Profile 1", "Profile 2", etc.
    options.AddArgument("--disable-blink-features=AutomationControlled");
    options.AddArgument("--start-maximized");

    // Remove aviso "Chrome está sendo controlado..."
    options.AddExcludedArgument("enable-automation");
    options.AddAdditionalOption("useAutomationExtension", false);

    // Instancia o ChromeDriver
    using (var driver = new ChromeDriver(options))
    {
        Console.WriteLine("Chrome iniciado. Aguardando página...");

        driver.Navigate().GoToUrl("https://www.linkedin.com/jobs/?focusToMoreMenuTrigger=true");

        // Espera o carregamento da página
        Thread.Sleep(5000);

        Console.WriteLine("Página carregada. Verifique se o login foi mantido.");
        Console.WriteLine("Pressione ENTER para encerrar...");
        Console.ReadLine();
    }
}
catch (WebDriverException ex)
{
    Console.WriteLine($"Erro do Selenium: {ex.Message}");
    if (ex.Message.Contains("another browser instance"))
        Console.WriteLine("❌ Feche todas as janelas do Chrome antes de rodar o Selenium.");
}
catch (Exception ex)
{
    Console.WriteLine($"Erro geral: {ex.Message}");
}

Console.ReadKey();
//driver.Quit();