from base_auth_test import *
import unittest

class TestClassProfileGrading(BaseTestCase):
    def setUp(self):
        self.teacher = TeacherSession(self).login(user_email, user_pwd)

    def internal_(self):
        list_of_classes = self.teacher.list_of_classes()
        one_random_class = random.choice(list_of_classes)

        self.teacher.get_json('/Class/Grading.json?' + 'classId=' + str(one_random_class))

    def test_class_profile_grading_tab(self):
        self.internal_()

if __name__ == '__main__':
    unittest.main()