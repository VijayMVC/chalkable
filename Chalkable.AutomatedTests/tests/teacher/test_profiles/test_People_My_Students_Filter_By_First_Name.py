from base_auth_test import *

class TestFeed(BaseAuthedTestCase):

    def test_people_student_sorting_by_first_name(self):
        self.do_student_sorting_firstname(0)
        self.do_student_sorting_firstname(10)
        self.do_student_sorting_firstname(20)         

    def do_student_sorting_firstname(self, start, count = 999):
        empty_list =[]
        empty_list2 =[]
        
        get_needed_page = self.get('/Student/GetStudents.json?myStudentsOnly=true&byLastName=false&start='+str(start)+'&count='+str(count))
        
        get_student = get_needed_page['data']
        totalcount = get_needed_page['totalcount']
        totalpages = get_needed_page['totalpages']
        self.assertGreaterEqual(len(get_student), 1, 'Verify that list has at leat 1 student')
  
        for item in get_student:
            p = item['firstname']
            empty_list.append(p)
        decoded_list = [x.encode('utf-8') for x in empty_list]

        only_one_student = decoded_list[0]
        
        get_filtered_page = self.get('/Student/GetStudents.json?classId=&filter='+str(only_one_student)+'&myStudentsOnly=true&byLastName=false&start='+str(start)+'&count='+str(count))
        one_or_two_students = get_filtered_page['data']
        #print len(one_or_two_students)
        
        if len(one_or_two_students) == 1:
            for item in one_or_two_students:
                a = item['firstname']
                self.assertEqual(only_one_student, a, 'Compare two lists of students')
 
        if len(one_or_two_students) > 1:
            for item in one_or_two_students:
                p = item['firstname']
                empty_list2.append(p)
            decoded_list2 = [x.encode('utf-8') for x in empty_list2]  
            
            for item in decoded_list2:
                self.assertEqual(item, only_one_student)
   
if __name__ == '__main__':
    unittest.main()    