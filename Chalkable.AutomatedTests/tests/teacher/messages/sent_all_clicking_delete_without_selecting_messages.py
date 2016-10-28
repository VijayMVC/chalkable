from base_auth_test import *
import unittest

class TestClickingDeleteButton(BaseTestCase):
    def setUp(self):
        self.teacher = TeacherSession(self).login(user_email, user_pwd)

    def internal_(self):
        post_delete = self.teacher.post_json('/PrivateMessage/Delete.json?', data={"ids": "", "income": False})

    def test_clicking_delete_button(self):
        self.internal_()

if __name__ == '__main__':
    unittest.main()