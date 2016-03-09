from base_auth_test import *

class TestFeed(BaseAuthedTestCase):

    def test_sorting_by_last_name(self):
        self.do_student_sorting_lastname(0)    

    def do_student_sorting_lastname(self, start, count = 1000):
        empty_list =[]
        get_needed_page = self.get('/Student/GetStudents.json?myStudentsOnly=false&byLastName=true&start='+str(start)+'&count='+str(count))
        totalcount = get_needed_page['totalcount']
        totalpages = get_needed_page['totalpages']
        get_student = get_needed_page['data']
        for item in get_student:
            p = item['lastname']
            empty_list.append(p)
         
        decoded_list = [x.encode('utf-8') for x in empty_list]
        lower_case_list = map(str.lower, decoded_list)
        self.assertEqual(lower_case_list, sorted(lower_case_list), 'Students sorted by Last Name')
        
        
if __name__ == '__main__':
    unittest.main()    