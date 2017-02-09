from base_auth_test import *
import unittest

class TestPuttingDiscipline(BaseTestCase):
    def setUp(self):
        self.teacher = TeacherSession(self).login(user_email, user_pwd)
        self.school_year = self.teacher.school_year()
        self.id_of_current_teacher = self.teacher.id_of_current_teacher()
        self.list_of_classes = self.teacher.list_of_classes()
        self.classes_marking_periods = self.teacher.classes_marking_periods() # self.dict_for_clas_marking_period
        self.marking_periods_with_dates = self.teacher.marking_periods_with_dates() # self.dict_for_marking_period_date_startdate_endate

    def internal_(self):
        if ('Maintain Classroom Discipline (Admin)' or 'Maintain Classroom Discipline') in self.teacher.permissions():
            # general call
            self.teacher.get_json('/Discipline/List.json?' + 'start=' + str(0))

            # getting id of infractions
            get_discipline_infractions = self.teacher.get_json('/DisciplineType/List.json?' + 'count=' + str(999))
            get_discipline_infractions_data = get_discipline_infractions['data']

            list_for_discipline_infractions_id = []
            for info_about_one_infraction in get_discipline_infractions_data:
                list_for_discipline_infractions_id.append(info_about_one_infraction['id'])

            for class_id, mp_id in self.classes_marking_periods.iteritems():
                for mp in mp_id:
                    one_reason = random.choice(list_for_discipline_infractions_id)
                    start_date = str(self.marking_periods_with_dates[mp][0])
                    end_date = str(self.marking_periods_with_dates[mp][1])
                    date_in_correct_format = self.random_date(start_date, end_date)

                    get_class_list = self.teacher.get_json('/Discipline/ClassList.json?' +
                                              'classId=' + str(class_id) + '&date=' + str(date_in_correct_format) +
                                              '&start=' + str(0))

                    json_data_get_class_list = get_class_list['data']

                    student_id_infraction = []

                    if len(get_class_list['data']) > 0:
                        for info_about_one_student in json_data_get_class_list:
                            student_id_infraction.append({"studentid": info_about_one_student["studentid"],
                                                          "id": info_about_one_student["id"]})

                        one_random_infraction = random.choice(student_id_infraction)

                        studentid = None
                        infraction_id = None
                        for key, value in one_random_infraction.iteritems():
                            if key == 'studentid':
                                studentid = value
                            if key == 'id':
                                infraction_id = value

                        if infraction_id is None:
                            self.teacher.post_json('/Discipline/SetClassDiscipline.json', data={"classId": class_id,
                                    "date": str(date_in_correct_format),
                                    "description": "",
                                    "id": "",
                                    "infractionsids": str(one_reason),
                                    "studentid": studentid,
                                    "time": int(round(time.time() * 1000))
                                    })

                        if infraction_id is not None:
                            self.teacher.post_json('/Discipline/SetClassDiscipline.json', data={"classId": class_id,
                                                     "date": str(date_in_correct_format),
                                                     "description": None,
                                                     "id": infraction_id,
                                                     "infractionsids": "",
                                                     "studentid": studentid,
                                                     "time": int(round(time.time() * 1000))
                                                     })

                            self.teacher.post_json('/Discipline/SetClassDiscipline.json', data={
                                "classId": class_id,
                                "date": str(date_in_correct_format),
                                "description": "",
                                "id": "",
                                "infractionsids": str(one_reason),
                                "studentid": studentid,
                                "time": int(round(time.time() * 1000))
                                })

                        # time.sleep(5)
                        get_class_list_second_time = self.teacher.get_json('/Discipline/ClassList.json?' +
                                                              'classId=' + str(class_id) + '&date=' + str(date_in_correct_format) +
                                                              '&start=' + str(0))

                        json_data_get_class_list_second_time = get_class_list_second_time['data']

                        for info_about_one_student in json_data_get_class_list_second_time:
                            if info_about_one_student['studentid'] == studentid:
                                for a in info_about_one_student['disciplinetypes']:
                                    self.assertEquals(a['id'], one_reason,
                                                      'discipline infractions are equals' + " " + str(
                                                          a['id']) + " " + str(one_reason) + " " + str(studentid))
        else:
            self.assertTrue(
                ('Maintain Classroom Discipline (Admin)' and 'Maintain Classroom Discipline') not in self.teacher.permissions(),
                'User does not have permissions to put discipline')

    def test_sorting_items_earliest(self):
        self.internal_()

    def tearDown(self):
        # reset all filters on the feed
        self.teacher.post_json('/Feed/SetSettings.json?', data={})

if __name__ == '__main__':
    unittest.main()