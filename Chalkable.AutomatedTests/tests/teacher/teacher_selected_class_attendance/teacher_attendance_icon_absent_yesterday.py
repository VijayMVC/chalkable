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
                    start_date = str(dict_for_marking_period_date_startdate_endate[mp][0])
                    end_date = str(dict_for_marking_period_date_startdate_endate[mp][1])
                    date_in_correct_format = self.random_date(start_date, end_date)


                    date_object = datetime.strptime('09-22-2014', '%m-%d-%Y')
                    date_tomorrow = date_object + timedelta(days=1)
                    date_tomorrow_correct_format = date_tomorrow.strftime('%d-%m-%Y')


                    get_class_list = self.get('/Attendance/ClassList.json?' +
                                              'classId=' + str(14436) +
                                              '&date=' + '09-22-2014')

                    json_data_get_class_list = get_class_list['data']

                    list_for_students = []
                    list_for_attendance_level = []
                    if len(get_class_list['data']) > 0:
                        for info_about_one_student in json_data_get_class_list:
                            if info_about_one_student['readonly'] == False:
                                self.attendance_level = list_of_levels[random.randint(0, 2)]

                                if self.attendance_level is None:
                                    attendance_reason_id = ""
                                else:
                                    attendance_reason_id = None

                                list_for_students.append({"personid": info_about_one_student['personid'],
                                                          "level": self.attendance_level,
                                                          "attendancereasonid": attendance_reason_id})
                                list_for_attendance_level.append(self.attendance_level)

                            else:
                                attendance_level_for_special_student = str(info_about_one_student['level'])
                                list_for_students.append({"personid": info_about_one_student['personid'],
                                                          "level": attendance_level_for_special_student,
                                                          "attendancereasonid": attendance_reason_id})
                                list_for_attendance_level.append(attendance_level_for_special_student)


                        data = {"classId": 14436,
                                "date": '09-22-2014',
                                "items": list_for_students}

                        post_attendance = self.postJSON('/Attendance/SetAttendance.json', data)

                        get_class_list_second_time = self.get('/Attendance/ClassList.json?' +
                                                              'classId=' + str(14436) +
                                                              '&date=' + date_tomorrow_correct_format)

                        json_data_get_class_list_second_time = get_class_list_second_time['data']
                        index = 0
                        for info_about_one_student_second_time in json_data_get_class_list_second_time:
                            attendance_level = info_about_one_student_second_time['level']
                            one_attendance_level = list_for_attendance_level[index]
                            if info_about_one_student_second_time['readonly'] == False:
                                if one_attendance_level == 'A':
                                    one_attendance_level = 'H'
                                    self.assertTrue(str(one_attendance_level) == str(attendance_level), " " + str(one_attendance_level) + " " + str(attendance_level))
                                    date_in_cycle = datetime.strptime(str(info_about_one_student_second_time['date']),
                                                                      '%Y-%m-%d').strftime('%m-%d-%Y')
                                    self.assertTrue(info_about_one_student_second_time['classid'] == 14436,
                                                    'class_id is not equal')
                                    self.assertTrue(info_about_one_student_second_time['isposted'] == True,
                                                    'attendance is not posted ' + str(14436))
                                    self.assertTrue(date_in_cycle == '09-22-2014', 'dates are equal ' + str(14436))
                                    self.assertTrue(info_about_one_student_second_time['absentpreviousday'] == True, 'student has absent alert')
                                else:
                                    self.assertTrue(str(one_attendance_level) == str(info_about_one_student_second_time['level'])," " + str(one_attendance_level) + " " + str(attendance_level))
                                    date_in_cycle = datetime.strptime(str(info_about_one_student_second_time['date']),
                                                                      '%Y-%m-%d').strftime('%m-%d-%Y')
                                    self.assertTrue(info_about_one_student_second_time['classid'] == 14436,
                                                    'class_id is not equal')
                                    self.assertTrue(info_about_one_student_second_time['isposted'] == True,
                                                    'attendance is not posted ' + str(14436))
                                    self.assertTrue(date_in_cycle == '09-22-2014', 'dates are equal ' + str(14436))
                                    self.assertTrue(info_about_one_student_second_time['absentpreviousday'] == True,
                                            'student has absent alert')
                            else:
                                    self.assertTrue(str(one_attendance_level) == str(attendance_level),
                                                    " " + str(one_attendance_level) + " " + str(attendance_level))
                                    date_in_cycle = datetime.strptime(str(info_about_one_student_second_time['date']),
                                                                      '%Y-%m-%d').strftime('%m-%d-%Y')
                                    self.assertTrue(info_about_one_student_second_time['classid'] == 14436,
                                                    'class_id is not equal')
                                    self.assertTrue(info_about_one_student_second_time['isposted'] == True,
                                                    'attendance is not posted ' + str(14436))
                                    self.assertTrue(date_in_cycle == '09-22-2014', 'dates are equal ' + str(14436))
                                    self.assertTrue(info_about_one_student_second_time['absentpreviousday'] == True,
                                            'student has absent alert')

                            index += 1


if __name__ == '__main__':
    unittest.main()