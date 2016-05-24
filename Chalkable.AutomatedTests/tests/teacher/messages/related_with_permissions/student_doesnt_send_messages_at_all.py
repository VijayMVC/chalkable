from base_auth_test import *
from base_auth_test_student import *


class TestMessagesAdmin(BaseAuthedTestCase):
    def test_feed(self):
        update_messaging_settings = self.get_admin('/School/UpdateMessagingSettings.json?' + 'studentMessaging=' + str(False) +
            '&studentToClassOnly=' + str(False) + '&teacherToStudentMessaging=' + str(True) + '&teacherToClassOnly=' + str(False))


class TestMessagesStudent(BaseAuthedTestCaseStudent):
    def test_feed(self):
        data = {"body": "this is a body", "personId": 5327, "subject": "this is a subject"}  # student ELI BATTLE

        # creating 1 message for the student
        post_send = self.post_student('/PrivateMessage/Send.json', data, success=False)


if __name__ == '__main__':
    unittest.main()
