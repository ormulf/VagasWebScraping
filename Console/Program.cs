// See https://aka.ms/new-console-template for more information
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using static System.Net.Mime.MediaTypeNames;

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

        driver.Navigate().GoToUrl("https://www.linkedin.com/jobs/");

        // Espera o carregamento da página
        Thread.Sleep(5000);
        //Console.WriteLine(driver.PageSource);

        var i = 0;
        var iframes = driver.FindElements(By.TagName("iframe"));
        foreach ( var iframe in iframes )
        {
            Console.WriteLine(iframe.GetAttribute("src"));
            driver.SwitchTo().Frame(iframe);
            Thread.Sleep(5000);
            IWebElement searchInput = GetSearchBoxInput(driver);

            var inputs = driver.FindElements(By.TagName("inputs"));
            Console.WriteLine("-------" + driver.PageSource);
            
            foreach ( var input in inputs)
            {
                Console.WriteLine("-------" + input.GetAttribute("label"));
            }
            i++;
            driver.SwitchTo().DefaultContent();
        }

        



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

IWebElement GetSearchBoxInput(ChromeDriver driver)
{
    try
    {
        var inputs = driver.FindElements(By.TagName("input"));
        foreach (var input in inputs)
        {
            var inputLabel = input.GetAttribute("label");
            if(inputLabel == "Pesquisar cargo, competência ou empresa")
            {
                return input;
            }
        }
    }
    catch (Exception)
    {
        Console.WriteLine($"SearchBox não encontrada");
        throw;
    }
    return null;
}

Console.ReadKey();
//driver.Quit();