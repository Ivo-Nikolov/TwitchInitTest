using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Support.UI;

namespace SeleniumTests
{
    [TestFixture]
    public class TestTrySelenium
    {
        private IWebDriver driver;
        private StringBuilder verificationErrors;
        private string baseURL;
        private bool acceptNextAlert = true;

        [SetUp]
        public void SetupTest()
        {
            driver = new FirefoxDriver();
            baseURL = "https://www.twitch.tv";
            verificationErrors = new StringBuilder();
        }

        [TearDown]
        public void TeardownTest()
        {
            try
            {
                driver.Quit();
            }
            catch (Exception)
            {
                // Ignore errors if unable to close the browser
            }
            Assert.AreEqual("", verificationErrors.ToString());
        }

        [Test]
        public void TheTrySeleniumTest()
        {
            driver.Navigate().GoToUrl(baseURL + "/");

            bool isLoginButtonPresent = IsElementPresent(By.CssSelector("#header_login > span"));
            if (isLoginButtonPresent)
            {
                driver.FindElement(By.CssSelector("#header_login > span")).Click();

                //waiting a little till the page is loaded
                var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(60));

                wait.Until(ExpectedConditions.VisibilityOfAllElementsLocatedBy(By.ClassName("noty_close")));
                //closing the cookie message
                driver.FindElement(By.ClassName("noty_close")).Click();

                //switch to login iframe
                driver.SwitchTo().Frame(driver.FindElement(By.Name("passport")));

                driver.FindElement(By.Name("password")).Clear();
                driver.FindElement(By.Name("password")).SendKeys("testtry1");
                driver.FindElement(By.Id("username")).Clear();
                driver.FindElement(By.Id("username")).SendKeys("testSelenium");

                driver.FindElement(By.XPath("//button[@type='submit']")).Click();

                driver.SwitchTo().DefaultContent();

                //check if the user is actually logged
                //Following link is presented only for logged users
                bool isUserLogged = IsElementPresent(By.Id("header_following"));
                if (isUserLogged)
                {
                    //find the following link
                    driver.FindElement(By.Id("header_following")).Click();
                    //still some problems with timeout 
                    wait.Until(ExpectedConditions.VisibilityOfAllElementsLocatedBy(By.ClassName("game-item")));
               
                    ICollection<IWebElement> diabloGameElements = driver.
                        FindElements(By.XPath("//a[@title = 'Diablo II: Lord of Destruction']"));

                    Assert.IsTrue(diabloGameElements.Count == 1, "Only 1 game diablo is present");

                    diabloGameElements.FirstOrDefault().Click();

                    //find all live streams now
                    ICollection<IWebElement> liveStreams = driver.FindElements(
                        By.XPath("//div[@class='stream item']//p[@class='info']/a"));

                    //find if the streamer uncapable has a live stream now
                    IWebElement streamerUncapable =
                        liveStreams.FirstOrDefault(e => e.Text == "Uncapable");
                    Assert.IsNotNull(streamerUncapable,"Uncapable is online");

                }
                else
                {
                    Assert.Inconclusive("The user doesnt exist or the password doesn't match");
                }
            }
            else
            {
                Assert.Inconclusive("The login button is changed or gone missing");
            }
        }
        private bool IsElementPresent(By by)
        {
            try
            {
                driver.FindElement(by);
                return true;
            }
            catch (NoSuchElementException)
            {
                return false;
            }
        }

        private bool IsAlertPresent()
        {
            try
            {
                driver.SwitchTo().Alert();
                return true;
            }
            catch (NoAlertPresentException)
            {
                return false;
            }
        }

        private string CloseAlertAndGetItsText()
        {
            try
            {
                IAlert alert = driver.SwitchTo().Alert();
                string alertText = alert.Text;
                if (acceptNextAlert)
                {
                    alert.Accept();
                }
                else
                {
                    alert.Dismiss();
                }
                return alertText;
            }
            finally
            {
                acceptNextAlert = true;
            }
        }
    }
}

