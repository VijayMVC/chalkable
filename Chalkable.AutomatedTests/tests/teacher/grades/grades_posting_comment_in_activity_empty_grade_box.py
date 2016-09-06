from base_auth_test import *

class TestFeed(BaseAuthedTestCase):

    def test_feed(self):
        empty_list = []
        person_me = self.get('/Person/me')
        person_me_list_of_dictionaries = person_me['data']['claims']

        for item in person_me_list_of_dictionaries:
            empty_list.append(item['values'])

        final_list = [item for sublist in empty_list for item in sublist]
        # ['Maintain Classroom', ...]
        decoded_list = [x.encode('utf-8') for x in final_list]

        # {14436: [295, 296], ...}
        dict_for_class_marking_period = self.dict_for_clas_marking_period

        # {296: (u'2015-01-05', u'2016-07-31'), ...}
        dict_for_marking_period_date_startdate_endate = self.dict_for_marking_period_date_startdate_endate

        list_for_class_id = []

        #general call
        get_grades_all = self.get('/Grading/TeacherSummary?' + 'teacherId=' + str(self.teacher_id))
        get_grades_all_data = get_grades_all['data']
        for k in get_grades_all_data:
            for key, value in k['class'].iteritems():
                if key == 'id':
                    list_for_class_id.append(value)

        for one_class in list_for_class_id:
            if one_class == 13861 or one_class == 13806 or one_class == 14011 or one_class == 14436:
                pass
            else:
                class_summary_grids = self.get('/Grading/ClassSummaryGrids?' + 'classId=' + str(one_class))
                class_summary_grids_data = class_summary_grids['data']

                students_activity_ForRandom = []
                # verifying if the class has students
                if len(class_summary_grids_data['currentgradinggrid']['students']) > 0:
                    # getting activity id; items without grades, lates, incompletes
                    for i in class_summary_grids_data['currentgradinggrid']['gradingitems']:
                        for k in i['studentannouncements']['items']:
                            #print k
                            if k['gradevalue'] == None and k['isexempt'] == False and k['isincomplete']== False and k['islate'] == False:
                                #list_for_activity_id.append(i['id'])
                                students_activity_ForRandom.append({'studentid': k['studentid'], 'announcementid': k['announcementid']})
                                break

                    random_student_activity = random.choice(students_activity_ForRandom)
                    id_of_one_student = random_student_activity['studentid']
                    id_of_one_activity = random_student_activity['announcementid']

                    self.get('/Grading/UpdateItem?' + "announcementId=" + str(id_of_one_activity)+ "&studentId=" + str(id_of_one_student) + "&gradeValue=" + "&comment=" + "this is a comment" + '&dropped=' + str(False) + '&late=' + str(False) + '&absent=' + str(False) + '&incomplete=' + str(False) + '&exempt=' + str(False) + '&commentWasChanged=' + str(True) + '&callFromGradeBook=' + str(True))

                    # verifying that student has a correct score and a grading comment
                    class_summary_grids = self.get('/Grading/ClassSummaryGrids?' + 'classId=' + str(one_class))
                    class_summary_grids_data = class_summary_grids['data']

                    for i in class_summary_grids_data['currentgradinggrid']['gradingitems']:
                        if i['id'] == id_of_one_activity:
                            for k in i['studentannouncements']['items']:
                                if k['studentid'] == id_of_one_student:
                                    #print k['studentid'], id_of_one_activity, one_class
                                    self.assertEqual(k['gradevalue'], None)
                                    self.assertEqual(k['isexempt'], False)
                                    self.assertEqual(k['isincomplete'], False)
                                    self.assertEqual(k['islate'], False)

if __name__ == '__main__':
    unittest.main()