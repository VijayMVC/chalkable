from base_auth_test import *
import unittest

class TestMessagesSearch(BaseTestCase):
    def setUp(self):
        self.teacher = TeacherSession(self).login(user_email, user_pwd)
        self.teacher_id = self.teacher.id_of_current_teacher()
        self.student = StudentSession(self).login(user_email_student, user_pwd_student)

    def internal_(self):
        if len(self.get_all_inbox_messages()) > 0:
            list_for_first_name = []
            for i in self.get_all_inbox_messages():
                list_for_first_name.append(i['incomemessagedata']['sender']['firstname'])

            self.random_first_name = random.choice(list_for_first_name)

            self.assertTrue(len(self.get_all_inbox_messages_after_search()) >= 1, 'Must be at least one message')

            if len(self.get_all_inbox_messages_after_search()) >= 1:
                for k in self.get_all_inbox_messages_after_search():
                    self.assertTrue(k['incomemessagedata']['sender']['firstname'] == self.random_first_name, 'Wrong owner of the message')
            else:
                print "Something went wrong. Must be at least one message"

        else:
            # creating 7 messages
            for i in range(7):
                post_send = self.teacher.post_json('/PrivateMessage/Send.json',
                                                   data={"body": "this is a body", "personId": self.teacher_id,
                                                         "subject": "this is a subject"})

                post_send = self.student.post_json('/PrivateMessage/Send.json',
                                                   data={"body": "this is a body", "personId": self.teacher_id,
                                                         "subject": "this is a subject"})

            self.internal_()

    def get_all_inbox_messages(self):
        get_messages_inbox_all = self.teacher.get_json(
            '/PrivateMessage/List.json?' + 'start=' + str(0) + '&count=' + str(100) + '&income=' + str(
                True) + '&role=' + '&classOnly=' + str(False))
        get_messages_inbox_all_data = get_messages_inbox_all['data']
        return get_messages_inbox_all_data

    def get_all_inbox_messages_after_search(self):
        get_messages_inbox_all = self.teacher.get_json(
            '/PrivateMessage/List.json?' + 'start=' + str(0) + '&count=' + str(100) + '&income=' + str(
                True) + '&role=' + '&keyword=' + str(self.random_first_name) + '&classOnly=' + str(False) + '&acadYear=')
        get_messages_inbox_all_data = get_messages_inbox_all['data']
        return get_messages_inbox_all_data

    def test_teacher_search(self):
        self.internal_()

if __name__ == '__main__':
    unittest.main()