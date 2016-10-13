from base_auth_test import *
import unittest

class TestTeacherSendMessagesToForeignStudent(BaseTestCase):
    def setUp(self):
        self.admin = DistrictAdminSession(self).login(user_email, user_pwd)

        update_messaging_settings = self.admin.get_json(
            '/School/UpdateMessagingSettings.json?' + 'studentMessaging=' + str(True) +
            '&studentToClassOnly=' + str(False) + '&teacherToStudentMessaging=' + str(True) + '&teacherToClassOnly=' + str(
                False))

        self.foreign_student = StudentSession(self).login(user_email_foreign_student, user_pwd_foreign_student)
        self.teacher = TeacherSession(self).login(user_email, user_pwd)

    def internal_(self):
        # getting students from the whole school
        get_students_whole_school = self.teacher.get_json(
            '/Student/GetStudents.json?' + 'myStudentsOnly=' + str(False) +
            '&byLastName=' + str(True) + '&start=' + str(
                0) + '&count=' + str(1000))
        get_students_whole_school_data = get_students_whole_school['data']

        list_for_student_id_whole_school = []

        for i in get_students_whole_school_data:
            list_for_student_id_whole_school.append(i['id'])

        # getting teacher's students
        get_students_my_students = self.teacher.get_json('/Student/GetStudents.json?' + 'myStudentsOnly=' + str(True) +
                                            '&byLastName=' + str(True) + '&start=' + str(0) + '&count=' + str(1000))
        get_students_my_students = get_students_my_students['data']

        list_for_student_id_my_students = []

        for k in get_students_my_students:
            list_for_student_id_my_students.append(k['id'])

        #self.one_random_student = random.choice([x for x in list_for_student_id_whole_school if x not in list_for_student_id_my_students])

        get_messages_inbox_all = self.foreign_student.get_json(
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
            post_delete = self.foreign_student.post_json('/PrivateMessage/Delete.json?', data={"ids": sliced_list,
                    "income": True})

        #self.student_id = self.id_of_current_student()

        # creating 3 messages for the student
        for i in range(3):
            post_send = self.teacher.post_json('/PrivateMessage/Send.json', data={"body": "this is a body", "personId": 3765, "subject": "this is a subject"}) #student TRACEY BURRIS

        # this piece of code isn't actual because I don't know an email of each user! If I'm using one random student
        get_messages_inbox_all = self.foreign_student.get_json('/PrivateMessage/List.json?' + 'start=' + str(0) + '&count=' + str(1000) + '&income=' + str(True) + '&role=' + '&classOnly=' + str(False))
        get_messages_inbox_all_data = get_messages_inbox_all['data']
        self.assertTrue(len(get_messages_inbox_all_data) == 3, 'student has 3 messages')

    def test_test_sending_message_to_foreign_student(self):
        self.internal_()

if __name__ == '__main__':
    unittest.main()