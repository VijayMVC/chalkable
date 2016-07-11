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

            for class_id, mp_id in dict_for_clas_marking_period.items():
                for mp in mp_id:
                    start_date = str(dict_for_marking_period_date_startdate_endate[mp][0])
                    end_date = str(dict_for_marking_period_date_startdate_endate[mp][1])
                    date_in_correct_format = self.random_date(start_date, end_date)

                    get_class_list = self.get('/Attendance/SeatingChart.json?' +
                                              'classId=' + str(13806) +
                                              '&date=' + '05-06-2015')
                    get_data = get_class_list['data']

                    if get_data is None or get_data['isscheduled'] is False:
                        pass
                    else:
                        seating_chart_list = get_class_list['data']['seatinglist']
                        notseatingstudents_list = get_class_list['data']['notseatingstudents']

                        list_for_students = []

                        if len(seating_chart_list) > 0:
                            for one_student in notseatingstudents_list:
                                if one_student['personid'] != 0:
                                    list_for_students.append(one_student['personid'])
                        else:
                            self.postJSON('/Attendance/PostSeatingChart.json', {"seatingChartInfo":{"rows":2,"columns":3,"classId":str(13806),"seatingList":[[{"row":1,"column":1,"index":1},{"row":1,"column":2,"index":2},{"row":1,"column":3,"index":3}],[{"row":2,"column":1,"index":3},{"row":2,"column":2,"index":4},{"row":2,"column":3,"index":5}]]},"date":'05-06-2015',"needInfo":True})
                            get_class_list = self.get('/Attendance/SeatingChart.json?' +
                                                                  'classId=' + str(13806) +
                                                                  '&date=' + '05-06-2015')
                            get_data = get_class_list['data']

                            if get_data is None or get_data['isscheduled'] is False:
                                pass
                            else:
                                seating_chart_list = get_class_list['data']['seatinglist']
                                notseatingstudents_list = get_class_list['data']['notseatingstudents']

                                if len(seating_chart_list) > 0:
                                    for one_student in notseatingstudents_list:
                                        if one_student['personid'] != 0:
                                            list_for_students.append(one_student['personid'])

                        columns = get_class_list['data']['columns']
                        rows = get_class_list['data']['rows']
                        data_seatinglist_columns = get_class_list['data']['seatinglist']

                        for i_rows in data_seatinglist_columns:
                            for j_columns in i_rows:
                                if j_columns['info']!= None:
                                    for key, value in j_columns['info'].iteritems():
                                        if key == 'personid':
                                            list_for_students.append(value)

                        list_3 = []
                        for i_rows_3 in data_seatinglist_columns:
                            list_for_dict_2 = []
                            for j_columns in i_rows_3:
                                list_for_dict_2.append({"column": j_columns['column'],
                                                      "index": j_columns['index'],
                                                      "row": j_columns['row'],
                                                      "studentId": None})
                            list_3.append(list_for_dict_2)

                        seatingChartInfo = {"classId": str(13806), 'columns': columns, 'rows': rows, 'seatingList': list_3}
                        data = {"needInfo": False, "date": '05-06-2015', "seatingChartInfo": seatingChartInfo}
                        self.postJSON('/Attendance/PostSeatingChart.json', data)

                        random.shuffle(list_for_students)

                        index = 0
                        list_2 = []
                        for i_rows_2 in data_seatinglist_columns:
                            list_for_dict = []
                            for j_columns in i_rows_2:
                                list_for_dict.append({"column": j_columns['column'],
                                                      "index": j_columns['index'],
                                                      "row": j_columns['row'],
                                                      "studentId": list_for_students[index] if index < len(list_for_students) else None})
                                index += 1

                            list_2.append(list_for_dict)

                        seatingChartInfo_2 = {"classId":str(13806), 'columns':columns, 'rows':rows, 'seatingList': list_2}
                        data_2 = {"needInfo": False, "date": '05-06-2015', "seatingChartInfo": seatingChartInfo_2}
                        self.postJSON('/Attendance/PostSeatingChart.json', data_2)

### verify that data is correct
                    get_class_list_2 = self.get('/Attendance/SeatingChart.json?' +
                                                  'classId=' + str(13806) +
                                                  '&date=' + '05-06-2015')

                    get_data_2 = get_class_list_2['data']

                    if get_data_2 is None or get_data_2['isscheduled'] is False:
                        pass
                    else:
                        seating_chart_list_2 = get_class_list_2['data']['seatinglist']
                        notseatingstudents_list_2 = get_class_list_2['data']['notseatingstudents']

                        list_for_students_2 = []
                        if seating_chart_list_2 > 1:
                            for one_student_2 in notseatingstudents_list_2:
                                if one_student_2['personid'] != 0:
                                    list_for_students_2.append(one_student['personid'])

                        data_seatinglist_columns_2 = get_class_list_2['data']['seatinglist']

                        for i_rows_2 in data_seatinglist_columns_2:
                            for j_columns_2 in i_rows_2:
                                if j_columns_2['info'] != None:
                                    date_in_cycle = datetime.strptime(str(j_columns_2['info']['date']),
                                                                      '%Y-%m-%d').strftime('%m-%d-%Y')
                                    self.assertTrue (j_columns_2['info']['classid'] == 13806)
                                    #self.assertTrue (j_columns_2['info']['date'] == date_in_cycle)
                                    self.assertTrue (date_in_cycle == '05-06-2015')
                                    self.assertTrue (list_for_students[j_columns_2['index']] == j_columns_2['info']['personid'])

if __name__ == '__main__':
    unittest.main()