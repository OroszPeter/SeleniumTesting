using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using System;
using System.Threading;

namespace SeleniumTests
{
    [TestClass]
    public class UnitTest1
    {
        private IWebDriver driver;

        [TestInitialize]
        public void Setup()
        {
            driver = new ChromeDriver();
            driver.Manage().Window.Maximize();
        }

        [TestMethod]
        public void TestCommentSubmissionWithRating()
        {
            // Böngésző indítása és bejelentkezés oldala
            driver.Navigate().GoToUrl("https://bibliothecamotusimaginibus.netlify.app/login"); // A bejelentkezési oldal URL-je
            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));

            // Bejelentkezési mezők várakozása
            var usernameField = wait.Until(ExpectedConditions.ElementIsVisible(By.Id("username")));
            var passwordField = wait.Until(ExpectedConditions.ElementIsVisible(By.Id("password")));
            var loginButton = wait.Until(ExpectedConditions.ElementToBeClickable(By.Id("login")));

            // Bejelentkezési adatok kitöltése
            usernameField.SendKeys("tester");
            passwordField.SendKeys("tester123");

            // Bejelentkezés gombra kattintás
            loginButton.Click();

            // Várakozás a sikeres bejelentkezésre és az átirányításra a főoldalra
            wait.Until(ExpectedConditions.UrlContains("/"));

            // Keresőmezőre várakozás és interakció
            var searchBox = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("input[type='search']")));
            searchBox.SendKeys("Breaking Bad");

            // Keresés gombra kattintás
            var searchButton = wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("button[type='button']")));
            searchButton.Click();

            // Várakozás, hogy az oldal betöltődjön és az URL tartalmazza a keresési kifejezést
            wait.Until(ExpectedConditions.UrlContains("https://bibliothecamotusimaginibus.netlify.app/result?query=Breaking%20Bad"));

            // Várakozás, hogy az első film linkje látható és kattintható legyen
            var firstMovieLink = wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector(".image-link")));
            firstMovieLink.Click();

            // Várakozás, hogy a film oldal betöltődjön
            wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector(".container.pt-5.w-75")));

            // Értékelés szimulálása (például 3 csillag)
            var stars = wait.Until(d => d.FindElements(By.CssSelector(".star-rating .star"))); // Várakozás a csillagok elemére
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(stars.Count > 0, "Nem találhatóak a csillagok.");

            stars[2].Click(); // A harmadik csillag kiválasztása (3-as értékelés)

            // Komment mezőre és a gombra várakozás
            var commentBox = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("#comment")));
            var submitButton = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("#send")));

            // Görgetés a gombhoz, hogy ne legyen akadályozva
            IJavaScriptExecutor jsExecutor = (IJavaScriptExecutor)driver;
            jsExecutor.ExecuteScript("arguments[0].scrollIntoView(true);", submitButton);

            // Várakozás, hogy a gomb kattintható legyen
            wait.Until(ExpectedConditions.ElementToBeClickable(submitButton));

            // Komment beírása
            commentBox.SendKeys("Ez egy teszt komment!");

            // Komment gombra kattintás
            submitButton.Click();

            // Várakozás a komment és értékelés frissülésére
            Thread.Sleep(2000);  // Várakozás, hogy a komment és értékelés frissüljön

            // Ellenőrizze, hogy a komment megjelenik-e az értékelések között
            var ratingSection = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector(".ratings-section")));
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(ratingSection.Text.Contains("Ez egy teszt komment!"), "A komment nem jelent meg.");

            // Ellenőrizze, hogy a csillagos értékelés (3 csillag) megjelenik-e
            var ratingStars = ratingSection.FindElements(By.CssSelector(".star"));
            int filledStars = 0;
            foreach (var star in ratingStars)
            {
                if (star.Text == "★") filledStars++;
            }

            // Kiírja a csillagos értékelést
            Console.WriteLine($"A csillagos értékelés: {filledStars} csillag");

            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(3, filledStars, "A csillagos értékelés nem egyezik a várt értékkel.");
        }

        [TestCleanup]
        public void TearDown()
        {
            driver.Quit();
        }
    }
}
