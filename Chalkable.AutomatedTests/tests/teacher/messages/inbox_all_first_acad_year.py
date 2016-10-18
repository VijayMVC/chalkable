from base_auth_test import *
import unittest

class TestSelectingFirstAcadYear(BaseTestCase):
    def setUp(self):
        self.teacher = TeacherSession(self).login(user_email, user_pwd)
        self.first_year = self.teacher.list_of_years()[0]

    def internal_(self):
        get_messages_inbox_all = self.teacher.get_json('/PrivateMessage/List.json?' + 'start=' + str(0) + '&count=' + str(10) + '&income=' + str(True) + '&role=' + '&classOnly=' + str(False) + '&acadYear=' + str(self.first_year))

    def test_messages_from_first_acad_year(self):
        self.internal_()

if __name__ == '__main__':
    unittest.main()