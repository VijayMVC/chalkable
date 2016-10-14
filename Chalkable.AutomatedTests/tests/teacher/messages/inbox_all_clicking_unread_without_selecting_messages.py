from base_auth_test import *
import unittest

class TestClickingUnreadButton(BaseAuthedTestCase):
    def setUp(self):
        self.teacher = TeacherSession(self).login(user_email, user_pwd)

    def internal_(self):
            post_read = self.teacher.post_json('/PrivateMessage/MarkAsRead.json?', data={"ids": "", "read": False})

    def test_clicking_unread_button(self):
        self.internal_()

if __name__ == '__main__':
    unittest.main()