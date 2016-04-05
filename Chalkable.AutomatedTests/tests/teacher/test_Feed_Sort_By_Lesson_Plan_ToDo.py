from base_auth_test import *


class TestFeed(BaseAuthedTestCase):
    def test_feed(self):
        #data that needed for filtering by lesson plans
        self.settings_data_lesson_plan = {'announcementType':'3', 'sortType':'0'}
        self.post('/Feed/SetSettings.json?', self.settings_data_lesson_plan)   
        
        #get 1000 'done' lesson plans
        done_lesson_plans = self.get('/Feed/List.json?start='+str(0)+'&classId=&complete=true&count='+str(100))
        for_lesson_plan_id = done_lesson_plans['data']['annoucementviewdatas']
       
        #making all of these lesson plans 'unmark'
        for item in for_lesson_plan_id:
            id = str(item['id'])
            type = str(item['type'])
            self.post('/Announcement/Complete', {'announcementId':id, 'announcementType':type, 'complete':'false'})

        #data that needed for filtering by activities
        self.settings_data_activity = {'announcementType':'1', 'sortType':'0'}
        self.post('/Feed/SetSettings.json?', self.settings_data_activity)   

        #get 1000 'done' activities
        done_items = self.get('/Feed/List.json?start='+str(0)+'&classId=&complete=true&count='+str(100))
        for_item_id = done_items['data']['annoucementviewdatas']        
        
        #making all of these activities 'unmark'
        for item in for_item_id:
            id = str(item['id'])
            type = str(item['type'])
            #print id
            self.post('/Announcement/Complete', {'announcementId':id, 'announcementType':type, 'complete':'false'})    

        #reset all filters on the feed
        self.dict = {}
        self.post('/Feed/SetSettings.json?', self.dict)

        ##########################################################################################
        self.data2 = {'type':'3'}
        self.settings_data = {'announcementType':'3', 'sortType':'0'}
    
        self.post('/Feed/SetSettings.json?', self.settings_data)   
    
        self.do_feed_list_and_verify(0)    

    def do_feed_list_and_verify(self, start, count=100):
        dictionary_get_items = self.get('/Feed/List.json?start='+str(start)+'&classId=&complete=false&count='+str(count))
        dictionary_verify_settingsforfeed = dictionary_get_items['data']['settingsforfeed']
        self.assertTrue('announcementtype' in dictionary_verify_settingsforfeed, 'announcementtype exists')
        self.assertTrue('sorttype' in dictionary_verify_settingsforfeed, 'sorttype exists')
        announcementtype = str(dictionary_verify_settingsforfeed['announcementtype'])
        sorttype = str(dictionary_verify_settingsforfeed['sorttype'])
        self.assertDictContainsSubset(self.settings_data, {'announcementType':announcementtype, 'sortType':sorttype}, 'key/value pair exists')
    
        dictionary_verify_annoucementviewdatas_all = dictionary_get_items['data']['annoucementviewdatas']   
        for item in dictionary_verify_annoucementviewdatas_all:
            type = str(item['type'])
            self.assertDictContainsSubset(self.data2, {'type':type}, 'key/value pair exists')
    
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