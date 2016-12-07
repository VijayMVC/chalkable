from base_auth_test import *
import unittest

class TestDeletingOneMessageNotOwnStudent(BaseTestCase):
    def setUp(self):
        self.admin = DistrictAdminSession(self).login(user_email, user_pwd)

        update_messaging_settings = self.admin.get_json(
            '/School/UpdateMessagingSettings.json?' + 'studentMessaging=' + str(True) +
            '&studentToClassOnly=' + str(False) + '&teacherToStudentMessaging=' + str(
                True) + '&teacherToClassOnly=' + str(
                False))

        self.teacher = TeacherSession(self).login(user_email, user_pwd)
        self.teacher_id = self.teacher.id_of_current_teacher()
        self.foreign_student = TeacherSession(self).login(user_email_foreign_student, user_pwd_foreign_student)

    def internal_(self):
        if len(self.get_all_inbox_messages()) > 0:
            list_for_message_id = []
            for i in self.get_all_inbox_messages():
                list_for_message_id.append(i['incomemessagedata']['id'])

            list_converted_to_string = str(list_for_message_id)

            sliced_list = list_converted_to_string[1:-1]

            # deleting all messages
            post_delete = self.teacher.post_json('/PrivateMessage/Delete.json?', data={"ids": sliced_list,
                                                                                       "income": True})

            self.assertTrue(len(self.get_all_inbox_messages()) == 0, "teacher doesn't have 0 messages")

            # creating 3 messages
            self.creating_3_messages()

            self.verify_message_owner()

        else:
            # creating 3 messages
            self.creating_3_messages()

            self.verify_message_owner()

    def creating_3_messages(self):
        for i in range(3):
            post_send = self.foreign_student.post_json('/PrivateMessage/Send.json',
                                                       data={"body": "this is a body", "personId": self.teacher_id,
                                                             "subject": "this is a subject"})

    def get_all_inbox_messages(self):
        get_messages_inbox_all = self.teacher.get_json(
            '/PrivateMessage/List.json?' + 'start=' + str(0) + '&count=' + str(750) + '&income=' + str(
                True) + '&role=' + '&classOnly=' + str(False))
        get_messages_inbox_all_data = get_messages_inbox_all['data']
        return get_messages_inbox_all_data


    def verify_message_owner(self):
        list_for_message_id_second = []
        for i in self.get_all_inbox_messages():
            list_for_message_id_second.append(i['incomemessagedata']['id'])

        # list_converted_to_string = str(list_for_message_id)
        random_message = random.choice(list_for_message_id_second)

        # deleting one message
        post_delete = self.teacher.post_json('/PrivateMessage/Delete.json?', data={"ids": random_message,
                                                                                   "income": True})

        list_for_message_id_third = []
        for i in self.get_all_inbox_messages():
            list_for_message_id_third.append(i['incomemessagedata']['id'])

        self.assertTrue(random_message not in list_for_message_id_third, "deleted message isn't in the list")

    def test_teacher_deleting_one_message(self):
        self.internal_()

if __name__ == '__main__':
    unittest.main()