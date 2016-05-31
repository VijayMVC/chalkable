from base_auth_test import *
from base_auth_test_teacher import *


class TestMessagesAdmin(BaseAuthedTestCase):
    def test_feed(self):
        update_messaging_settings = self.get_admin('/School/UpdateMessagingSettings.json?' + 'studentMessaging=' + str(True) +
            '&studentToClassOnly=' + str(False) + '&teacherToStudentMessaging=' + str(False) + '&teacherToClassOnly=' + str(False))


class TestMessagesTeacher(BaseAuthedTestCaseTeacher):
    def test_feed(self):
        data = {"body": "this is a body", "personId": 3688, "subject": "this is a subject"}  # student CATALINA AYERS

        # creating 1 message for the student
        post_send = self.post('/PrivateMessage/Send.json', data, success=False)


if __name__ == '__main__':
    unittest.main()
