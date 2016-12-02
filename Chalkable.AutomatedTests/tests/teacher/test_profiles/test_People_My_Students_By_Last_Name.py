from base_auth_test import *

class TestFeed(BaseAuthedTestCase):

    def test_sorting_by_last_name(self):
       
        self.do_student_sorting_lastname(0)    

    def do_student_sorting_lastname(self, start, count = 1000):
        empty_list =[]
        dictionary_get_students = self.get('/Student/GetStudents.json?myStudentsOnly=true&byLastName=true&start='+str(start)+'&count='+str(count))
        get_student = dictionary_get_students['data']
        for item in get_student:
            p = item['lastname']
            empty_list.append(p)
            
        totalcount = dictionary_get_students['totalcount']
        totalpages = dictionary_get_students['totalpages']
        self.assertEqual(empty_list, sorted(empty_list), 'Students sorted by Last Name')
        
if __name__ == '__main__':
    unittest.main()    