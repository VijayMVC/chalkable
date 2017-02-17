from selenium.webdriver.common.by import By
import time
#from Selenium_Webdriver_Python.base.selenium_driver import SeleniumDriver
import Selenium_Webdriver_Python.utilities.custom_logger as cl
import logging
from Selenium_Webdriver_Python.base.basepage import BasePage

class LoginPage(BasePage):

    log = cl.customLogger(logging.DEBUG)

    def __init__(self, driver):
        super().__init__(driver)
        self.driver = driver

    # locators
    _email_field = 'UserName'
    _password_field = 'Password'
    _login_button = "//div[@id='login-form']//div[@class='form-group']//button[contains(text(),'LOGIN')]"

    def enterEmail(self, email):
        self.sendKeys(email, self._email_field, locatorType="name")

    def enterPassword(self, password):
        self.sendKeys(password, self._password_field, locatorType="name")

    def clickLoginButton(self):
        self.elementClick(self._login_button, locatorType="xpath")

    def login(self, email="", password=""):
        self.enterEmail(email)
        self.enterPassword(password)
        self.clickLoginButton()

        time.sleep(15)

    def verifyLoginSuccessful(self):
        #result = self.isTitlePresent(title)
        #return result
        result = self.isElementPresent("//div[@id='page']//button[@class='action-button blue notifications-link']",
                                       locatorType="xpath")
        return result

    def verifyLoginFailed(self, title):
        result = self.isTitlePresent(title)
        #result = self.isElementPresent('Password-error')
        return result

    # result = self.isElementPresent("//div[contains(text(),'Invalid email or password')]",
    #                                locatorType="xpath")

    def verifyLoginTitle(self):
        # if self.getTitle() == title:
        #     return True
        # else:
        #     return False
        return self.verifyPageTitle('Teacher')