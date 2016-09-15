import UserSession
import json
import re


class TeacherSession(UserSession):
    def __init__(self, unittest):
        pass

        self.teacher_id = None

    def parse_body_(self, page_as_one_string):
        # getting id of the current teacher
        tmp = re.findall('var currentChlkPerson = .+', page_as_one_string)
        tmp = ''.join(tmp)
        tmp = tmp[31:-3]
        tmp = json.loads(tmp)
        self.teacher_id = tmp['data']['id']
