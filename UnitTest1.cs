using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using System;
using System.Linq;
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
            driver.Navigate().GoToUrl("https://bibliothecamotusimaginibus.netlify.app/login");
            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));

            // Wait for login fields
            var usernameField = wait.Until(ExpectedConditions.ElementIsVisible(By.Id("username")));
            var passwordField = wait.Until(ExpectedConditions.ElementIsVisible(By.Id("password")));
            var loginButton = wait.Until(ExpectedConditions.ElementToBeClickable(By.Id("login")));

            // Fill in credentials
            usernameField.SendKeys("tester");
            passwordField.SendKeys("tester123");

            // Click login using JavaScript to avoid UI issues
            IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
            js.ExecuteScript("arguments[0].click();", loginButton);

            // Wait for the main page to load and verify login success
            wait.Until(ExpectedConditions.UrlContains("/"));
            Thread.Sleep(2000); // Small delay to allow login process completion

            // Search for a movie
            var searchBox = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("input[type='search']")));
            searchBox.SendKeys("Végzetes mélypont");

            var searchButton = wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("button[type='button']")));
            searchButton.Click();

            wait.Until(ExpectedConditions.UrlContains("https://bibliothecamotusimaginibus.netlify.app/result?query=V%C3%A9gzetes%20m%C3%A9lypont"));

            var firstMovieLink = wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector(".image-link")));
            firstMovieLink.Click();

            wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector(".container.pt-5.w-75")));

            // Simulate rating selection (3 stars)
            var stars = wait.Until(d => d.FindElements(By.CssSelector(".star-rating .star")));
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(stars.Count > 0, "Stars not found.");

            // Scroll down to make sure stars and comment box are visible
            js.ExecuteScript("arguments[0].scrollIntoView(true);", stars[0]); // Scroll to the first star
            wait.Until(ExpectedConditions.ElementToBeClickable(stars[2])).Click(); // Click third star (rating)

            // Now scroll down to the comment box and submit button
            var commentBox = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("#comment")));
            var submitButton = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("#send")));

            // Scroll to the submit button
            js.ExecuteScript("arguments[0].scrollIntoView(true);", submitButton);

            // Wait for any overlay to disappear (optional, in case it's a temporary message)
            Thread.Sleep(3000); // Give some time for any overlay to disappear

            // Remove any overlay if it's still visible using JavaScript (optional, try this if overlay persists)
            js.ExecuteScript("var overlay = document.querySelector('div[style*=\"position: fixed\"][style*=\"background-color: green\"]'); if (overlay) overlay.style.display = 'none';");


            // Submit comment
            commentBox.SendKeys("Ez egy teszt komment!");
            submitButton.Click();

            Thread.Sleep(2000);  // Wait for comment to be posted

            // Verify comment appears
            var ratingSection = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector(".ratings-section")));
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(ratingSection.Text.Contains("Ez egy teszt komment!"), "Comment not found in ratings.");

            // Verify rating appears correctly
            var ratingStars = ratingSection.FindElements(By.CssSelector(".star"));
            int filledStars = ratingStars.Count(star => star.Text == "★");

            Console.WriteLine($"Star rating detected: {filledStars} stars");
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(3, filledStars, "Star rating does not match expected value.");
        }





        [TestCleanup]
        public void TearDown()
        {
            driver.Quit();
        }
    }
}
