import re
import unittest
import requests
from base_config import *
from datetime import datetime, timedelta
from datetime import timedelta
import time
import json
import random
import ast


class BaseAuthedTestCase(unittest.TestCase):
    def setUp(self):
        # info for the teacher
        s = requests.Session()
        payload = {'UserName': user_email, 'Password': user_pwd, 'remember': 'false'}
        url = chlk_server_url + '/User/LogOn.aspx'
        r = s.post(url, data=payload)

        page_as_text = r.text
        page_as_one_string = str(page_as_text)

        # info for the student
        s_student = requests.Session()
        payload_student = {'UserName': user_email_student, 'Password': user_pwd_student, 'remember': 'false'}
        url_student = chlk_server_url + '/User/LogOn.aspx'
        r_student = s_student.post(url_student, data=payload_student)

        page_as_text_student = r_student.text
        page_as_one_string_student = str(page_as_text_student)

        # info for the foreign student
        s_foreign_student = requests.Session()
        payload_foreign_student = {'UserName': user_email_foreign_student, 'Password': user_pwd_foreign_student, 'remember': 'false'}
        url_foreign_student = chlk_server_url + '/User/LogOn.aspx'
        r_foreign_student = s_foreign_student.post(url_foreign_student, data=payload_foreign_student)

        page_as_text_foreign_student = r_foreign_student.text
        page_as_one_string_foreign_foreign_student = str(page_as_text_foreign_student)

        # info for the classmate
        s_classmate = requests.Session()
        payload_classmate = {'UserName': user_email_classmate, 'Password': user_pwd_classmate,
                                   'remember': 'false'}
        url_classmate = chlk_server_url + '/User/LogOn.aspx'
        r_classmate = s_classmate.post(url_classmate, data=payload_classmate)

        page_as_text_classmate = r_classmate.text
        page_as_one_string_classmate = str(page_as_text_classmate)


        # info for the admin
        s_admin = requests.Session()
        payload_admin = {'UserName': user_email, 'Password': user_pwd, 'remember': 'false'}
        url_admin = chlk_server_url + '/User/LogOn.aspx'
        r_admin = s_admin.post(url_admin, data=payload_admin)
        url_admin = chlk_server_url + '/User/SwitchToDistrictAdmin.aspx'
        r_admin = s_admin.get(url_admin)

        page_as_text_admin = r_admin.text
        page_as_one_string_admin = str(page_as_text_admin)

        # getting messagingSettings
        var_messagingSettings = re.findall('var messagingSettings = .+', page_as_one_string)
        var_messagingSettings_string = ''.join(var_messagingSettings)
        var_markingPeriods_cut_off_string = var_messagingSettings_string[31:-3]
        self.var_messagingSettings_cut_off_string = json.loads(var_markingPeriods_cut_off_string)

        # getting DistrictId
        found_sting = re.findall('var districtId = .+', page_as_one_string)
        concatenated_str = ''.join(found_sting)
        self.districtId = concatenated_str[18:54]

        # getting SchoolYearId
        var_school_year = re.findall('var currentSchoolYearId = .+', page_as_one_string)
        var_school_year_string = ''.join(var_school_year)
        self.schoolYearId = var_school_year_string[-5:-2]

        # getting all academic years
        var_years_list = re.findall('var years =.+', page_as_one_string)
        var_years_string = ''.join(var_years_list)
        var_years__cut_off_string = var_years_string[29:-19]
        var_years_cut_off_string = ''.join(var_years__cut_off_string)
        self.var_years_list = ast.literal_eval(var_years_cut_off_string)

        # getting id of the current teacher
        var_current_chlk_person_list = re.findall('var currentChlkPerson = .+', page_as_one_string)
        var_current_chlk_person_list_string = ''.join(var_current_chlk_person_list)
        var_current_chlk_person_list_string_cut_off_string = var_current_chlk_person_list_string[31:-3]
        var_current_chlk_person_cut_off_string = json.loads(var_current_chlk_person_list_string_cut_off_string)

        for key, value in var_current_chlk_person_cut_off_string['data'].iteritems():
            if key == 'id':
                self.teacher_id = value

        # getting id of the current student
        var_current_chlk_person_list_student = re.findall('var currentChlkPerson = .+', page_as_one_string_student)
        var_current_chlk_person_list_string_student = ''.join(var_current_chlk_person_list_student)
        var_current_chlk_person_list_string_cut_off_string_student = var_current_chlk_person_list_string_student[31:-3]
        var_current_chlk_person_cut_off_string_student = json.loads(var_current_chlk_person_list_string_cut_off_string_student)
        for key, value in var_current_chlk_person_cut_off_string_student['data'].iteritems():
            if key == 'id':
                self.student_id = value


        # getting grading periods
        var_grading_periods_list = re.findall('var gradingPeriods = .+', page_as_one_string)
        var_grading_periods_string = ''.join(var_grading_periods_list)
        var_grading_periods_cut_off_list = re.findall('"id":[0-9]+', var_grading_periods_string)
        var_grading_periods_cut_off_string = ''.join(var_grading_periods_cut_off_list)
        self.var_grading_periods_final_list = re.findall('[0-9]+', var_grading_periods_cut_off_string)

        # getting marking periods
        var_marking_periods_list = re.findall('var markingPeriods = .+', page_as_one_string)
        var_markingPeriods_string = ''.join(var_marking_periods_list)
        var_markingPeriods_cut_off_string = var_markingPeriods_string[31:-3]
        dictionary_var_markingPeriods_cut_off_string = json.loads(var_markingPeriods_cut_off_string)
        self.dict_for_marking_period_date_startdate_endate = {}
        dictionary_var_markingPeriods_cut_off_string_data = dictionary_var_markingPeriods_cut_off_string['data']

        for marking_period in dictionary_var_markingPeriods_cut_off_string_data:
            self.dict_for_marking_period_date_startdate_endate[marking_period['id']] = marking_period['startdate'], marking_period['enddate']

        # getting list of marking periods
        self.list_for_marking_periods = []
        for key,value in self.dict_for_marking_period_date_startdate_endate.iteritems():
            self.list_for_marking_periods.append(key)

        # getting pairs 'class/marking period'
        var_classesToFilter_list = re.findall('var classesToFilter = .+', page_as_one_string)
        var_classesToFilter_string = ''.join(var_classesToFilter_list)
        var_markingPeriods_cut_off_string = var_classesToFilter_string[31:-3]
        dictionary_var_markingPeriods_cut_off_string = json.loads(var_markingPeriods_cut_off_string)
        self.dict_for_clas_marking_period = {}
        self.list_of_classes = []
        dictionary_var_markingPeriods_cut_off_string_data = dictionary_var_markingPeriods_cut_off_string['data']

        for info_for_one_class in dictionary_var_markingPeriods_cut_off_string_data:
            self.dict_for_clas_marking_period[info_for_one_class['id']] = info_for_one_class['markingperiodsid']

        # getting list of classes
        for k in dictionary_var_markingPeriods_cut_off_string_data:
            self.list_of_classes.append(k['id'])

        self.session = s

        self.session_student = s_student

        self.session_foreign_student = s_foreign_student

        self.session_admin = s_admin

        self.session_classmate = s_classmate

    def list_of_years(self):
        list_of_years = self.var_years_list
        return list_of_years

    def list_of_classes(self):
        list_of_class_id = self.list_of_classes
        return list_of_class_id

    def id_of_current_teacher(self):
        teacher_id = self.teacher_id
        return teacher_id

    def id_of_current_student(self):
        student_id = self.student_id
        return student_id

    def school_year(self):
        current_school_year = self.schoolYearId
        return current_school_year

    def gr_periods(self):
        list_of_gr_periods = self.var_grading_periods_final_list
        return list_of_gr_periods

    def marking_periods(self):
        list_of_marking_periods = self.list_for_marking_periods
        return list_of_marking_periods

    def get(self, url, status=200, success=True):
        s = self.session
        headers = {'X-Requested-With': 'XMLHttpRequest'}
        r = s.get(chlk_server_url + url, headers=headers)
        return self.verify_response(r, status, success)

    def postJSON(self, url, obj, status=200, success=True):
        s = self.session
        headers = {'X-Requested-With': 'XMLHttpRequest'}
        r = s.post(chlk_server_url + url, json=obj, headers=headers)
        return self.verify_response(r, status, success)

    def post(self, url, params, status=200, success=True):

        s = self.session
        headers = {'X-Requested-With': 'XMLHttpRequest'}

        try:
            r = s.post(chlk_server_url + url, data=params, headers=headers)
        except ValueError:
            self.assertTrue(False, 'Request failed, ' + url)
            return None

        return self.verify_response(r, status, success)

    def verify_response(self, r, status, success):
        self.assertEquals(r.status_code, status, 'Response status code: ' + str(r.status_code) + ', body:' + r.text)

        try:
            data = r.json()
        except ValueError:
            self.assertTrue(False, 'Parse JSON failed, ' + r.url)
            return None

        self.assertEquals(data['success'], success, 'API success')

        return data

    # these methods for the student
    def get_student(self, url_student, status=200, success=True):
        s_student = self.session_student
        headers = {'X-Requested-With': 'XMLHttpRequest'}
        r_student = s_student.get(chlk_server_url + url_student, headers=headers)
        return self.verify_response_student(r_student, status, success)

    def postJSON_student(self, url_student, obj, status=200, success=True):
        s_student = self.session_student
        headers = {'X-Requested-With': 'XMLHttpRequest'}
        r_student = s_student.post(chlk_server_url + url_student, json=obj, headers=headers)
        return self.verify_response_student(r_student, status, success)

    def post_student(self, url_student, params, status=200, success=True):
        s_student = self.session_student
        headers = {'X-Requested-With': 'XMLHttpRequest'}

        try:
            r_student = s_student.post(chlk_server_url + url_student, data=params, headers=headers)
        except ValueError:
            self.assertTrue(False, 'Request failed, ' + url_student)
            return None

        return self.verify_response_student(r_student, status, success)

    def verify_response_student(self, r_student, status, success):
        self.assertEquals(r_student.status_code, status, 'Response status code')

        try:
            data = r_student.json()
        except ValueError:
            self.assertTrue(False, 'Parse JSON failed, ' + r_student.url_student)
            return None

        self.assertEquals(data['success'], success, 'API success')

        return data
    # the end of methods for the student

    # these methods for the foreign student
    def get_foreign_student(self, url_foreign_student, status=200, success=True):
        s_foreign_student = self.session_foreign_student
        headers = {'X-Requested-With': 'XMLHttpRequest'}
        r_foreign_student = s_foreign_student.get(chlk_server_url + url_foreign_student, headers=headers)
        return self.verify_response_foreign_student(r_foreign_student, status, success)

    def postJSON_foreign_student(self, url_foreign_student, obj, status=200, success=True):
        s_foreign_student = self.session_foreign_student
        headers = {'X-Requested-With': 'XMLHttpRequest'}
        r_foreign_student = s_foreign_student.post(chlk_server_url + url_foreign_student, json=obj, headers=headers)
        return self.verify_response_foreign_student(r_foreign_student, status, success)

    def post_foreign_student(self, url_foreign_student, params, status=200, success=True):
        s_foreign_student = self.session_foreign_student
        headers = {'X-Requested-With': 'XMLHttpRequest'}

        try:
            r_foreign_student = s_foreign_student.post(chlk_server_url + url_foreign_student, data=params, headers=headers)
        except ValueError:
            self.assertTrue(False, 'Request failed, ' + url_foreign_student)
            return None

        return self.verify_response_foreign_student(r_foreign_student, status, success)

    def verify_response_foreign_student(self, r_foreign_student, status, success):
        self.assertEquals(r_foreign_student.status_code, status, 'Response status code')

        try:
            data = r_foreign_student.json()
        except ValueError:
            self.assertTrue(False, 'Parse JSON failed, ' + r_foreign_student.url_foreign_student)
            return None

        self.assertEquals(data['success'], success, 'API success')

        return data
    # the end of methods for the foreign student


    # these methods for classmate
    def get_classmate(self, url_classmate, status=200, success=True):
        s_classmate = self.session_classmate
        headers = {'X-Requested-With': 'XMLHttpRequest'}
        r_classmate = s_classmate.get(chlk_server_url + url_classmate, headers=headers)
        return self.verify_response_classmate(r_classmate, status, success)

    def postJSON_classmate(self, url_classmate, obj, status=200, success=True):
        s_classmate = self.session_classmate
        headers = {'X-Requested-With': 'XMLHttpRequest'}
        r_classmate = s_classmate.post(chlk_server_url + url_classmate, json=obj, headers=headers)
        return self.verify_response_classmate(r_classmate, status, success)

    def post_classmate(self, url_classmate, params, status=200, success=True):
        s_classmate = self.session_classmate
        headers = {'X-Requested-With': 'XMLHttpRequest'}

        try:
            r_classmate = s_classmate.post(chlk_server_url + url_classmate, data=params, headers=headers)
        except ValueError:
            self.assertTrue(False, 'Request failed, ' + url_classmate)
            return None

        return self.verify_response_classmate(r_classmate, status, success)

    def verify_response_classmate(self, r_classmate, status, success):
        self.assertEquals(r_classmate.status_code, status, 'Response status code')

        try:
            data = r_classmate.json()
        except ValueError:
            self.assertTrue(False, 'Parse JSON failed, ' + r_classmate.url_student)
            return None

        self.assertEquals(data['success'], success, 'API success')

        return data
    # the end of methods for classmates



    # these methods for the admin
    def get_admin(self, url_admin, status=200, success=True):
        s_admin = self.session_admin
        r_admin = s_admin.get(chlk_server_url + url_admin)
        return self.verify_response_admin(r_admin, status, success)

    def verify_response_admin(self, r_admin, status, success):
        self.assertEquals(r_admin.status_code, status, 'Response status code')

        try:
            data = r_admin.json()
        except ValueError:
            self.assertTrue(False, 'Parse JSON failed, ' + r_admin.url)
            return None

        self.assertEquals(data['success'], success, 'API success')

        return data
    # the end of methods for the admin

    # noinspection PyMethodMayBeStatic
    def random_date(self, start_date, end_date):
        # type: (string, string) -> string
        start_date1 = datetime.strptime(start_date, "%Y-%m-%d").date()
        end_date1 = datetime.strptime(end_date, "%Y-%m-%d").date()

        delta = (end_date1 - start_date1).days
        random_date_for_attendance = start_date1 + timedelta(random.randint(0, delta))

        return datetime.strptime(str(random_date_for_attendance), '%Y-%m-%d').strftime('%m-%d-%Y')



if __name__ == '__main__':
    unittest.main()
