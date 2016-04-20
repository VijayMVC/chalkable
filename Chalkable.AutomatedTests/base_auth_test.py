import re
import unittest
import requests
from base_config import *
from datetime import datetime
from datetime import timedelta
import time

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
        #print self.found_sting5
        self.session = s
        
    def gr_periods(self):
        k = self.found_sting5
        return k
            
    def get(self, url, status=200, success=True):
        s = self.session
        r = s.get(chlk_server_url + url)
        return self.verifyResponce(r, status, success)

    def postJSON(self, url, obj, status=200, success=True):
        s = self.session
        r = s.post(chlk_server_url + url, json=obj)
        return self.verifyResponce(r, status, success)

    def post(self, url, params, status=200, success=True):
        s = self.session
        r = s.post(chlk_server_url + url, data=params)
        return self.verifyResponce(r, status, success)

    def verifyResponce(self, r, status, success):
        # print r.status_code
        self.assertEquals(r.status_code, status, 'Response status code')

        try:
            data = r.json()
        except ValueError:
            print 'Decoding JSON has failed'

        self.assertEquals(data['success'], success, 'API success')
        return data
        

if __name__ == '__main__':
    unittest.main()