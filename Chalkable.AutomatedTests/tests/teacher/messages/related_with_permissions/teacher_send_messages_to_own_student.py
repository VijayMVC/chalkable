from base_auth_test import *

class TestFeed(BaseAuthedTestCase):
    def test_feed(self):
        # getting students from the whole school
        get_students_whole_school = self.get(
            '/Student/GetStudents.json?' + 'myStudentsOnly=' + str(False) +
            '&byLastName=' + str(True) + '&start=' + str(
                0) + '&count=' + str(1000))

        get_students_whole_school_data = get_students_whole_school['data']

        list_for_student_id_whole_school = []

        for i in get_students_whole_school_data:
            for key, value in i.iteritems():
                if key == 'id':
                    list_for_student_id_whole_school.append(value)

        # getting teacher's students
        get_students_my_students = self.get('/Student/GetStudents.json?' + 'myStudentsOnly=' + str(True) +
                                            '&byLastName=' + str(True) + '&start=' + str(0) + '&count=' + str(1000))

        get_students_my_students = get_students_my_students['data']

        list_for_student_id_my_students = []

        for k in get_students_my_students:
            for key, value in k.iteritems():
                if key == 'id':
                    list_for_student_id_my_students.append(value)

        self.one_random_student = random.choice([x for x in list_for_student_id_whole_school if x not in list_for_student_id_my_students])

        update_messaging_settings = self.get_admin('/School/UpdateMessagingSettings.json?' + 'studentMessaging=' + str(True) +
            '&studentToClassOnly=' + str(False) + '&teacherToStudentMessaging=' + str(True) + '&teacherToClassOnly=' + str(True))


        get_messages_inbox_all = self.get_student(
            '/PrivateMessage/List.json?' + 'start=' + str(0) + '&count=' + str(1000) + '&income=' + str(
                True) + '&role=' + '&classOnly=' + str(False))

        get_messages_inbox_all_data = get_messages_inbox_all['data']

        if len(get_messages_inbox_all_data) > 0:
            list_for_message_id = []
            for i in get_messages_inbox_all_data:
                for key, value in i['incomemessagedata'].iteritems():
                    if key == 'id':
                        list_for_message_id.append(value)

            list_converted_to_string = str(list_for_message_id)
            random_message = random.choice(list_for_message_id)
            sliced_list = list_converted_to_string[1:-1]

            # deleting all messages
            data = {"ids": sliced_list,
                    "income": True}

            post_delete = self.postJSON_student('/PrivateMessage/Delete.json?', data)

        #self.student_id = self.id_of_current_student()

        data = {"body": "this is a body", "personId": 3688, "subject": "this is a subject"}  #student CATALINA AYERS

        # creating 3 messages for the student
        for i in range(3):
            post_send = self.postJSON('/PrivateMessage/Send.json', data)

        # this piece of code isn't actual because I don't know an email of each user!
        get_messages_inbox_all = self.get_student('/PrivateMessage/List.json?' + 'start=' + str(0) + '&count=' + str(1000) + '&income=' + str(True) + '&role=' + '&classOnly=' + str(False))
        get_messages_inbox_all_data = get_messages_inbox_all['data']
        self.assertTrue(len(get_messages_inbox_all_data) == 3, 'student has 3 messages')



if __name__ == '__main__':
    unittest.main()
