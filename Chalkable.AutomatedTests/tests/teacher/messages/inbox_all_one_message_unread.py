from base_auth_test import *
import unittest

class TestMarkingReadOneMessage(BaseTestCase):
    def setUp(self):
        self.teacher = TeacherSession(self).login(user_email, user_pwd)
        self.teacher_id = self.teacher.id_of_current_teacher()

    def internal_(self):
        if len(self.get_all_inbox_messages()) > 0:
            list_for_message_id = []

            for i in self.get_all_inbox_messages():
                list_for_message_id.append(i['incomemessagedata']['id'])

            list_converted_to_string = str(list_for_message_id)
            random_message = random.choice(list_for_message_id)

            # all messages in state read
            sliced_list = list_converted_to_string[1:-1]
            post_unread = self.teacher.post_json('/PrivateMessage/MarkAsRead.json?', data={"ids": sliced_list,
                    "read": True})

            # one message in state unread
            post_unread = self.teacher.post_json('/PrivateMessage/MarkAsRead.json?', data={"ids": random_message,
                    "read": False})

            get_messages_inbox_all = self.teacher.get_json('/PrivateMessage/List.json?' + 'start=' + str(0) + '&count=' + str(750) + '&income=' + str(True) + '&role=' + '&classOnly=' + str(False))
            get_messages_inbox_all_data = get_messages_inbox_all['data']

            for info_about_one_message in get_messages_inbox_all_data:
                if info_about_one_message['incomemessagedata']['id'] == random_message:
                    self.assertFalse(info_about_one_message['incomemessagedata']['read'],
                                    'Message is not in the state False')

        else:
            # creating 7 messages
            for i in range(7):
                post_send = self.teacher.post_json('/PrivateMessage/Send.json',
                                                   data={"body": "this is a body", "personId": self.teacher_id,
                                                         "subject": "this is a subject"})

            self.internal_()

    def get_all_inbox_messages(self):
        get_messages_inbox_all = self.teacher.get_json(
            '/PrivateMessage/List.json?' + 'start=' + str(0) + '&count=' + str(750) + '&income=' + str(
                True) + '&role=' + '&classOnly=' + str(False))
        get_messages_inbox_all_data = get_messages_inbox_all['data']
        return get_messages_inbox_all_data

    def test_teacher_marking_unread_one_message(self):
        self.internal_()

if __name__ == '__main__':
    unittest.main()