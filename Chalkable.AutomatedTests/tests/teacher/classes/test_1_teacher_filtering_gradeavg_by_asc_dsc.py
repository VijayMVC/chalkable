from base_auth_test import *
import unittest

class TestFinteringClassGradeAvg(BaseTestCase):
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

            last_item = None
            for item in get_list_of_classes_data:
                if last_item != None:
                    if sortType == 10:
                        self.assertTrue(last_item <= item['average'],
                                        "Items are sorted not in earliest order" + ": " +
                                        str(last_item) + " " +
                                        str(item['average']) + " " + "item_type: " + str(last_item) +
                                        " " + "item_id: " + str(item['average']))
                    if sortType == 11:
                        self.assertTrue(last_item >= item['average'],
                                        "Items are sorted not in latest order" + ": " +
                                        str(last_item) + " " +
                                        str(item['average']) + " " + "item_type: " + str(last_item) +
                                        " " + "item_id: " + str(item['average']))

                last_item = item['average']

        else:
            self.teacher.post_json('/Class/ClassesStats.json?',
                                   data={"schoolYearId": str(self.school_year), "start": str(0), "count": str(100),
                                         "sortType": str(0),
                                         "teacherId": str(self.id_of_current_teacher)}, success=False)

    def test_sorting_items_earliest(self):
        self.internal_(10)

    def test_sorting_items_latest(self):
        self.internal_(11)

    def tearDown(self):
        # reset all filters on the feed
        self.teacher.post_json('/Feed/SetSettings.json?', data={})

if __name__ == '__main__':
    unittest.main()