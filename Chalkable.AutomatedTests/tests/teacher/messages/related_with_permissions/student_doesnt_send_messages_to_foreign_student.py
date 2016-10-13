from base_auth_test import *
import unittest

class TestStudentSendingMessages(BaseTestCase):
    def setUp(self):
        self.admin = DistrictAdminSession(self).login(user_email, user_pwd)

        update_messaging_settings = self.admin.get_json('/School/UpdateMessagingSettings.json?' + 'studentMessaging=' + str(True) +
            '&studentToClassOnly=' + str(True) + '&teacherToStudentMessaging=' + str(True) + '&teacherToClassOnly=' + str(False))

        self.student = StudentSession(self).login(user_email_student, user_pwd_student)

    def internal_(self, person_id):
        # creating 1 message for the student
        post_send = self.student.post_json('/PrivateMessage/Send.json', data={"body": "this is a body", "personId": person_id, "subject": "this is a subject"}, success=False)

    def test_student_doesnt_send_messages_to_foreign_student(self):
        self.internal_(3765) # student TRACEY BURRIS

    def tearDown(self):
        update_messaging_settings = self.admin.get_json(
            '/School/UpdateMessagingSettings.json?' + 'studentMessaging=' + str(True) +
            '&studentToClassOnly=' + str(False) + '&teacherToStudentMessaging=' + str(
                True) + '&teacherToClassOnly=' + str(False))

if __name__ == '__main__':
    unittest.main()