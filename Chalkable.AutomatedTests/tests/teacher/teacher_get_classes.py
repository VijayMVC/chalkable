from base_auth_test import *


class TestFeed(BaseAuthedTestCase):
    def test_feed(self):
        school_year = self.school_year()
        id_of_current_teacher = self.id_of_current_teacher()

        list_of_classes = self.list_of_classes

        get_list_of_classes = self.get('/Class/ClassesStats.json?' + 'schoolYearId=' + str(school_year) + '&start=' + str(0) + '&count=' + str(100) + '&sortType=0' + '&teacherId=' + str(id_of_current_teacher))
        get_list_of_classes_data = get_list_of_classes['data']

        for key in get_list_of_classes_data:
            for key2, value2 in key.iteritems():
                if key2 == 'teacherids':
                    teacher_ids = value2
                    self.assertTrue(id_of_current_teacher in teacher_ids, "teacher isn't owner of the class")
                if key2 == 'id':
                    id = value2
                    self.assertTrue(id in list_of_classes, "teacher isn't owner of the class")


if __name__ == '__main__':
    unittest.main()