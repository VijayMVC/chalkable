import requests
import json
import sys
import re
import unittest
from base_config import *

class BaseAuthedTestCase(unittest.TestCase):
  def setUp(self):
    s = requests.Session();
    payload = {'UserName':user_email, 'Password':user_pwd, 'remember':'false'}
    url = chlk_server_url + '/User/LogOn.aspx'
    r = s.post(url, data = payload)
    
    page_as_text = r.text
    page_as_one_string = str(page_as_text)
    
    #getting DistrictId
    found_sting = re.findall('var districtId = .+', page_as_one_string)
    concatenated_str = ''.join(found_sting)
    self.districtId = concatenated_str[18:54]

    #getting SchoolYearId
    found_sting2 = re.findall('var currentSchoolYearId = .+', page_as_one_string)
    concatenated_str2 = ''.join(found_sting2)
    self.schoolYearId = concatenated_str2[-5:-2]
      
    self.session = s
    
  def get(self, url, status=200, success=True):
    s = self.session
    r = s.get(chlk_server_url + url)
    self.assertEquals(r.status_code, status, 'Response status code')
    data = r.json()
    self.assertEquals(data['success'], success, 'API success')
    return data

if __name__ == '__main__':
    unittest.main()