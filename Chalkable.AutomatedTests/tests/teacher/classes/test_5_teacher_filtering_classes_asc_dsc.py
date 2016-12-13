from base_auth_test import *
import unittest

class TestFilteringClassClasses(BaseTestCase):
    def setUp(self):
        self.teacher = TeacherSession(self).login(user_email, user_pwd)
        self.school_year = self.teacher.school_year()
        self.id_of_current_teacher = self.teacher.id_of_current_teacher()

    def internal_(self, sortType):
        if ('View Classroom' or 'View Classroom (Admin)' or 'Maintain Classroom' or 'Maintain Classroom (Admin)') in self.teacher.permissions():
            get_list_of_classes = self.teacher.get_json(
                '/Class/ClassesStats.json?' + 'schoolYearId=' + str(self.school_year) + '&start=' + str(0) + '&count=' + str(
                    100) + '&sortType=' + str(sortType) + '&teacherId=' + str(self.id_of_current_teacher))
            get_list_of_classes_data = get_list_of_classes['data']

            empty_list = []
            for one_class in get_list_of_classes_data:
                empty_list.append(one_class['name'])

            if sortType == 0:
                self.assertEqual(empty_list, sorted(empty_list), 'Classes sorted in asc')

            if sortType == 1:
                self.assertEqual(empty_list, sorted(empty_list, reverse=True), 'Classes sorted in dsc')

        else:
            self.teacher.post_json('/Class/ClassesStats.json?', data={"schoolYearId": str(self.school_year), "start": str(0), "count": str(100), "sortType": str(0),
                    "teacherId": str(self.id_of_current_teacher)}, success=False)

    def test_sorting_items_earliest(self):
        self.internal_(0)

    def test_sorting_items_latest(self):
        self.internal_(1)

    def tearDown(self):
        # reset all filters on the feed
        self.teacher.post_json('/Feed/SetSettings.json?', data={})

if __name__ == '__main__':
    unittest.main()