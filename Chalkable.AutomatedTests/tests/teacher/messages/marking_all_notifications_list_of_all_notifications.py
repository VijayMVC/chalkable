from base_auth_test import *
import unittest

class TestMarkingAllNotificationsAsRead(BaseTestCase):
    def setUp(self):
        self.teacher = TeacherSession(self).login(user_email, user_pwd)

    def internal_(self):
        mark_all_notifications = self.teacher.get_json('/Notification/MarkAllAsShown.json?' + 'start=' + str(0) + '&count=' + str(5))

    def test_marking_notifications(self):
        self.internal_()

if __name__ == '__main__':
    unittest.main()