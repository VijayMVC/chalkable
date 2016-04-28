import re
import unittest
import requests
from base_config import *
from datetime import datetime, timedelta
from datetime import timedelta
import time
import json
import random


class BaseAuthedTestCase(unittest.TestCase):
    def setUp(self):
        s = requests.Session();
        payload = {'UserName': user_email, 'Password': user_pwd, 'remember': 'false'}
        url = chlk_server_url + '/User/LogOn.aspx'
        r = s.post(url, data=payload)

        page_as_text = r.text
        page_as_one_string = str(page_as_text)

        # getting DistrictId
        found_sting = re.findall('var districtId = .+', page_as_one_string)
        concatenated_str = ''.join(found_sting)
        self.districtId = concatenated_str[18:54]

        # getting SchoolYearId
        found_sting2 = re.findall('var currentSchoolYearId = .+', page_as_one_string)
        concatenated_str2 = ''.join(found_sting2)
        self.schoolYearId = concatenated_str2[-5:-2]

        # getting grading periods
        found_sting3 = re.findall('var gradingPeriods = .+', page_as_one_string)
        concatenated_str2 = ''.join(found_sting3)
        found_sting4 = re.findall('"id":[0-9]+', concatenated_str2)
        concatenated_str3 = ''.join(found_sting4)
        self.found_sting5 = re.findall('[0-9]+', concatenated_str3)
        # print self.found_sting5

        # getting marking periods
        var_marking_periods_list = re.findall('var markingPeriods = .+', page_as_one_string)
        var_markingPeriods_string = ''.join(var_marking_periods_list)
        var_markingPeriods_cut_off_string = var_markingPeriods_string[31:-3]
        dictionary_var_markingPeriods_cut_off_string = json.loads(var_markingPeriods_cut_off_string)
        self.dict_for_marking_period_date_startdate_endate = {}
        dictionary_var_markingPeriods_cut_off_string_data = dictionary_var_markingPeriods_cut_off_string['data']

        for marking_period in dictionary_var_markingPeriods_cut_off_string_data:
            self.dict_for_marking_period_date_startdate_endate[marking_period['id']] = marking_period['startdate'], \
                                                                                       marking_period['enddate']

        # getting pairs 'class/marking period'
        var_classesToFilter_list = re.findall('var classesToFilter = .+', page_as_one_string)
        var_classesToFilter_string = ''.join(var_classesToFilter_list)
        var_markingPeriods_cut_off_string = var_classesToFilter_string[31:-3]
        dictionary_var_markingPeriods_cut_off_string = json.loads(var_markingPeriods_cut_off_string)
        self.dict_for_clas_marking_period = {}
        dictionary_var_markingPeriods_cut_off_string_data = dictionary_var_markingPeriods_cut_off_string['data']

        for info_for_one_class in dictionary_var_markingPeriods_cut_off_string_data:
            self.dict_for_clas_marking_period[info_for_one_class['id']] = info_for_one_class['markingperiodsid']

        self.session = s

    def gr_periods(self):
        list_of_gr_periods = self.found_sting5
        return list_of_gr_periods

    def get(self, url, status=200, success=True):
        s = self.session
        r = s.get(chlk_server_url + url)
        return self.verify_response(r, status, success)

    def postJSON(self, url, obj, status=200, success=True):
        s = self.session
        r = s.post(chlk_server_url + url, json=obj)
        return self.verify_response(r, status, success)

    def post(self, url, params, status=200, success=True):
        s = self.session

        try:
            r = s.post(chlk_server_url + url, data=params)
        except ValueError:
            self.assertTrue(False, 'Request failed, ' + url)
            return None

        return self.verify_response(r, status, success)

    def verify_response(self, r, status, success):
        self.assertEquals(r.status_code, status, 'Response status code')

        try:
            data = r.json()
        except ValueError:
            self.assertTrue(False, 'Parse JSON failed, ' + r.url)
            return None

        self.assertEquals(data['success'], success, 'API success')

        return data

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
