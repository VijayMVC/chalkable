from base_auth_test import *
import unittest

class TestTeacherDeletingAllMessages(BaseTestCase):
    def setUp(self):
        self.admin = DistrictAdminSession(self).login(user_email, user_pwd)

        update_messaging_settings = self.admin.get_json(
            '/School/UpdateMessagingSettings.json?' + 'studentMessaging=' + str(True) +
            '&studentToClassOnly=' + str(False) + '&teacherToStudentMessaging=' + str(True) + '&teacherToClassOnly=' + str(
                False))

        self.teacher = TeacherSession(self).login(user_email, user_pwd)
        self.teacher_id = self.teacher.id_of_current_teacher()
        self.student = StudentSession(self).login(user_email_student, user_pwd_student)

    def internal_(self):
        # getting teacher's students
        get_students_my_students = self.teacher.get_json('/Student/GetStudents.json?' + 'myStudentsOnly=' + str(True) +
                                                         '&byLastName=' + str(True) + '&start=' + str(
            0) + '&count=' + str(1000))
        get_students_my_students = get_students_my_students['data']

        list_for_student_id_my_students = []

        for k in get_students_my_students:
            list_for_student_id_my_students.append(k['id'])

        self.one_random_student = random.choice(list_for_student_id_my_students)

        # creating 11 messages
        for i in range(11):
            post_send = self.teacher.post_json('/PrivateMessage/Send.json',
                                               data={"body": "this is a body_inbox_deleting_all_message",
                                                     "personId": self.teacher_id,
                                                     "subject": "this is a subject_inbox_deleting_all_message"})
            post_send = self.student.post_json('/PrivateMessage/Send.json',
                                               data={"body": "this is a body_inbox_deleting_all_message",
                                                     "personId": self.teacher_id,
                                                     "subject": "this is a subject_inbox_deleting_all_message"})

        list_for_message_id = []

        for i in self.get_all_inbox_messages():
            list_for_message_id.append(i['incomemessagedata']['id'])

        list_converted_to_string = str(list_for_message_id)

        sliced_list = list_converted_to_string[1:-1]

        # deleting all messages
        post_delete = self.teacher.post_json('/PrivateMessage/Delete.json?', data={"ids": sliced_list,
                "income": True})

        self.assertTrue(len(self.get_all_inbox_messages()) == 0, "teacher doesn't have 0 messages")

    def get_all_inbox_messages(self):
        get_messages_inbox_all = self.teacher.get_json(
            '/PrivateMessage/List.json?' + 'start=' + str(0) + '&count=' + str(1000) + '&income=' + str(
                True) + '&role=' + '&classOnly=' + str(False))
        get_messages_inbox_all_data = get_messages_inbox_all['data']
        return get_messages_inbox_all_data

    def test_teacher_deleting_all_messages(self):
        self.internal_()

if __name__ == '__main__':
    unittest.main()