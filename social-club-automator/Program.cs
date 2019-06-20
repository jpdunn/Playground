using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;

namespace SocialClubAutomator {
    class Program {

        private readonly List<string> removalNames = new List<string> {
            ""
        };

        static void Main(string[] args) {
            IWebElement emailInput;
            IWebElement passwordInput;


            IWebDriver driver = new ChromeDriver();
            driver.Url = "https://signin.rockstargames.com/signin/user-form?cid=socialclub";
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);

            emailInput = driver.FindElement(By.Name("email"));
            passwordInput = driver.FindElement(By.Name("password"));

            if (emailInput != null) {
                IWebElement reCaptcha;

                // ENTER EMAIL AND PASSWORD HERE.
                emailInput.SendKeys("");
                passwordInput.SendKeys("");
                driver.FindElement(By.ClassName("loginform__submit")).Click();
                driver.Manage().Window.Maximize();


                // Check if we have an iframe for the reCaptcha challenge.
                reCaptcha = driver.FindElement(By.TagName("iframe"));

                if (reCaptcha != null) {
                    IWebElement avatar;

                    WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromMinutes(5));
                    Func<IWebDriver, bool> waitForElement = new Func<IWebDriver, bool>((IWebDriver Web) => {
                        try {
                            Web.FindElement(By.CssSelector("[data-ui-name=\"avatar\"]"));
                            return true;
                        } catch (Exception) {

                            return false;
                        }
                    });
                    wait.Until(waitForElement);

                    // If we have a recaptcha, wait 5 minutes to allow for manual input. There's no way around it
                    // and I guess that's the point of recaptcha.
                    driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromMinutes(5);

                    avatar = driver.FindElement(By.CssSelector("[data-ui-name=\"avatar\"]"));

                    if (avatar != null) {
                        IWebElement friendsButton;


                        avatar.Click();

                        friendsButton = driver.FindElement(By.CssSelector("[data-ui-name=\"friends\"]"));

                        if (friendsButton != null) {
                            IReadOnlyCollection<IWebElement> friends;


                            friendsButton.Click();

                            WebDriverWait friendsListWait = new WebDriverWait(driver, TimeSpan.FromMinutes(5));
                            Func<IWebDriver, bool> waitForFriendsList = new Func<IWebDriver, bool>((IWebDriver Web) => {
                                try {
                                    Web.FindElement(By.CssSelector("[data-ui-name=\"myFriends\"]"));
                                    return true;
                                } catch (Exception) {

                                    return false;
                                }
                            });

                            friendsListWait.Until(waitForFriendsList);

                            friends = driver.FindElements(By.CssSelector("[data-ui-name=\"myFriends\"]"));

                            if (friends.Count > 0) {
                                IWebElement loadMoreElement;


                                loadMoreElement = driver.FindElement(By.ClassName("Friends__loadingWrap__2USIi"));

                                do {
                                    loadMoreElement.FindElement(By.TagName("button")).Click();

                                    WebDriverWait loadMoreWait = new WebDriverWait(driver, TimeSpan.FromMinutes(5));
                                    Func<IWebDriver, bool> waitForLoadMore = new Func<IWebDriver, bool>((IWebDriver Web) => {
                                        try {
                                            Web.FindElement(By.ClassName("Friends__loadingWrap__2USIi"));
                                            return true;
                                        } catch (Exception) {

                                            return false;
                                        }
                                    });
                                    loadMoreWait.Until(waitForLoadMore);
                                    loadMoreElement = driver.FindElement(By.ClassName("Friends__loadingWrap__2USIi"));


                                } while (loadMoreElement != null);

                                IWebElement friend;


                                // Enter friend name here.
                                friend = driver.FindElement(By.CssSelector("[data-ui-name=\"\"]"));

                                if (friend != null) {

                                }
                            }
                        }

                    }
                }
            }
        }
    }
}
