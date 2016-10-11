from base_auth_test import *
import unittest

class TestStudentDoesntSendMessagesAtAll(BaseTestCase):
    def setUp(self):
        self.admin = DistrictAdminSession(self).login(user_email, user_pwd)

        update_messaging_settings = self.admin.get_json(
            '/School/UpdateMessagingSettings.json?' + 'studentMessaging=' + str(False) +
            '&studentToClassOnly=' + str(False) + '&teacherToStudentMessaging=' + str(
                True) + '&teacherToClassOnly=' + str(False))

        self.student = StudentSession(self).login(user_email_student, user_pwd_student)

    def internal_(self, person_id):
        # creating 1 message for the student
        post_send = self.student.post_json('/PrivateMessage/Send.json', data={"body": "this is a body", "personId": person_id, "subject": "this is a subject"}, success=False)

    def test_student_doesnt_send_messages(self):
        self.internal_(5327) # student ELI BATTLE

if __name__ == '__main__':
    unittest.main()