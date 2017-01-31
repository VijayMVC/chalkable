from base_auth_test import *
import unittest

class TestMarkingReadAllMessages(BaseTestCase):
    def setUp(self):
        self.admin = DistrictAdminSession(self).login(user_email, user_pwd)

        update_messaging_settings = self.admin.get_json(
            '/School/UpdateMessagingSettings.json?' + 'studentMessaging=' + str(True) +
            '&studentToClassOnly=' + str(False) + '&teacherToStudentMessaging=' + str(
                True) + '&teacherToClassOnly=' + str(False))

        self.teacher = TeacherSession(self).login(user_email, user_pwd)
        self.teacher_id = self.teacher.id_of_current_teacher()
        self.foreign_student = StudentSession(self).login(user_email_foreign_student, user_pwd_foreign_student)

    def internal_(self):
        # creating 3 messages
        for i in range(3):
            post_send = self.teacher.post_json('/PrivateMessage/Send.json',
                                               data={"body": "this is a body", "personId": self.teacher_id,
                                                     "subject": "this is a subject"})
        for i in range(3):
            post_send = self.foreign_student.post_json('/PrivateMessage/Send.json',
                                                       data={"body": "this is a body", "personId": self.teacher_id,
                                                             "subject": "this is a subject"})
        list_for_message_id = []

        for i in self.get_all_inbox_messages():
            list_for_message_id.append(i['incomemessagedata']['id'])

        list_converted_to_string = str(list_for_message_id)
        sliced_list = list_converted_to_string[1:-1]

        post_read = self.teacher.post_json('/PrivateMessage/MarkAsRead.json?', data={"ids": sliced_list,
                "read": True})

        for i in self.get_all_inbox_messages():
            self.assertTrue(i['incomemessagedata']['read'], 'Message is not in the state True')

    def get_all_inbox_messages(self):
        get_messages_inbox_all = self.teacher.get_json(
            '/PrivateMessage/List.json?' + 'start=' + str(0) + '&count=' + str(750) + '&income=' + str(
                True) + '&role=' + '&classOnly=' + str(False))
        get_messages_inbox_all_data = get_messages_inbox_all['data']
        return get_messages_inbox_all_data

    def test_teacher_marking_read_all_messages(self):
        self.internal_()

if __name__ == '__main__':
    unittest.main()