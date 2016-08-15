from base_auth_test import *


class TestFeed(BaseAuthedTestCase):
    def test_feed(self):
        empty_list = []
        person_me = self.get('/Person/me')
        person_me_list_of_dictionaries = person_me['data']['claims']

        for item in person_me_list_of_dictionaries:
            empty_list.append(item['values'])

        final_list = [item for sublist in empty_list for item in sublist]
        decoded_list = [x.encode('utf-8') for x in final_list]

        dict_for_clas_marking_period = self.dict_for_clas_marking_period
        dict_for_marking_period_date_startdate_endate = self.dict_for_marking_period_date_startdate_endate

        list_of_levels = ['A', 'T', None]

        if 'View Classroom Attendance' in decoded_list:
            get_attendance_summary = self.get('/Attendance/AttendanceSummary.json?')

            for class_id, mp_id in dict_for_clas_marking_period.items():
                for mp in mp_id:
                    attendance_level = list_of_levels[random.randint(0, 2)]
                    if attendance_level is None:
                        attendance_reason_id = ""
                    else:
                        attendance_reason_id = None

                    start_date = str(dict_for_marking_period_date_startdate_endate[mp][0])
                    end_date = str(dict_for_marking_period_date_startdate_endate[mp][1])
                    date_in_correct_format = self.random_date(start_date, end_date)

                    get_class_list = self.get('/Attendance/ClassList.json?' +
                                              'classId=' + str(class_id) +
                                              '&date=' + date_in_correct_format)

                    json_data_get_class_list = get_class_list['data']

                    list_for_students = []
                    if len(get_class_list['data']) > 0:
                        for info_about_one_student in json_data_get_class_list:
                            list_for_students.append({"personid": info_about_one_student['personid'],
                                                      "level": attendance_level,
                                                      "attendancereasonid": attendance_reason_id})

                        data = {"classId": class_id,
                                "date": date_in_correct_format,
                                "items": list_for_students}

                        post_attendance = self.postJSON('/Attendance/SetAttendance.json', data)

                        get_class_list_second_time = self.get('/Attendance/ClassList.json?' +
                                                              'classId=' + str(class_id) +
                                                              '&date=' + date_in_correct_format)

                        json_data_get_class_list_second_time = get_class_list_second_time['data']

                        for info_about_one_student_second_time in json_data_get_class_list_second_time:
                            date_in_cycle = datetime.strptime(str(info_about_one_student_second_time['date']),
                                                              '%Y-%m-%d').strftime('%m-%d-%Y')

                            self.assertTrue(info_about_one_student_second_time['classid'] == class_id,
                                            'class_id is not equal')
                            self.assertTrue(info_about_one_student_second_time['isposted'] == True,
                                            'attendance is not posted ' + str(class_id))

                            if info_about_one_student_second_time['level'] == 'H':
                                self.assertTrue(attendance_level == 'A', 'attendance levels are equal ' + str(class_id))

                            self.assertTrue(date_in_cycle == date_in_correct_format, 'dates are equal ' + str(class_id))
                    else:
                        self.assertTrue(len(get_class_list['data']) == 0, 'There are no students!')


if __name__ == '__main__':
    unittest.main()
