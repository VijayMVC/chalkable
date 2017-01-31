from base_auth_test import *
import unittest

class TestFilteringClassTeacher(BaseTestCase):
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

        else:
            self.teacher.post_json('/Class/ClassesStats.json?', data={"schoolYearId": str(self.school_year), "start": str(0), "count": str(100), "sortType": str(0),
                    "teacherId": str(self.id_of_current_teacher)}, success=False)

    def test_sorting_items_earliest(self):
        self.internal_(2)

    def test_sorting_items_latest(self):
        self.internal_(3)

    def tearDown(self):
        # reset all filters on the feed
        self.teacher.post_json('/Feed/SetSettings.json?', data={})

if __name__ == '__main__':
    unittest.main()