from base_auth_test import *

class TestFeed(BaseAuthedTestCase):
    def test_feed(self):
        update_messaging_settings = self.get_admin('/School/UpdateMessagingSettings.json?' + 'studentMessaging=' + str(True) +
            '&studentToClassOnly=' + str(False) + '&teacherToStudentMessaging=' + str(True) + '&teacherToClassOnly=' + str(True))

        data = {"body": "this is a body", "personId": 3765, "subject": "this is a subject"} #student TRACEY BURRIS

        # creating 1 message for the student
        post_send = self.post('/PrivateMessage/Send.json', data, 200, False)


if __name__ == '__main__':
    unittest.main()
