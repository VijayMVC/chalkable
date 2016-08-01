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

        # call of Grades
        get_teacher_summary = self.get('/Grading/TeacherSummary?' + 'teacherId=' + str(self.teacher_id))
        get_get_teacher_summary_data = get_teacher_summary['data']
        for k in get_get_teacher_summary_data:
            for key, value in k['class'].iteritems():
                if key == 'id':
                    list_for_class_id.append(value)

        list_for_student_id =[]

        for one_class in list_for_class_id:
            if one_class == 13861 or one_class == 13806 or one_class == 14011 or one_class == 14436 or one_class == 13770 or one_class == 13772:
                pass
            else:
                class_summary_grids = self.get('/Grading/ClassSummaryGrids?' + 'classId=' + str(one_class))
                class_summary_grids_data = class_summary_grids['data']

                for t in class_summary_grids_data['gradingperiods']:
                    # getting a collapsed grading period
                    class_grading_grid = self.get('/Grading/ClassGradingGrid?' + 'classId=' + str(
                        one_class) + '&gradingPeriodId=' + str(
                        t['id']) + '&standardId=' + '&classAnnouncementTypeId=' + '&notCalculateGrid=' + str(
                        False))
                    class_grading_grid_data = class_grading_grid['data']

                    for key, value in class_grading_grid_data.iteritems():
                        if key == 'students':
                            if len(value) > 0:
                                # getting list of students id
                                for y in class_grading_grid_data['students']:
                                    list_for_student_id.append(y['studentinfo']['id'])

                                one_comment = 'Just a comment'
                                one_score = '45.00'
                                students_activity_ForRandom = []

                                for p in class_grading_grid_data['gradingitems']:
                                    if p['candropstudentscore'] == True:
                                        for j in p['studentannouncements']['items']:
                                            if j['isabsent'] == False:
                                                students_activity_ForRandom.append({'studentid': j['studentid'], 'announcementid': j['announcementid']})

                                random_student_activity = random.choice(students_activity_ForRandom)
                                id_of_one_student = random_student_activity['studentid']
                                id_of_one_activity = random_student_activity['announcementid']

                                self.get('/Grading/UpdateItem?' + "announcementId=" + str(
                                    id_of_one_activity) + "&studentId=" + str(
                                    id_of_one_student) + "&gradeValue=" + one_score + "&comment=" + one_comment + '&dropped=' + str(
                                    True) + '&late=' + str(True) + '&absent=' + str(
                                    False) + '&incomplete=' + str(True) + '&exempt=' + str(
                                    False) + '&commentWasChanged=' + str(
                                    True) + '&callFromGradeBook=' + str(
                                    True))

                    # verifying that student has a correct score #7 shift tabs
                    class_grading_grid = self.get(
                        '/Grading/ClassGradingGrid?' + 'classId=' + str(
                            one_class) + '&gradingPeriodId=' + str(
                            t['id']) + '&standardId=' + '&classAnnouncementTypeId=' + '&notCalculateGrid=' + str(
                            False))
                    class_grading_grid_data = class_grading_grid['data']

                    for kk in class_grading_grid_data['gradingitems']:
                        if kk['id'] == id_of_one_activity:
                            for ii in kk['studentannouncements']['items']:
                                if ii['studentid'] == id_of_one_student:
                                    grade_value = ii['gradevalue']
                                    grade_value = grade_value.encode('utf8')
                                    isincomplete = ii['isincomplete']
                                    islate = ii['islate']
                                    dropped = ii['dropped']

                                    self.assertEqual(grade_value, one_score)
                                    self.assertEqual(str(ii['comment']),one_comment)
                                    self.assertEqual(isincomplete, True)
                                    self.assertEqual(islate, True)
                                    self.assertEqual(dropped, True)

if __name__ == '__main__':
    unittest.main()