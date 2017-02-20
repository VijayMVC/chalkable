import pytest
from selenium import webdriver
import os
from Selenium_Webdriver_Python.base.webdriverfactory import WebDriverFactory
#from Selenium_Webdriver_Python.pages.home.login_page import LoginPage

@pytest.yield_fixture()
def setUp():
    print("Running method level setUp")
    yield
    print("Running method level tearDown")


@pytest.yield_fixture(scope="class")
def oneTimeSetUp(request, browser):
    print("Running one time setUp")
    wdf = WebDriverFactory(browser)
    driver = wdf.getWebDriverInstance()

    #lp = LoginPage(driver) #!?
    #lp.login("teacher@chalkable.com", "qqqq1111") #!?

    # if browser == "firefox":
        # baseURL = "https://dev.chalkable.com/"
        # driverLocation = r'C:\Python34\additional_libs\chromedriver_win32\chromedriver.exe'
        # os.environ["webdriver.chrome.driver"] = driverLocation
        # driver = webdriver.Chrome(driverLocation)
        # driver.maximize_window()
        # driver.implicitly_wait(3)
        # driver.get(baseURL)
        # print("Running tests on firefox")
    # else:
    #     baseURL = "https://dev.chalkable.com/"
    #     driverLocation = r'C:\Python34\additional_libs\chromedriver_win32\chromedriver.exe'
    #     os.environ["webdriver.chrome.driver"] = driverLocation
    #     driver = webdriver.Chrome(driverLocation)
    #     driver.get(baseURL)
    #     print("Running tests on chrome")

    if request.cls is not None:
        request.cls.driver = driver

    yield driver

    driver.quit()

    print("Running one time tearDown")

def pytest_addoption(parser):
    parser.addoption("--browser")
    parser.addoption("--osType", help="Type of operating system")

@pytest.fixture(scope="session")
def browser(request):
    return request.config.getoption("--browser")

@pytest.fixture(scope="session")
def osType(request):
    return request.config.getoption("--osType")