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

        if ('Maintain Classroom Discipline (Admin)' or 'Maintain Classroom Discipline') in decoded_list:
            #general call
            self.get('/Discipline/List.json?' + 'start=' + str(0))

            # getting id of infractions
            get_discipline_infractions = self.get('/DisciplineType/List.json?' + 'count=' + str(999))
            get_discipline_infractions_data = get_discipline_infractions['data']
            list_for_discipline_infractions_id = []
            for info_about_one_infraction in get_discipline_infractions_data:
                for key, value in info_about_one_infraction.iteritems():
                    if key == 'id':
                        list_for_discipline_infractions_id.append(value)

            for class_id, mp_id in dict_for_clas_marking_period.items():
                for mp in mp_id:
                    one_reason = random.choice(list_for_discipline_infractions_id)

                    start_date = str(dict_for_marking_period_date_startdate_endate[mp][0])
                    end_date = str(dict_for_marking_period_date_startdate_endate[mp][1])
                    date_in_correct_format = self.random_date(start_date, end_date)

                    get_class_list = self.get('/Discipline/ClassList.json?' +
                                              'classId=' + str(13770) + '&date=' + '05-24-2016' +
                                              '&start=' + str(0))


                    json_data_get_class_list = get_class_list['data']

                    student_id_infraction = []

                    if len(get_class_list['data']) > 0:
                        for info_about_one_student in json_data_get_class_list:
                            student_id_infraction.append({"studentid": info_about_one_student["studentid"],
                                                  "id": info_about_one_student["id"]})


                        one_random_infraction = random.choice(student_id_infraction)

                        studentid = None
                        id = None
                        for key, value in one_random_infraction.iteritems():
                            if key == 'studentid':
                                studentid = value
                            if key == 'id':
                                id = value


                        if id is None:
                            data = {"classId": 13770,
                                    "date": '05-24-2016',
                                    "description": "",
                                    "id": "",
                                    "infractionsids": str(one_reason),
                                    "studentid": studentid,
                                    "time": int(round(time.time() * 1000))
                                    }

                            self.postJSON('/Discipline/SetClassDiscipline.json', data)

                        if id is not None:
                            data_reset_discipline = {"classId": 13770,
                                                     "date": '05-24-2016',
                                                     "description": None,
                                                     "id": id,
                                                     "infractionsids": "",
                                                     "studentid": studentid,
                                                     "time": int(round(time.time() * 1000))
                                                     }

                            self.postJSON('/Discipline/SetClassDiscipline.json', data_reset_discipline)

                            data = {"classId": 13770,
                                    "date": '05-24-2016',
                                    "description": "",
                                    "id": "",
                                    "infractionsids": str(one_reason),
                                    "studentid": studentid,
                                    "time": int(round(time.time() * 1000))
                                    }

                            self.postJSON('/Discipline/SetClassDiscipline.json', data)

                        time.sleep(1) # due to setting infractions. Bad API
                        get_class_list_second_time = self.get('/Discipline/ClassList.json?' +
                                                              'classId=' + str(13770) + '&date=' + '05-24-2016' +
                                                              '&start=' + str(0))

                        json_data_get_class_list_second_time = get_class_list_second_time['data']

                        for info_about_one_student in json_data_get_class_list_second_time:
                            if info_about_one_student['studentid'] == studentid:
                                for a in info_about_one_student['disciplinetypes']:
                                    self.assertEquals(a['id'], one_reason, 'discipline infractions are equals' + " " + str(a['id']) + " " + str(one_reason) + " " + str(studentid))
        else:
            self.assertTrue(('Maintain Classroom Discipline (Admin)' and 'Maintain Classroom Discipline') not in decoded_list, 'User does not have permissions to put discipline')

if __name__ == '__main__':
    unittest.main()