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
        list_for_standards_id = []
        list_for_class_announcement_types = []

        # call of Grades
        get_teacher_summary = self.get('/Grading/TeacherSummary?' + 'teacherId=' + str(self.teacher_id))
        get_get_teacher_summary_data = get_teacher_summary['data']
        for k in get_get_teacher_summary_data:
            for key, value in k['class'].iteritems():
                if key == 'id':
                    list_for_class_id.append(value)

        for one_class in list_for_class_id:
            if one_class == 13861 or one_class == 13806 or one_class == 14011 or one_class == 14436:
                pass
            else:
                class_summary_grids = self.get('/Grading/ClassSummaryGrids?' + 'classId=' + str(one_class))
                class_summary_grids = class_summary_grids['data']

                # getting class_announcement types of activity
                for i in class_summary_grids['classannouncementtypes']:
                    list_for_class_announcement_types.append(i['id'])

                if len(class_summary_grids['standards']) > 0:
                    # getting list of standards
                    for i in class_summary_grids['standards']:
                        list_for_standards_id.append(i['standardid'])

                    one_random_activity_type = random.choice(list_for_class_announcement_types)
                    one_random_standard = random.choice(list_for_standards_id)

                    for key, value in class_summary_grids.iteritems():
                        if key == 'gradingperiods':
                            for i in value:
                                for key2, value2 in i.iteritems():
                                    if key2 == 'id':
                                        if len(class_summary_grids['standards']) > 0:
                                            # sorting by one standard and one activity
                                            self.get('/Grading/ClassGradingGrid?' + 'classId=' + str(
                                                one_class) + '&gradingPeriodId=' + str(
                                                value2) + '&standardId=' + str(one_random_standard) + '&classAnnouncementTypeId=' + str(one_random_activity_type) + '&notCalculateGrid=' + str(
                                                True))

if __name__ == '__main__':
    unittest.main()
