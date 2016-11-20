from base_auth_test import *
import unittest

class TestDoesntSendMessageToForeignStudent(BaseTestCase):
    def setUp(self):
        self.admin = DistrictAdminSession(self).login(user_email, user_pwd)

        update_messaging_settings = self.admin.get_json('/School/UpdateMessagingSettings.json?' + 'studentMessaging=' + str(True) +
            '&studentToClassOnly=' + str(False) + '&teacherToStudentMessaging=' + str(True) + '&teacherToClassOnly=' + str(True))

        self.teacher = TeacherSession(self).login(user_email, user_pwd)

    def internal_(self):
        # creating 1 message for the student
        post_send = self.teacher.post_json('/PrivateMessage/Send.json', data={"body": "this is a body", "personId": 3765, "subject": "this is a subject"}, success=False) #student TRACEY BURRIS

    def test_test_sending_message_to_foreign_student(self):
        self.internal_()

    def tearDown(self):
        update_messaging_settings = self.admin.get_json(
            '/School/UpdateMessagingSettings.json?' + 'studentMessaging=' + str(True) +
            '&studentToClassOnly=' + str(False) + '&teacherToStudentMessaging=' + str(
                True) + '&teacherToClassOnly=' + str(False))

if __name__ == '__main__':
    unittest.main()