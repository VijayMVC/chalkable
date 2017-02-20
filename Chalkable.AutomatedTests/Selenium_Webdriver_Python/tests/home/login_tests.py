from selenium import webdriver
import os
import unittest
from Selenium_Webdriver_Python.pages.home.login_page import LoginPage
from Selenium_Webdriver_Python.utilities.teststatus import StatusTest #!
import pytest

@pytest.mark.usefixtures("oneTimeSetUp", "setUp")
class LoginTests(unittest.TestCase):

    @pytest.fixture(autouse=True)
    def classSetup(self, oneTimeSetUp):
        self.lp = LoginPage(self.driver)
        self.ts = StatusTest(self.driver)

    @pytest.mark.run(order=1)
    def test_validLogin(self):
        # baseURL = "https://dev.chalkable.com/"
        # driverLocation = r'C:\Python34\additional_libs\chromedriver_win32\chromedriver.exe'
        # os.environ["webdriver.chrome.driver"] = driverLocation
        # driver = webdriver.Chrome(driverLocation)
        # driver.maximize_window()
        # driver.implicitly_wait(3)
        # lp = LoginPage(driver)
        # driver.get(baseURL)

        self.lp.login("teacher@chalkable.com", "qqqq1111")

        result1 = self.lp.verifyLoginTitle()
        self.ts.mark(result1, "Title Verified") #!
        #assert result1 == True
        result2 = self.lp.verifyLoginSuccessful()
        #assert result2 == True
        self.ts.markFinal("test_validLogin", result2, "Login was successful") #!


        #driver.quit()

    @pytest.mark.run(order=1)
    def test_invalidLogin(self):
        # baseURL = "https://dev.chalkable.com/"
        # driverLocation = r'C:\Python34\additional_libs\chromedriver_win32\chromedriver.exe'
        # os.environ["webdriver.chrome.driver"] = driverLocation
        # driver = webdriver.Chrome(driverLocation)
        # driver.maximize_window()
        # driver.implicitly_wait(3)
        # lp = LoginPage(driver)
        # driver.get(baseURL)

        self.lp.login("teacher@chalkable.com", "")

        result = self.lp.verifyLoginFailed('Chalkable Classroom')
        assert result == True

        self.driver.find_element_by_name('UserName').clear()
        self.driver.find_element_by_name('Password').clear()

        #driver.quit()