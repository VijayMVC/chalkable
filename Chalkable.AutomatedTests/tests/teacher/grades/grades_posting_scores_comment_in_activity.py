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
        list_for_activity_id = []

        for one_class in list_for_class_id:
            if one_class == 13861 or one_class == 13806 or one_class == 14011 or one_class == 14436:
                pass
            else:
                class_summary_grids = self.get('/Grading/ClassSummaryGrids?' + 'classId=' + str(one_class))
                class_summary_grids_data = class_summary_grids['data']

                for key, value in class_summary_grids_data.iteritems():
                    if key == 'gradingperiods':
                        for i in value:
                            for key2, value2 in i.iteritems():
                                if key2 == 'id':
                                    # getting a collapsed grading period
                                    class_grading_grid = self.get('/Grading/ClassGradingGrid?' + 'classId=' + str(
                                        one_class) + '&gradingPeriodId=' + str(
                                        value2) + '&standardId=' + '&classAnnouncementTypeId=' + '&notCalculateGrid=' + str(
                                        False))
                                    class_grading_grid_data = class_grading_grid['data']

                                    for key, value in class_grading_grid_data.iteritems():
                                        if key == 'students':
                                            if len(value) > 0:
                                                # getting list of students id
                                                for y in class_grading_grid_data['students']:
                                                    list_for_student_id.append(y['studentinfo']['id'])

                                                # getting activity id
                                                for i in class_grading_grid_data['gradingitems']:
                                                    list_for_activity_id.append(i['id'])

                                                one_comment = 'Just a comment'
                                                one_random_student = random.choice(list_for_student_id)
                                                one_randon_activity = random.choice(list_for_activity_id)
                                                one_score = '45.00'
                                                self.get('/Grading/UpdateItem?' + "announcementId=" + str(
                                                    one_randon_activity) + "&studentId=" + str(
                                                    one_random_student) + "&gradeValue=" + one_score + "&comment=" + one_comment + '&dropped=' + str(
                                                    False) + '&late=' + str(False) + '&absent=' + str(
                                                    False) + '&incomplete=' + str(False) + '&exempt=' + str(
                                                    False) + '&commentWasChanged=' + str(
                                                    True) + '&callFromGradeBook=' + str(
                                                    True))

                                                list_for_student_id = []
                                                list_for_activity_id = []

                                                # verifying that student has a correct score
                                                class_grading_grid = self.get(
                                                    '/Grading/ClassGradingGrid?' + 'classId=' + str(
                                                        one_class) + '&gradingPeriodId=' + str(
                                                        value2) + '&standardId=' + '&classAnnouncementTypeId=' + '&notCalculateGrid=' + str(
                                                        False))
                                                class_grading_grid_data = class_grading_grid['data']

                                                for kk in class_grading_grid_data['gradingitems']:
                                                    if kk['id'] == one_randon_activity:
                                                        for ii in kk['studentannouncements']['items']:
                                                            if ii['studentid'] == one_random_student:
                                                                grade_value = ii['gradevalue']
                                                                grade_value = grade_value.encode('utf8')
                                                                self.assertEqual(grade_value, one_score)
                                                                self.assertEqual(str(ii['comment']),one_comment)

if __name__ == '__main__':
    unittest.main()