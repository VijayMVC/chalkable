from base_auth_test import *

class TestFeed(BaseAuthedTestCase):
    def test_people_list(self):
      
        self.do_people_loading(0)    
        
    def do_people_loading(self, start, count = 10):
        dictionary_get_items = self.get('/Student/GetStudents.json?myStudentsOnly=false&byLastName=true&start='+str(start)+'&count='+str(count)) 
        totalcount = dictionary_get_items['totalcount']
        totalpages = dictionary_get_items['totalpages']
        hasnextpage = dictionary_get_items['hasnextpage']
        haspreviouspage = dictionary_get_items['haspreviouspage']
        pageindex = dictionary_get_items['pageindex']
       
        index = 1
        while (index < min (3, totalpages)):
            #print start
            self.get('/Student/GetStudents.json?myStudentsOnly=true&byLastName=true&start='+str(start)+'&count='+str(count)) 
            index += 1
            start += 10
        if (totalcount > 10*3):
            self.get('/Student/GetStudents.json?myStudentsOnly=true&byLastName=true&start='+str(30)+'&count='+str(1000))
            self.assertGreater(totalcount, 30, 'Verify number of students are more than 30')
        if (totalcount == 0): 
            self.assertEqual(totalpages, 0, 'Verify totalpages is <=1')
            self.assertFalse(haspreviouspage, 'Verify haspreviouspage is false')
            self.assertFalse(hasnextpage, 'Verify hasnextpage is false')
            self.assertEqual(pageindex, 0, 'Verify pageindex 0') 
        if (totalcount >=1 and totalcount <=10):    
            self.assertEqual(totalpages, 1, 'Verify totalpages is <=1')
            self.assertFalse(haspreviouspage, 'Verify haspreviouspage is false')
            self.assertFalse(hasnextpage, 'Verify hasnextpage is false')
            self.assertEqual(pageindex, 0, 'Verify pageindex 0')
                
if __name__ == '__main__':
    unittest.main()
    
    
'''    
MABELL-239188226  user has 19 students
ADPOLLARD-1233830209  user has 0 students
'''   