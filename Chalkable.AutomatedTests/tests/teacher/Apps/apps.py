from base_auth_test import *
import unittest


class TestFeed(unittest.TestCase):

    def setUp(self):
        self.teacher = TeacherSession(self).login(user_email, user_pwd)

    def test_my_apps(self):
        get_apps = self.teacher.get_json('/Application/MyApps.json?' + 'start=' + str(0) + '&count=' + str(1000))

if __name__ == '__main__':
    unittest.main()