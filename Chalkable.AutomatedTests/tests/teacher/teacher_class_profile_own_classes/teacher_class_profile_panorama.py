from base_auth_test import *
import unittest

# ToDo.. after this task is fixed http://emp.myjetbrains.com/youtrack/issue/CHLK-5432
class TestClassProfilePanorama(BaseAuthedTestCase):
    def setUp(self):
        self.teacher = TeacherSession(self).login(user_email, user_pwd)

    def internal_(self):
        list_of_classes = self.teacher.list_of_classes()

        one_random_class = random.choice(list_of_classes)

        post_panorama = self.teacher.post_json('/Class/Panorama.json', data={'classId': one_random_class})
        post_panorama = post_panorama['data']

        list_standardizedtests = []
        for i in post_panorama['standardizedtests']:
            if len(i['components']) > 0:
                list_standardizedtests.append(i)

        print list_standardizedtests, len(list_standardizedtests)

        restore_panorama = self.teacher.post_json('/Class/RestorePanoramaSettings.json', data={'classId': one_random_class})
        restore_panorama = restore_panorama['data']
        list_of_acad_years = restore_panorama['acadyears']
        list_of_acad_years = map(str, list_of_acad_years)

        list_of_acad_years_filtered = []
        for i in list_of_acad_years:
            if i == '2008':
                continue
            list_of_acad_years_filtered.append(i)

        one_random_year = str(random.choice(list_of_acad_years_filtered))

        #post_all_years_panorama
        self.teacher.post_json('/Class/Panorama.json', data={'classId': one_random_class, "acadYears": list_of_acad_years, "standardizedTestFilters": [], "selectedStudents": []})

        #save_all_years_panorama
        self.teacher.post_json('/Class/SavePanoramaSettings.json', data={"classId": one_random_class,"standardizedTestFilters": [],"acadYears": list_of_acad_years})

        #post_none_years_panorama
        self.teacher.post_json('/Class/Panorama.json', data={'classId': one_random_class, "standardizedTestFilters": [], "selectedStudents": []})

        #save_none_years_panorama
        self.teacher.post_json('/Class/SavePanoramaSettings.json', data={"classId": one_random_class, "standardizedTestFilters": []}, success=False)

        #post_one_year_panorama
        self.teacher.post_json('/Class/Panorama.json', data={'classId': one_random_class,
                                                                                       "acadYears": one_random_year,
                                                                                       "standardizedTestFilters": [],
                                                                                       "selectedStudents": []})

        #save_one_year_panorama
        self.teacher.post_json('/Class/SavePanoramaSettings.json', data={"classId": one_random_class, "standardizedTestFilters": [], "acadYears": one_random_year})

    def test_student_profile_info_panorama(self):
        self.internal_()

if __name__ == '__main__':
    unittest.main()