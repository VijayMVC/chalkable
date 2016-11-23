from base_auth_test import *
import unittest

class TestSearchingByClasses(BaseTestCase):
    def setUp(self):
        self.teacher = TeacherSession(self).login(user_email, user_pwd)
        self.school_year = self.teacher.school_year()
        self.id_of_current_teacher = self.teacher.id_of_current_teacher()
        self.list_of_classes = self.teacher.list_of_classes()

    def internal_(self):
        if ('View Classroom' or 'View Classroom (Admin)' or 'Maintain Classroom' or 'Maintain Classroom (Admin)') in self.teacher.permissions():
            get_list_of_classes = self.teacher.get_json(
                '/Class/ClassesStats.json?' + 'schoolYearId=' + str(self.school_year) + '&start=' + str(0) + '&count=' + str(
                    100) + '&sortType=' + str(8) + '&teacherId=' + str(self.id_of_current_teacher) + '&filter=')
            get_list_of_classes_data = get_list_of_classes['data']

            # ToDo make more better code
            if len(get_list_of_classes['data']) > 0:
                for key in get_list_of_classes_data:
                    for key2, value2 in key.iteritems():
                        if key2 == 'teacherids':
                            self.assertTrue(self.id_of_current_teacher in value2, 'teacher is the owner of the class')

        else:
            self.teacher.post_json('/Class/ClassesStats.json?', data={"schoolYearId": str(self.school_year), "start": str(0), "count": str(100), "sortType": str(0),
                    "teacherId": str(self.id_of_current_teacher)}, success=False)

    def test_sorting_items_earliest(self):
        self.internal_()

    def tearDown(self):
        # reset all filters on the feed
        self.teacher.post_json('/Feed/SetSettings.json?', data={})

if __name__ == '__main__':
    unittest.main()