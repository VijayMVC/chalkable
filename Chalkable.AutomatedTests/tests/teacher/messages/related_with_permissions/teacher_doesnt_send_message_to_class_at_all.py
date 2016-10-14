from base_auth_test import *
import unittest

class TestTeacherSendingMessages(BaseTestCase):
    def setUp(self):
        self.admin = DistrictAdminSession(self).login(user_email, user_pwd)

        update_messaging_settings = self.admin.get_json('/School/UpdateMessagingSettings.json?' + 'studentMessaging=' + str(True) +
            '&studentToClassOnly=' + str(False) + '&teacherToStudentMessaging=' + str(False) + '&teacherToClassOnly=' + str(False))

        self.teacher = TeacherSession(self).login(user_email, user_pwd)

    def internal_(self):
        # creating 1 message for the student
        post_send = self.teacher.post_json('/PrivateMessage/Send.json', data={"body": "this is a body", "classId": 13861, "subject": "this is a subject"}, success=False)

    def test_teacher_doesnt_send_message(self):
        self.internal_()

    def tearDown(self):
        update_messaging_settings = self.admin.get_json(
            '/School/UpdateMessagingSettings.json?' + 'studentMessaging=' + str(True) +
            '&studentToClassOnly=' + str(False) + '&teacherToStudentMessaging=' + str(
                True) + '&teacherToClassOnly=' + str(False))

if __name__ == '__main__':
    unittest.main()