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
        
        #get 'done' lesson plans
        done_lesson_plans = self.get('/Feed/List.json?start='+str(0)+'&classId=&complete=true&count='+str(500))
        for_lesson_plan_id = done_lesson_plans['data']['annoucementviewdatas']
       
        #making all of these lesson plans 'unmark'
        for item in for_lesson_plan_id:
            id = str(item['id'])
            type = str(item['type'])
            self.post('/Announcement/Complete', {'announcementId':id, 'announcementType':type, 'complete':'false'})
        
        #getting all unmarked lesson plans
        verify_on_true = self.get('/Feed/List.json?start='+str(0)+'&classId=&complete=false&count='+str(500)) 
        verify_for_len = verify_on_true['data']['annoucementviewdatas']
        self.assertGreaterEqual(len(verify_for_len), 1, 'Verify that we have at least one unmarked lesson plan')
        
        #data that needed for filtering by activities
        self.settings_data3 = {'announcementType':'1', 'sortType':'0'}
        self.post('/Feed/SetSettings.json?', self.settings_data3)   

        #get 'done' activities
        done_items = self.get('/Feed/List.json?start='+str(0)+'&classId=&complete=true&count='+str(500))
        for_item_id = done_items['data']['annoucementviewdatas']        
        
        #making all of these activities 'unmark'
        for item in for_item_id:
            id = str(item['id'])
            type = str(item['type'])
            self.post('/Announcement/Complete', {'announcementId':id, 'announcementType':type, 'complete':'false'})
        
        #getting all unmarked activities
        verify_on_true = self.get('/Feed/List.json?start='+str(0)+'&classId=&complete=false&count='+str(500)) 
        verify_for_len = verify_on_true['data']['annoucementviewdatas']
        self.assertGreaterEqual(len(verify_for_len), 1, 'Verify that we have at least one unmarked lesson plan')
        
        #reset all filters on the feed
        self.post('/Feed/SetSettings.json?', self.dict)
        
        #getting all unmarked activities and lesson plans
        verify_on_true = self.get('/Feed/List.json?start='+str(0)+'&classId=&complete=false&count='+str(1000)) 
        verify_for_len = verify_on_true['data']['annoucementviewdatas']
        self.assertGreaterEqual(len(verify_on_true), 2, 'Sum of all activities and lesson plans must be >=2')

        #applying 'mark all done' option
        self.post('/Announcement/Done.json?', self.settings_data2)   
        dictionary_get_items = self.get('/Feed/List.json?start='+str(0)+'&classId=&complete=false&count='+str(1000))
        
        dictionary_verify_annoucementviewdatas_all = dictionary_get_items['data']['annoucementviewdatas']  
        
        self.assertFalse(dictionary_verify_annoucementviewdatas_all, "Verify items doesn't exist")

    def tearDown(self):
        #reset all filters on the feed
        self.dict = {}
        self.post('/Feed/SetSettings.json?', self.dict)
        
        #marking all items as 'done'
        #get_all_unmarket_items = self.get('/Feed/List.json?start='+str(0)+'&classId=&complete=false&count='+str(2000)) 
        self.settings_data = {'option':'3'}
        self.post('/Announcement/Done.json?', self.settings_data) 

if __name__ == '__main__':
    unittest.main()