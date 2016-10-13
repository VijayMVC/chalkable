from base_auth_test import *
import unittest

class TestStudentSendingMessages(BaseTestCase):
    def setUp(self):
        self.admin = DistrictAdminSession(self).login(user_email, user_pwd)

        update_messaging_settings = self.admin.get_json('/School/UpdateMessagingSettings.json?' + 'studentMessaging=' + str(True) +
            '&studentToClassOnly=' + str(True) + '&teacherToStudentMessaging=' + str(True) + '&teacherToClassOnly=' + str(True))

        self.classmate = StudentSession(self).login(user_email_classmate, user_pwd_classmate)
        self.student = StudentSession(self).login(user_email_student, user_pwd_student)

    def internal_(self):
        get_messages_inbox_all = self.classmate.get_json(
            '/PrivateMessage/List.json?' + 'start=' + str(0) + '&count=' + str(1000) + '&income=' + str(
                True) + '&role=' + '&classOnly=' + str(False))
        get_messages_inbox_all_data = get_messages_inbox_all['data']

        if len(get_messages_inbox_all_data) > 0:
            list_for_message_id = []
            for i in get_messages_inbox_all_data:
                for i in get_messages_inbox_all_data:
                    list_for_message_id.append(i['incomemessagedata']['id'])

            list_converted_to_string = str(list_for_message_id)
            #random_message = random.choice(list_for_message_id)
            sliced_list = list_converted_to_string[1:-1]

            # deleting all messages
            post_delete = self.classmate.post_json('/PrivateMessage/Delete.json?', data={"ids": sliced_list,
                    "income": True})

        #self.student_id = self.id_of_current_student()

        # creating 3 messages for the student
        for i in range(3):
            post_send = self.student.post_json('/PrivateMessage/Send.json', data={"body": "this is a body", "personId": 5327, "subject": "this is a subject"}) # #student ELI BATTLE

        get_messages_inbox_all = self.classmate.get_json('/PrivateMessage/List.json?' + 'start=' + str(0) + '&count=' + str(1000) + '&income=' + str(True) + '&role=' + '&classOnly=' + str(False))
        get_messages_inbox_all_data = get_messages_inbox_all['data']
        self.assertTrue(len(get_messages_inbox_all_data) == 3, 'student has 3 messages')
        for i in get_messages_inbox_all_data:
            self.assertTrue(i['incomemessagedata']['body'] == "this is a body", 'body of the message is equal')
            self.assertTrue(i['incomemessagedata']['subject'] == "this is a subject", 'subject of the message is equal')

    def test_sending_message_to_classmate(self):
        self.internal_()

    def tearDown(self):
        update_messaging_settings = self.admin.get_json(
            '/School/UpdateMessagingSettings.json?' + 'studentMessaging=' + str(True) +
            '&studentToClassOnly=' + str(False) + '&teacherToStudentMessaging=' + str(
                True) + '&teacherToClassOnly=' + str(False))

if __name__ == '__main__':
    unittest.main()