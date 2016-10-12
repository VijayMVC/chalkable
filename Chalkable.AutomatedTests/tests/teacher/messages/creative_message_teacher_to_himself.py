from base_auth_test import *
import unittest

class TestTeacherSendingMessagesToHimSelf(BaseTestCase):
    def setUp(self):
        self.teacher = TeacherSession(self).login(user_email, user_pwd)
        self.teacher_id = self.teacher.id_of_current_teacher()

    def internal_(self):
        get_messages_inbox_all = self.teacher.get_json(
            '/PrivateMessage/List.json?' + 'start=' + str(0) + '&count=' + str(1000) + '&income=' + str(
                True) + '&role=' + '&classOnly=' + str(False))
        get_messages_inbox_all_data = get_messages_inbox_all['data']

        if len(get_messages_inbox_all_data) > 0:
            list_for_message_id = []
            for i in get_messages_inbox_all_data:
                list_for_message_id.append(i['incomemessagedata']['id'])

            list_converted_to_string = str(list_for_message_id)
            #random_message = random.choice(list_for_message_id)
            sliced_list = list_converted_to_string[1:-1]

            # deleting all messages
            post_delete = self.teacher.post_json('/PrivateMessage/Delete.json?', data={"ids": sliced_list,
                    "income": True})

        # creating 15 messages
        for i in range(15):
            post_send = self.teacher.post_json('/PrivateMessage/Send.json', data={"body": "this is a body", "personId": self.teacher_id, "subject": "this is a subject"})

        get_messages_inbox_all = self.teacher.get_json(
            '/PrivateMessage/List.json?' + 'start=' + str(0) + '&count=' + str(1000) + '&income=' + str(
                True) + '&role=' + '&classOnly=' + str(False))
        self.assertTrue(len(get_messages_inbox_all['data']) == 15, "teacher doesn't have 15 messages")

        for i in get_messages_inbox_all_data:
            self.assertTrue(i['incomemessagedata']['body'] == "this is a body", 'body of the message is equal')
            self.assertTrue(i['incomemessagedata']['subject'] == "this is a subject", 'subject of the message is equal')

    def test_teacher_sending_message_to_himself(self):
        self.internal_()

if __name__ == '__main__':
    unittest.main()