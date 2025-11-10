// See https://aka.ms/new-console-template for more information
using OpenQA.Selenium.Chrome;

Console.WriteLine("Hello, World!");
var driver = new ChromeDriver();
driver.Navigate().GoToUrl("https://www.linkedin.com/jobs/?focusToMoreMenuTrigger=true");
Console.ReadKey();
driver.Quit();