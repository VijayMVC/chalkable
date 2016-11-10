from base_auth_test import *
import unittest

class TestClassProfileDiscipline(BaseTestCase):
    def setUp(self):
        self.teacher = TeacherSession(self).login(user_email, user_pwd)

    def internal_(self):
        list_of_classes = self.teacher.list_of_classes()

        self.one_random_class = random.choice(list_of_classes)

        self.teacher.get_json('/Class/Summary.json?' + 'classId=' + str(self.one_random_class))

        self.discipline_gr_period_month_year_day(0)
        self.discipline_gr_period_month_year_day(1)
        self.discipline_gr_period_month_year_day(2)
        self.discipline_gr_period_month_year_day(3)

    def discipline_gr_period_month_year_day(self, dateType):
        self.teacher.get_json('/Class/DisciplineSummary.json?' + 'classId=' + str(self.one_random_class) + '&dateType=' + str(dateType))

    def test_class_profile_discipline_tab(self):
        self.internal_()

if __name__ == '__main__':
    unittest.main()