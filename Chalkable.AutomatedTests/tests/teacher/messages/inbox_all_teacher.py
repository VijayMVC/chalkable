from base_auth_test import *
import unittest

class TestTeacherMessagesInbox(BaseTestCase):
    def setUp(self):
        self.teacher = TeacherSession(self).login(user_email, user_pwd)

    def internal_(self):
        get_messages_inbox_all = self.teacher.get_json('/PrivateMessage/List.json?' + 'start=' + str(0) + '&count=' + str(10) + '&income=' + str(True) + '&role=' + str('teacher') + '&classOnly=' + str(False))

    def test_inbox_teacher(self):
        self.internal_()

if __name__ == '__main__':
    unittest.main()