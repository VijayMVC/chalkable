from base_auth_test import *
import unittest

class TestClassProfileNow(BaseTestCase):
    def setUp(self):
        self.teacher = TeacherSession(self).login(user_email, user_pwd)

    def internal_(self):
        list_of_classes = self.teacher.list_of_classes()

        one_random_class = random.choice(list_of_classes)

        get_now_page_of_one_class = self.teacher.get_json('/Class/Summary.json?' + 'classId=' + str(one_random_class))

        self.teacher.post_json('/Feed/ClassFeed.json', data={'classId': one_random_class, "complete": False, "count": 10, "start": 0})

        self.teacher.get_json('/SchoolYear/ListOfSchoolYearClasses.json?') # Feed

        self.teacher.get_json('/GradingPeriod/ListByClassId.json?' + 'classId=' + str(one_random_class))

        self.teacher.get_json('/Class/Days.json?' + 'classId=' + str(one_random_class))

    def test_class_profile_now_tab(self):
        self.internal_()

if __name__ == '__main__':
    unittest.main()