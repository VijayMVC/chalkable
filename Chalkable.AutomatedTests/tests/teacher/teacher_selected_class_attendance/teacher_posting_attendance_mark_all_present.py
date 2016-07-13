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


        if 'View Classroom Attendance' in decoded_list:
            self.get('/Attendance/AttendanceSummary.json?')

            for class_id, mp_id in dict_for_clas_marking_period.items():
                for mp in mp_id:
                    start_date = str(dict_for_marking_period_date_startdate_endate[mp][0])
                    end_date = str(dict_for_marking_period_date_startdate_endate[mp][1])
                    date_in_correct_format = self.random_date(start_date, end_date)
                    get_class_list = self.get('/Attendance/ClassList.json?' +
                                              'classId=' + str(14436) +
                                              '&date=' + '09-22-2014')

                    if len(get_class_list['data']) > 0:
                        self.get('/Attendance/SetAttendanceForClass.json?' +
                                                  'classId=' + str(14436) +
                                                  '&date=' + '09-22-2014')

                get_class_list_second_time = self.get('/Attendance/ClassList.json?' +
                                                              'classId=' + str(14436) +
                                                              '&date=' + '09-22-2014')

                json_data_get_class_list_second_time = get_class_list_second_time['data']

                for info_about_one_student_second_time in json_data_get_class_list_second_time:
                    print info_about_one_student_second_time['level']
                    if info_about_one_student_second_time['readonly'] == True:
                        pass
                    else:
                        self.assertTrue(str(info_about_one_student_second_time['level']) == 'None')
                        date_in_cycle = datetime.strptime(str(info_about_one_student_second_time['date']),
                                                          '%Y-%m-%d').strftime('%m-%d-%Y')
                        self.assertTrue(info_about_one_student_second_time['classid'] == 14436,
                                        'class_id is not equal')
                        self.assertTrue(info_about_one_student_second_time['isposted'] == True,
                                        'attendance is not posted ' + str(14436))
                        self.assertTrue(date_in_cycle == '09-22-2014', 'dates are equal ' + str(14436))

if __name__ == '__main__':
    unittest.main()