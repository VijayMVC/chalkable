import pyodbc
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
import string
from time import sleep


class UserSession:
    def __init__(self, instance):
        pass
        self.unittest = instance

        self.session = requests.Session()
        self.districtId = None
        self.schoolYearId = None

    def login(self, email, password):
        payload = {'UserName': email, 'Password': password, 'remember': 'false'}
        r = self.post_html('/User/LogOn.aspx', data=payload)
        self.parse_body_(r)
        return self

    def parse_body_(self, page_as_one_string):
        # getting DistrictId
        found_sting = re.findall('var districtId = .+', page_as_one_string)
        concatenated_str = ''.join(found_sting)
        self.districtId = concatenated_str[18:54]

        # getting SchoolYearId
        var_school_year = re.findall('var currentSchoolYearId = .+', page_as_one_string)
        var_school_year_string = ''.join(var_school_year)
        self.schoolYearId = var_school_year_string[-5:-2]

        # getting grading periods
        var_grading_periods_list = re.findall('var gradingPeriods = .+', page_as_one_string)

        var_grading_periods_string = ''.join(var_grading_periods_list)
        var_grading_periods_cut_off_list = re.findall('"id":[0-9]+', var_grading_periods_string)
        var_grading_periods_cut_off_string = ''.join(var_grading_periods_cut_off_list)
        self.var_grading_periods_final_list = re.findall('[0-9]+', var_grading_periods_cut_off_string)

        # getting all academic years
        var_years_list = re.findall('var years =.+', page_as_one_string)
        var_years_string = ''.join(var_years_list)
        var_years__cut_off_string = var_years_string[29:-19]
        var_years_cut_off_string = ''.join(var_years__cut_off_string)
        self.var_years_list = ast.literal_eval(var_years_cut_off_string)

        # getting grading periods and dates
        self.list_for_grading_periods_and_classes = []
        var_grading_periods_list_for_dict = re.findall('var gradingPeriods = .+', page_as_one_string)
        var_grading_periods_string_for_dict = ''.join(var_grading_periods_list_for_dict)
        var_grading_periods_cut_off_string_for_dict = var_grading_periods_string_for_dict[30:-3]
        dictionary_var_grading_periods_cut_off_string_for_dict = json.loads(var_grading_periods_cut_off_string_for_dict)
        # print dictionary_var_grading_periods_cut_off_string_for_dict, type(dictionary_var_grading_periods_cut_off_string_for_dict)
        dictionary_var_grading_periods_cut_off_string_for_dict_data = \
        dictionary_var_grading_periods_cut_off_string_for_dict['data']
        for j in dictionary_var_grading_periods_cut_off_string_for_dict_data:
            # print j['startdate'], j['enddate']
            self.list_for_grading_periods_and_classes.append({'startdate': j['startdate'], 'enddate': j['enddate']})



    def get_(self, url, status=200, **kwargs):
        r = self.session.get(chlk_server_url + url, **kwargs)
        self.unittest.assertEquals(r.status_code, status, 'Response status code: ' + str(r.status_code) + ', body:' + r.text)
        return r

    def get_file_(self, url, status=200, success=True):
        r = self.session.get(chlk_server_url + url)
        self.unittest.assertEquals(r.status_code, status,
                                   'Response status code: ' + str(r.status_code) + ', body:' + r.text)
        return r.text

    def post_(self, url, status=200, **kwargs):
        r = self.session.post(chlk_server_url + url, **kwargs)
        self.unittest.assertEquals(r.status_code, status, 'Response status code: ' + str(r.status_code) + ', body:' + r.text)
        return r

    def validate_json_(self, r, success_property, success):
        try:
            data = r.json()
        except ValueError:
            self.unittest.assertTrue(False, 'Parse JSON failed, ' + r.url + r.text)
            return None

        msg = 'API success ' + str(data[success_property]) + '=' + str(success)
        self.unittest.assertEquals(data[success_property], success, msg)

        return data

    def get_html(self, url, **kwargs):
        return self.get_(url, **kwargs).text

    def get_json(self, url, success_property='success', success=True, **kwargs):
        headers = {'X-Requested-With': 'XMLHttpRequest'}
        r = self.get_(url, headers=headers, **kwargs)
        return self.validate_json_(r, success_property, success)

    def post_html(self, url, **kwargs):
        return self.post_(url, **kwargs).text

    def post_json(self, url, success_property='success', success=True, **kwargs):
        headers = {'X-Requested-With': 'XMLHttpRequest'}
        r = self.post_(url, headers=headers, **kwargs)
        return self.validate_json_(r, success_property, success)

    def gr_periods(self):
        list_of_gr_periods = self.var_grading_periods_final_list
        return list_of_gr_periods

    def permissions(self):
        list_for_permissions = []
        person_me = self.get_json('/Person/me')
        person_me_list_of_dictionaries = person_me['data']['claims']

        for item in person_me_list_of_dictionaries:
            list_for_permissions.append(item['values'])

        final_list = [item for sublist in list_for_permissions for item in sublist]
        decoded_list_for_permissions = [x.encode('utf-8') for x in final_list]

        return decoded_list_for_permissions

    def school_year(self):
        current_school_year = self.schoolYearId
        return current_school_year

    def list_of_years(self):
        return self.var_years_list

class TeacherSession(UserSession):
    def __init__(self, instance):
        UserSession.__init__(self, instance)

        self.teacher_id = None

    def parse_body_(self, page_as_one_string):
        UserSession.parse_body_(self, page_as_one_string)

        # getting id of the current teacher
        tmp = re.findall('var currentChlkPerson = .+', page_as_one_string)
        tmp = ''.join(tmp)
        tmp = tmp[31:-3]
        tmp = json.loads(tmp)
        self.teacher_id = tmp['data']['id']

        # getting pairs 'class/marking period'
        var_classesToFilter_list = re.findall('var classesToFilter = .+', page_as_one_string)
        var_classesToFilter_string = ''.join(var_classesToFilter_list)
        var_markingPeriods_cut_off_string = var_classesToFilter_string[31:-3]
        dictionary_var_markingPeriods_cut_off_string = json.loads(var_markingPeriods_cut_off_string)
        self.dict_for_clas_marking_period = {}
        self.empty_list_of_classes = []
        self.dictionary_var_markingPeriods_cut_off_string_data = dictionary_var_markingPeriods_cut_off_string['data']

        # getting marking periods
        var_marking_periods_list = re.findall('var markingPeriods = .+', page_as_one_string)
        var_markingPeriods_string = ''.join(var_marking_periods_list)
        var_markingPeriods_cut_off_string2 = var_markingPeriods_string[31:-3]
        dictionary_var_markingPeriods_cut_off_string2 = json.loads(var_markingPeriods_cut_off_string2)
        self.dict_for_marking_period_date_startdate_endate = {}
        self.dictionary_var_markingPeriods_cut_off_string_data2 = dictionary_var_markingPeriods_cut_off_string2['data']

    def id_of_current_teacher(self):
        teacher_id = self.teacher_id
        return teacher_id

    def list_of_classes(self):
        # getting list of classes
        for k in self.dictionary_var_markingPeriods_cut_off_string_data:
            self.empty_list_of_classes.append(k['id'])
        return self.empty_list_of_classes

    def classes_marking_periods(self):
        for info_for_one_class in self.dictionary_var_markingPeriods_cut_off_string_data:
            self.dict_for_clas_marking_period[info_for_one_class['id']] = info_for_one_class['markingperiodsid']
        return self.dict_for_clas_marking_period

    def marking_periods_with_dates(self):
        for marking_period in self.dictionary_var_markingPeriods_cut_off_string_data2:
            self.dict_for_marking_period_date_startdate_endate[marking_period['id']] = marking_period['startdate'], \
                                                                                       marking_period['enddate']
        return self.dict_for_marking_period_date_startdate_endate


class StudentSession(UserSession):
    def __init__(self, instance):
        UserSession.__init__(self, instance)

        self.student_id = None

    def parse_body_(self, page_as_one_string):
        UserSession.parse_body_(self, page_as_one_string)


class DistrictAdminSession(UserSession):
    def __init__(self, instance):
        UserSession.__init__(self, instance)

        self.admin_id = None

    def parse_body_(self, page_as_one_string):
        UserSession.parse_body_(self, page_as_one_string)

    def login(self, email, password):
        UserSession.login(self, email, password)
        r_admin = self.get_html('/User/SwitchToDistrictAdmin.aspx')
        self.parse_body_(r_admin)
        return self

class BaseTestCase(unittest.TestCase):
    # noinspection PyMethodMayBeStatic
    def random_date(self, start_date, end_date):
        # type: (string, string) -> string
        start_date1 = datetime.strptime(start_date, "%Y-%m-%d").date()
        end_date1 = datetime.strptime(end_date, "%Y-%m-%d").date()

        delta = (end_date1 - start_date1).days
        random_date_for_attendance = start_date1 + timedelta(random.randint(0, delta))

        return datetime.strptime(str(random_date_for_attendance), '%Y-%m-%d').strftime('%m-%d-%Y')


class BaseAuthedTestCase(BaseTestCase):
    def setUp(self):
        self.teacher = TeacherSession(self).login(user_email, user_pwd)

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


        # getting grading periods and dates
        self.list_for_grading_periods_and_classes = []
        var_grading_periods_list_for_dict = re.findall('var gradingPeriods = .+', page_as_one_string)
        var_grading_periods_string_for_dict = ''.join(var_grading_periods_list_for_dict)
        var_grading_periods_cut_off_string_for_dict = var_grading_periods_string_for_dict[30:-3]
        dictionary_var_grading_periods_cut_off_string_for_dict = json.loads(var_grading_periods_cut_off_string_for_dict)
        #print dictionary_var_grading_periods_cut_off_string_for_dict, type(dictionary_var_grading_periods_cut_off_string_for_dict)
        dictionary_var_grading_periods_cut_off_string_for_dict_data = dictionary_var_grading_periods_cut_off_string_for_dict['data']
        for j in dictionary_var_grading_periods_cut_off_string_for_dict_data:
            #print j['startdate'], j['enddate']
            self.list_for_grading_periods_and_classes.append({'startdate': j['startdate'], 'enddate': j['enddate']})

        #gradingComments
        var_grading_comments_list = re.findall('var gradingComments = .+', page_as_one_string)
        var_grading_comments_string = ''.join(var_grading_comments_list)
        var_grading_comments_cut_off_string = var_grading_comments_string[31:-3]
        dictionary_var_grading_comments_cut_off_string = json.loads(var_grading_comments_cut_off_string)
        dictionary_var_grading_comments_cut_off_string_data = dictionary_var_grading_comments_cut_off_string['data']

        self.dict_for_codes = {}
        self.one_grading_comment = random.choice(dictionary_var_grading_comments_cut_off_string_data)
        #print self.one_grading_comment, 'self.one_grading_comment'
        self.dict_for_codes['comment'] = self.one_grading_comment['comment']
        self.dict_for_codes['code'] = self.one_grading_comment['code']
        self.dict_for_codes['id'] = self.one_grading_comment['id']

        # alphaGrades
        var_alpha_grades_list = re.findall('var alphaGrades = .+', page_as_one_string)
        var_alpha_grades_string = ''.join(var_alpha_grades_list)
        var_alpha_grades_cut_off_string = var_alpha_grades_string[27:-3]
        dictionary_var_alpha_grades_cut_off_string = json.loads(var_alpha_grades_cut_off_string)
        dictionary_var_alpha_grades_cut_off_string_data = dictionary_var_alpha_grades_cut_off_string['data']

        # classesAdvancedData getting allowed standards
        self.dic_for_class_allowed_standard = {}
        var_classes_advanced_data_list = re.findall('var classesAdvancedData = .+', page_as_one_string)
        var_classes_advanced_data_string = ''.join(var_classes_advanced_data_list)
        var_classes_advanced_data_off_string = var_classes_advanced_data_string[35:-3]
        #print var_classes_advanced_data_off_string, type(var_classes_advanced_data_off_string)
        dictionary_var_classes_advanced_data_cut_off_string = json.loads(var_classes_advanced_data_off_string)
        #print dictionary_var_classes_advanced_data_cut_off_string, type(dictionary_var_classes_advanced_data_cut_off_string)

        for i in dictionary_var_classes_advanced_data_cut_off_string['data']:
            for k in i['alphagradesforstandards']:
                if not (i['classid'] in self.dic_for_class_allowed_standard):
                    self.dic_for_class_allowed_standard[i['classid']] = []
                self.dic_for_class_allowed_standard[i['classid']].append(k['id'])

        dictionary_var_classes_advanced_data_cut_off_string_data = dictionary_var_classes_advanced_data_cut_off_string['data']

        # getting marking periods
        var_marking_periods_list = re.findall('var markingPeriods = .+', page_as_one_string)
        var_markingPeriods_string = ''.join(var_marking_periods_list)
        var_markingPeriods_cut_off_string = var_markingPeriods_string[31:-3]
        dictionary_var_markingPeriods_cut_off_string = json.loads(var_markingPeriods_cut_off_string)

        self.dict_for_marking_period_date_startdate_endate = {}
        dictionary_var_markingPeriods_cut_off_string_data = dictionary_var_markingPeriods_cut_off_string['data']
       # print 'dictionary_var_markingPeriods_cut_off_string_data', dictionary_var_markingPeriods_cut_off_string_data

        for marking_period in dictionary_var_markingPeriods_cut_off_string_data:
            self.dict_for_marking_period_date_startdate_endate[marking_period['id']] = marking_period['startdate'], marking_period['enddate']
            #print 'marking_period', marking_period

        # getting list of marking periods
        self.list_for_marking_periods = []
        self.list_for_marking_periods_dates = []
        for key, value in self.dict_for_marking_period_date_startdate_endate.iteritems():
            self.list_for_marking_periods.append(key)
            self.list_for_marking_periods_dates.append(value)

        self.list_for_marking_periods_dates2 = []
        #print self.list_for_marking_periods_dates[0][0], type(self.list_for_marking_periods_dates[0][0])
        for k in self.list_for_marking_periods_dates:
            for i in k:
                self.list_for_marking_periods_dates2.append(str(i))

        self.start_date_school_year = min(self.list_for_marking_periods_dates2)
        self.end_date_school_year = max(self.list_for_marking_periods_dates2)

        # getting pairs 'class/marking period'
        var_classesToFilter_list = re.findall('var classesToFilter = .+', page_as_one_string)
        var_classesToFilter_string = ''.join(var_classesToFilter_list)
        var_markingPeriods_cut_off_string = var_classesToFilter_string[31:-3]
        dictionary_var_markingPeriods_cut_off_string = json.loads(var_markingPeriods_cut_off_string)
        self.dict_for_clas_marking_period = {}
        self.list_of_classes = []
        dictionary_var_markingPeriods_cut_off_string_data = dictionary_var_markingPeriods_cut_off_string['data']

        for info_for_one_class in dictionary_var_markingPeriods_cut_off_string_data:
           # print 'info_for_one_class',info_for_one_class
            self.dict_for_clas_marking_period[info_for_one_class['id']] = info_for_one_class['markingperiodsid']

        # getting list of classes
        for k in dictionary_var_markingPeriods_cut_off_string_data:
            self.list_of_classes.append(k['id'])

        self.session = s

        self.session_student = s_student

        self.session_foreign_student = s_foreign_student

        self.session_admin = s_admin

        self.session_classmate = s_classmate

    def post(self, url, params, status=200, success=True):
        s = self.session
        headers = {'X-Requested-With': 'XMLHttpRequest'}
        try:
            r = s.post(chlk_server_url + url, data=params, headers=headers)
        except ValueError:
            self.assertTrue(False, 'Request failed, ' + url)
            return None
        return self.verify_response(r, status, success)

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

    def get_file(self, url, status=200, success=True):
        s = self.session
        r = s.get(chlk_server_url + url)
        self.assertEquals(r.status_code, status, 'Response status code: ' + str(r.status_code) + ', body:' + r.text)
        return r.text

    def postJSON(self, url, obj, status=200, success=True, files=None):
        s = self.session
        headers = {'X-Requested-With': 'XMLHttpRequest'}
        r = s.post(chlk_server_url + url, json=obj, headers=headers, files=files)
        return self.verify_response(r, status, success)

    def postJSON_Assessment(self, url, obj, status=200, success=True, files=None):
        s = self.session
        headers = {'X-Requested-With': 'XMLHttpRequest'}

        try:
            r = s.post(chlk_server_url + url, data=obj, headers=headers, files=files)
        except ValueError:
            self.assertTrue(False, 'Request failed, ' + url)
            return None

        return self.verify_response(r, status, success)

    def verify_response(self, r, status, success, success_property='success'):
        self.assertEquals(r.status_code, status, 'Response status code: ' + str(r.status_code) + ', body:' + r.text)

        try:
            data = r.json()
        except ValueError:
            self.assertTrue(False, 'Parse JSON failed, ' + r.url + r.text)
            return None

        self.assertEquals(data[success_property], success,
                          'API success ' + str(data[success_property]) + '=' + str(success))

        return data

    # these methods for the student
    def get_student(self, url_student, status=200, success=True):
        s_student = self.session_student
        headers = {'X-Requested-With': 'XMLHttpRequest'}
        r_student = s_student.get(chlk_server_url + url_student, headers=headers)
        return self.verify_response(r_student, status, success)

    def postJSON_student(self, url_student, obj, status=200, success=True):
        s_student = self.session_student
        headers = {'X-Requested-With': 'XMLHttpRequest'}
        r_student = s_student.post(chlk_server_url + url_student, json=obj, headers=headers)
        return self.verify_response(r_student, status, success)

    def post_student(self, url_student, params, status=200, success=True):
        s_student = self.session_student
        headers = {'X-Requested-With': 'XMLHttpRequest'}

        try:
            r_student = s_student.post(chlk_server_url + url_student, data=params, headers=headers)
        except ValueError:
            self.assertTrue(False, 'Request failed, ' + url_student)
            return None

        return self.verify_response(r_student, status, success)
    # the end of methods for the student

    # these methods for the foreign student
    def get_foreign_student(self, url_foreign_student, status=200, success=True):
        s_foreign_student = self.session_foreign_student
        headers = {'X-Requested-With': 'XMLHttpRequest'}
        r_foreign_student = s_foreign_student.get(chlk_server_url + url_foreign_student, headers=headers)
        return self.verify_response(r_foreign_student, status, success)

    def postJSON_foreign_student(self, url_foreign_student, obj, status=200, success=True):
        s_foreign_student = self.session_foreign_student
        headers = {'X-Requested-With': 'XMLHttpRequest'}
        r_foreign_student = s_foreign_student.post(chlk_server_url + url_foreign_student, json=obj, headers=headers)
        return self.verify_response(r_foreign_student, status, success)

    def post_foreign_student(self, url_foreign_student, params, status=200, success=True):
        s_foreign_student = self.session_foreign_student
        headers = {'X-Requested-With': 'XMLHttpRequest'}

        try:
            r_foreign_student = s_foreign_student.post(chlk_server_url + url_foreign_student, data=params, headers=headers)
        except ValueError:
            self.assertTrue(False, 'Request failed, ' + url_foreign_student)
            return None

        return self.verify_response(r_foreign_student, status, success)
    # the end of methods for the foreign student

    # these methods for classmate
    def get_classmate(self, url_classmate, status=200, success=True):
        s_classmate = self.session_classmate
        headers = {'X-Requested-With': 'XMLHttpRequest'}
        r_classmate = s_classmate.get(chlk_server_url + url_classmate, headers=headers)
        return self.verify_response(r_classmate, status, success)

    def postJSON_classmate(self, url_classmate, obj, status=200, success=True):
        s_classmate = self.session_classmate
        headers = {'X-Requested-With': 'XMLHttpRequest'}
        r_classmate = s_classmate.post(chlk_server_url + url_classmate, json=obj, headers=headers)
        return self.verify_response(r_classmate, status, success)

    def post_classmate(self, url_classmate, params, status=200, success=True):
        s_classmate = self.session_classmate
        headers = {'X-Requested-With': 'XMLHttpRequest'}

        try:
            r_classmate = s_classmate.post(chlk_server_url + url_classmate, data=params, headers=headers)
        except ValueError:
            self.assertTrue(False, 'Request failed, ' + url_classmate)
            return None

        return self.verify_response(r_classmate, status, success)
    # the end of methods for classmates

    # these methods for the admin
    def get_admin(self, url_admin, status=200, success=True):
        s_admin = self.session_admin
        r_admin = s_admin.get(chlk_server_url + url_admin)
        return self.verify_response(r_admin, status, success)

    def get_file_admin(self, url_admin, status=200, success=True):
        s_admin = self.session_admin
        r_admin = s_admin.get(chlk_server_url + url_admin)
        self.assertEquals(r_admin.status_code, status, 'Response status code: ' + str(r_admin.status_code) + ', body:' + r_admin.text)
        return r_admin.text

    def post_admin(self, url_admin, params, status=200, success=True):
        s_admin = self.session_admin
        headers = {'X-Requested-With': 'XMLHttpRequest'}
        try:
            r_admin = s_admin.post(chlk_server_url + url_admin, data=params, headers=headers)
        except ValueError:
            self.assertTrue(False, 'Request failed, ' + url_admin)
            return None
        return self.verify_response(r_admin, status, success)
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