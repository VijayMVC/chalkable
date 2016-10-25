from base_auth_test import *
import unittest

class TestClassProfileExplorer(BaseTestCase):
    def setUp(self):
        self.teacher = TeacherSession(self).login(user_email, user_pwd)

    def internal_(self):
        list_of_classes = self.teacher.list_of_classes()

        one_random_class = random.choice(list_of_classes)

        get_class_explorer = self.teacher.get_json('/Class/Explorer.json?' + 'classId=' + str(one_random_class))
        get_class_explorer_data_standards = get_class_explorer['data']['standards']

        if len(get_class_explorer_data_standards) > 0:
            self.teacher.get_json('/Application/SuggestedApps.json?' + 'start=' + str(0) + '&count=' + str(9999) + '&myAppsOnly=' + str(True))

    def test_class_profile_explorer_tab(self):
        self.internal_()

if __name__ == '__main__':
    unittest.main()