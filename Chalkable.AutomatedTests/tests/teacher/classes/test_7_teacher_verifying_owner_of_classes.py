from base_auth_test import *
import unittest

class TestVerifyingOwnerOfClasses(BaseTestCase):
    def setUp(self):
        self.teacher = TeacherSession(self).login(user_email, user_pwd)
        self.school_year = self.teacher.school_year()
        self.id_of_current_teacher = self.teacher.id_of_current_teacher()
        self.list_of_classes = self.teacher.list_of_classes()

    def internal_(self):
        if ('View Classroom' or 'View Classroom (Admin)' or 'Maintain Classroom' or 'Maintain Classroom (Admin)') in self.teacher.permissions():
            get_list_of_classes = self.teacher.get_json(
                '/Class/ClassesStats.json?' + 'schoolYearId=' + str(self.school_year) + '&start=' + str(0) + '&count=' + str(
                    100) + '&sortType=0' + '&teacherId=' + str(self.id_of_current_teacher))
            get_list_of_classes_data = get_list_of_classes['data']

            # ToDo make more better code
            for key in get_list_of_classes_data:
                for key2, value2 in key.iteritems():
                    if key2 == 'teacherids':
                        teacher_ids = value2
                        self.assertTrue(self.id_of_current_teacher in teacher_ids, "teacher isn't owner of the class")
                    if key2 == 'id':
                        id = value2
                        self.assertTrue(id in self.list_of_classes, "teacher isn't owner of the class")

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