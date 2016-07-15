from base_auth_test import *
from base_auth_test_student import *


class TestMessagesAdmin(BaseAuthedTestCase):
    def test_feed(self):
        update_messaging_settings = self.get_admin('/School/UpdateMessagingSettings.json?' + 'studentMessaging=' + str(True) +
            '&studentToClassOnly=' + str(True) + '&teacherToStudentMessaging=' + str(True) + '&teacherToClassOnly=' + str(False))


class TestMessagesStudent(BaseAuthedTestCaseStudent):
    def test_feed(self):
        data = {"body": "this is a body", "personId": 3765, "subject": "this is a subject"}  # student TRACEY BURRIS

        # creating 1 message for the student
        post_send = self.post_student('/PrivateMessage/Send.json', data, success=False)


if __name__ == '__main__':
    unittest.main()
