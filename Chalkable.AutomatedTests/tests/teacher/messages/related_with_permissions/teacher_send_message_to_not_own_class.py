from base_auth_test import *
import unittest

class TestTeacherSendMessagesToOwnStudent(BaseTestCase):
    def setUp(self):
        self.admin = DistrictAdminSession(self).login(user_email, user_pwd)

        update_messaging_settings = self.admin.get_json(
            '/School/UpdateMessagingSettings.json?' + 'studentMessaging=' + str(True) +
            '&studentToClassOnly=' + str(False) + '&teacherToStudentMessaging=' + str(
                True) + '&teacherToClassOnly=' + str(False))

        self.teacher = TeacherSession(self).login(user_email, user_pwd)
        self.foreign_student = StudentSession(self).login(user_email_foreign_student, user_pwd_foreign_student)

    def internal_(self):
        list_for_message_id = []

        for i in self.student_get_all_inbox_messages():
            list_for_message_id.append(i['incomemessagedata']['id'])

        list_converted_to_string = str(list_for_message_id)
        #random_message = random.choice(list_for_message_id)
        sliced_list = list_converted_to_string[1:-1]

        # deleting all messages
        post_delete = self.foreign_student.post_json('/PrivateMessage/Delete.json?', data={"ids": sliced_list,
                "income": True})

        #self.student_id = self.id_of_current_student()

        # creating 3 messages for the class
        for i in range(3):
            post_send = self.teacher.post_json('/PrivateMessage/Send.json',
                                               data={"body": "this is a body_message_for_not_own_class",
                                                     "classId": 13905,
                                                     "subject": "this is a subject_message_for_not_own_class"})
        self.assertTrue(len(self.student_get_all_inbox_messages()) == 3, 'student has 3 messages')

        # this piece of code isn't actual because I don't know an email of each user! If I'm using one random student
        for i in self.student_get_all_inbox_messages():
            self.assertTrue(i['incomemessagedata']['body'] == "this is a body_message_for_not_own_class", 'body of the message is equal')
            self.assertTrue(i['incomemessagedata']['subject'] == "this is a subject_message_for_not_own_class", 'subject of the message is equal')

    def test_sending_message_to_own_student(self):
        self.internal_()

    def student_get_all_inbox_messages(self):
        get_messages_inbox_all = self.foreign_student.get_json(
            '/PrivateMessage/List.json?' + 'start=' + str(0) + '&count=' + str(1000) + '&income=' + str(
                True) + '&role=' + '&classOnly=' + str(False))
        get_messages_inbox_all_data = get_messages_inbox_all['data']
        return get_messages_inbox_all_data

if __name__ == '__main__':
    unittest.main()