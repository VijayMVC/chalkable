from base_auth_test import *

class TestFeed(BaseAuthedTestCase):
    def test_feed_mark_items(self):
        #data that will be used for "mark all done"
        self.settings_data2 = {'option':'3'}
        
        #data that will be used for for reset all filters on the feed
        self.dict = {}
        
        #data that needed for filtering by lesson plans
        self.settings_data = {'announcementType':'3', 'sortType':'0'}
        self.post('/Feed/SetSettings.json?', self.settings_data)   
        
        #get 10 'done' lesson plans
        done_lesson_plans = self.get('/Feed/List.json?start='+str(0)+'&classId=&complete=true&count='+str(10))
        for_lesson_plan_id = done_lesson_plans['data']['annoucementviewdatas']
       
        #making all of these lesson plans 'unmark'
        for item in for_lesson_plan_id:
            id = str(item['id'])
            type = str(item['type'])
            #print id
            self.post('/Announcement/Complete', {'announcementId':id, 'announcementType':type, 'complete':'false'})
        
        #getting all unmarked lesson plans
        verify_on_true = self.get('/Feed/List.json?start='+str(0)+'&classId=&complete=false&count='+str(100)) 
        verify_for_len = verify_on_true['data']['annoucementviewdatas']
        self.assertGreaterEqual(len(verify_for_len), 1, 'Verify that we have at least one unmarked lesson plan')
        All_len = len(verify_for_len)
        
        #data that needed for filtering by activities
        self.settings_data3 = {'announcementType':'1', 'sortType':'0'}
        self.post('/Feed/SetSettings.json?', self.settings_data3)   

        #get 10 'done' activities
        done_items = self.get('/Feed/List.json?start='+str(0)+'&classId=&complete=true&count='+str(10))
        for_item_id = done_items['data']['annoucementviewdatas']        
        
        #making all of these activities 'unmark'
        for item in for_item_id:
            id = str(item['id'])
            type = str(item['type'])
            #print id
            self.post('/Announcement/Complete', {'announcementId':id, 'announcementType':type, 'complete':'false'})
        
        #getting all unmarked activities
        verify_on_true = self.get('/Feed/List.json?start='+str(0)+'&classId=&complete=false&count='+str(100)) 
        verify_for_len = verify_on_true['data']['annoucementviewdatas']
        self.assertGreaterEqual(len(verify_for_len), 1, 'Verify that we have at least one unmarked lesson plan')
        
        #print  All_len
        
        #reset all filters on the feed
        self.post('/Feed/SetSettings.json?', self.dict)
        
        #getting all unmarked activities and lesson plans
        verify_on_true = self.get('/Feed/List.json?start='+str(0)+'&classId=&complete=false&count='+str(100)) 
        verify_for_len = verify_on_true['data']['annoucementviewdatas']
        self.assertGreaterEqual(len(verify_on_true), 2, 'Sum of all activities and lesson plans must be >=2')
        
        #applying mark all done' option
        self.post('/Announcement/Done.json?', self.settings_data2)   
        dictionary_get_items = self.get('/Feed/List.json?start='+str(0)+'&classId=&complete=false&count='+str(100))
        
        dictionary_verify_annoucementviewdatas_all = dictionary_get_items['data']['annoucementviewdatas']  
        #print 'LEN(dictionary_verify_annoucementviewdatas_all): ', len(dictionary_verify_annoucementviewdatas_all)
        
        self.assertFalse(dictionary_verify_annoucementviewdatas_all, 'dictionary of items exists')
        
        #return dictionary_get_items  07.03.2016
    def tearDown(self):
        get_all_unmarket_items = self.get('/Feed/List.json?start='+str(0)+'&classId=&complete=false&count='+str(1000)) 
        for_item_id = get_all_unmarket_items['data']['annoucementviewdatas'] 
        for item in for_item_id:
            id = str(item['id'])
            type = str(item['type'])
            #print id
            self.post('/Announcement/Complete', {'announcementId':id, 'announcementType':type, 'complete':'true'})
            
if __name__ == '__main__':
    unittest.main()