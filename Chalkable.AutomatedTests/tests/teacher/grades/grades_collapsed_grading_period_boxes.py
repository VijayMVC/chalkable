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

        list_for_class_id = []
        list_for_standards_id = []
        list_for_class_announcement_types = []

        #general call
        get_grades_all = self.get('/Grading/TeacherSummary?' + 'teacherId=' + str(self.teacher_id))
        get_grades_all_data = get_grades_all['data']

        for k in get_grades_all_data:
            for key, value in k['class'].iteritems():
                if key == 'id':
                    list_for_class_id.append(value)

        for one_class in list_for_class_id:
            if one_class == 13861 or one_class == 13806 or one_class == 14011:
                pass
            else:
                class_summary_grids = self.get('/Grading/ClassSummaryGrids?' + 'classId=' + str(one_class))
                class_summary_grids = class_summary_grids['data']

                for key, value in class_summary_grids.iteritems():
                    if key == 'gradingperiods':
                        for i in value:
                            for key2, value2 in i.iteritems():
                                if key2 == 'id':
                                    # getting a collapsed grading period
                                    self.get('/Grading/ClassGradingPeriodSummary?' + 'classId=' + str(one_class) + '&gradingPeriodId=' + str(value2))

if __name__ == '__main__':
    unittest.main()
