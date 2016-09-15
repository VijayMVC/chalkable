from base_config import *
import requests
import re


class UserSession:
    def __init__(self, unittest):
        pass
        self.unittest = unittest

        self.session = requests.Session()
        self.districtId = None
        self.schoolYearId = None

    def login(self, email, passwd):
        r = self.get_html('/User/Login.json?UserName='+email+'&Password='+passwd+'&Remember=false')
        self.parse_body_(r.text)
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

    def get_(self, url, status, **kwargs):
        r = self.session.get(chlk_server_url + url, **kwargs)
        self.unittest.assertEquals(r.status_code, status, 'Response status code: ' + str(r.status_code) + ', body:' + r.text)
        return r

    def post_(self, url, status, **kwargs):
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