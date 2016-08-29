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

        #general call
        get_grades_all = self.get('/Grading/TeacherSummary?' + 'teacherId=' + str(self.teacher_id))
        get_grades_all_data = get_grades_all['data']
        for k in get_grades_all_data:
            for key, value in k['class'].iteritems():
                if key == 'id':
                    list_for_class_id.append(value)

        list_for_student_id =[]

        list_for_standards_id =[]

        self.list_of_standards_from_dict = []

        for one_class in list_for_class_id:
            if one_class == 13806 or one_class == 14011 or one_class == 14436:
                pass
            else:
                class_summary_grids = self.get('/Grading/ClassStandardGrids?' + 'classId=' + str(one_class))
                class_summary_grids = class_summary_grids['data']

                # veryfying if the class has students
                for key, value in class_summary_grids.iteritems():
                    if key == 'currentstandardgradinggrid':
                        for key2, value2 in value.iteritems():
                            if key2 == 'gradingitems':
                                if len(value2) > 0:
                                    if one_class in self.dic_for_class_allowed_standard: #verifying if teacher can put standards. Verifying if the area is not disabled.
                                        self.list_of_standards_from_dict = self.dic_for_class_allowed_standard[one_class]

                                        # getting list of standards id
                                        for y in class_summary_grids['currentstandardgradinggrid']['gradingitems']:
                                            list_for_standards_id.append(y['standard']['standardid'])

                                        # getting list of students id
                                        for y in class_summary_grids['currentstandardgradinggrid']['students']:
                                            list_for_student_id.append(y['studentinfo']['id'])

                                        for key, value in class_summary_grids.iteritems():
                                            if key == 'gradingperiods':
                                                for i in value:
                                                    for key2, value2 in i.iteritems():
                                                        if key2 == 'id':
                                                            random_student_id = random.choice(list_for_student_id)
                                                            random_standard_id = random.choice(list_for_standards_id)
                                                            random_alpha_grade_id = random.choice(self.list_of_standards_from_dict)

                                                            self.get('/Grading/UpdateStandardGrade?' + "classId=" + str(
                                                                one_class) + "&gradingPeriodId=" + str(value2) +
                                                                "&studentId=" + str(random_student_id) +
                                                                "&standardId=" + str(random_standard_id)+
                                                                '&alphaGradeId=' + str(random_alpha_grade_id) + '&note=')

                                                            # verifying that student has a correct score and a grading comment
                                                            class_summary_grids = self.get(
                                                                '/Grading/ClassStandardGrids?' + 'classId=' + str(one_class))
                                                            class_summary_grids_data = class_summary_grids['data']

                                                            for k in class_summary_grids_data['currentstandardgradinggrid']['gradingitems']:
                                                                if k['standard']['standardid'] == random_standard_id:
                                                                    for p in k['items']:
                                                                        if p['studentid'] == random_student_id:
                                                                            self.assertEqual(p['gradeid'],
                                                                                             random_alpha_grade_id)
                                                                            break
                                                                    #break

                                        list_for_student_id = []
                                        list_for_standards_id = []
                                        self.list_of_standards_from_dict = []

if __name__ == '__main__':
    unittest.main()
